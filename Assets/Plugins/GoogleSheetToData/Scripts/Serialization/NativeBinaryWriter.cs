using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Rui.IO.Serialization
{
    /// <summary>
    /// 변수를 바이너리 데이터로 변경하여 작성하는데 사용되는 구조체 입니다
    /// 버스트 기능을 지원합니다. / 작성완료시 Dispose 필요
    /// 구조체이지만 참조형식처럼 사용해도 됩니다
    /// </summary>
    public unsafe struct NativeBinaryWriter : IDisposable
    {
        [NativeDisableUnsafePtrRestriction] private UnsafeList<byte>* _binary;

        public int Length => _binary->Length;

        public NativeBinaryWriter(int capacity, Allocator allocator)
        {
            _binary = UnsafeList<byte>.Create(capacity, allocator);
        }

        public void* GetPtr()
        {
            return _binary->Ptr;
        }

        public int GetNowPosition()
        {
            return Length;
        }

        private void* Position(int byteLength,int customPosition = -1)
        {
            if (customPosition < 0)
            {
                int pos = _binary->Length;
                _binary->Length += byteLength;
                return (byte*)IntPtr.Add((IntPtr)_binary->Ptr, pos);
            }
            else
            {
                return (byte*)IntPtr.Add((IntPtr)_binary->Ptr, customPosition);
            }
        }
        
        /// <summary> 구조체로 구성된 데이터를 작성합니다 </summary>
        public void Write<T>(T data, int customPosition = -1) where T : unmanaged
        {
            int size = UnsafeUtility.SizeOf<T>();
            (*(T*)Position(size, customPosition)) = data;
        }

        /// <summary> 구조체로 구성된 NativeList<T> 을 빠르게 작성합니다 </summary>
        public void Write<T>(NativeArray<T> data,int customPosition =-1) where T : unmanaged
        {
            int size = UnsafeUtility.SizeOf<T>() * data.Length;
            Write(size); //add SizeData
            void* destPtr = Position(size, customPosition);
            void* srcPtr = (void*)((IntPtr)data.GetUnsafePtr());
            UnsafeUtility.MemCpy(destPtr, srcPtr, size);
        }

        /// <summary> 구조체로 구성된 UnsafeList<T> 을 빠르게 작성합니다 </summary>
        public void Write<T>(UnsafeList<T> data,int customPosition=-1) where T : unmanaged
        {
            int size = UnsafeUtility.SizeOf<T>() * data.Length;
            Write(size); //add SizeData
            void* destPtr = Position(size, customPosition); 
            void* srcPtr = (void*)((IntPtr)data.Ptr);
            UnsafeUtility.MemCpy(destPtr, srcPtr, size);
        }

        /// <summary> 0으로 채워지는 여유 공간을 작성합니다</summary>
        public void WritePadding(int size,int customPosition=-1)
        {
            void* destPtr =  Position(size, customPosition);
            UnsafeUtility.MemSet(destPtr, 0, size);
        }

        #region managermentCode

        /// <summary>
        /// 배열을 바이너리에 작성 / 버스트미지원
        /// NativeContainer 사용하면 버스트 사용 가능 </summary>
        [BurstDiscard]
        public void Write<T>(T[] data,int customPosition = -1) where T : unmanaged
        {
            int size = UnsafeUtility.SizeOf<T>() * data.Length;
            Write(size); //add SizeData
            void* destPtr = Position(size, customPosition);
            fixed (void* srcPtr = &data[0])
            {
                UnsafeUtility.MemCpy(destPtr, srcPtr, size);
            }
        }

        /// <summary>
        /// List<T> 바이너리에 작성 / 버스트미지원
        /// NativeContainer 함수 사용하면 버스트 사용 가능 </summary>
        [BurstDiscard]
        public void Write<T>(List<T> data,int customPosition = -1) where T : unmanaged
        {
            void* dataPtr = UnsafeUtility.PinGCObjectAndGetAddress(data, out var gcHandle);
            int size = UnsafeUtility.SizeOf<T>() * data.Count;
            Write(size); //add SizeData
            void* destPtr = Position(size, customPosition);
            UnsafeUtility.MemCpy(destPtr, dataPtr, size);
            UnsafeUtility.ReleaseGCObject(gcHandle);
        }


        /// <summary>
        /// string을 바이너리에 작성 / 버스트미지원
        /// 문자열 최대길이가 고정일 경우 FixedString64Bytes 권장</summary>
        [BurstDiscard]
        public void Write(string str)
        {
            if(str.Length == 0)
                Write(0);
            else
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                Write(bytes);
            }
        }


        #endregion
        
        public void Dispose()
        {
            UnsafeList<byte>.Destroy(_binary);
        }
    }
}