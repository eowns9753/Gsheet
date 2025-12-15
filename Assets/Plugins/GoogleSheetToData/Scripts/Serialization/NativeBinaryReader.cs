using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using Object = System.Object;

namespace Rui.IO.Serialization
{
    /// <summary>
    /// 바이너리 데이터를 읽는데 사용되는 구조체입니다.
    /// 버스트 기능을 지원합니다. 
    /// </summary>
    public unsafe struct NativeBinaryReader
    {
        private void* _array;
        private int* _position;
        
        #region constructor
        public NativeBinaryReader(NativeArray<byte> datas,Allocator allocator)
        {
            _array = datas.GetUnsafePtr();
            _position = AllocatorManager.Allocate<int>(allocator);
            (*_position) = 0;
        }

        public NativeBinaryReader(NativeList<byte> datas, Allocator allocator)
        {
            _array = datas.GetUnsafePtr();
            _position = AllocatorManager.Allocate<int>(allocator);
            (*_position) = 0;
        }

        public NativeBinaryReader(NativeBinaryWriter writer, Allocator allocator)
        {
            _array = (void*)writer.GetPtr();
            _position = AllocatorManager.Allocate<int>(allocator);
            (*_position) = 0;
        }
        
        #endregion
        
        public unsafe IntPtr GetIntPtr()
        {
            return (IntPtr)_array + *_position;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void* GetPtr()
        {
            return (void*)((IntPtr)_array + *_position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPosition(int size)
        {
            *_position += size;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPosition(int pos)
        {
            *_position = pos;
        }

        /// <summary> T에 해당하는 형식으로 토큰값을 읽어 반환</summary>
        public T Read<T>() where T : unmanaged
        {
            int size = UnsafeUtility.SizeOf<T>();
            T result = *(T*)GetPtr();
            AddPosition(size);
            return result;
        }
        
        /// <summary> T에 해당하는 형식으로 토큰값을 읽어 반환</summary>
        public T ReadObject<T>() 
        {
            string a;
            
            /*int size = UnsafeUtility.SizeOf<T>();
            T result = *(T*)GetPtr();
            AddPosition(size);*/
            return default;
        }
        

        /// <summary> 토큰에 해당하는 메모리를 원형으로 사용한 변수 리스트를 반환합니다. 읽기전용</summary>
        public UnsafeList<T> ReadArray_Ref<T>() where T : unmanaged
        {
            int size = Read<int>();
            int typeSize = UnsafeUtility.SizeOf<T>();
            int len = size / typeSize;
            var result2 = new UnsafeList<T>((T*)GetPtr(), len);
            AddPosition(size);
            return result2;
        }

        /// <summary> list에 복사  </summary>
        public void ReadUnsafeList<T>(ref UnsafeList<T> list) where T : unmanaged
        {
            int size = Read<int>();
            list.Resize(size/ UnsafeUtility.SizeOf<T>());
            UnsafeUtility.MemCpy(list.Ptr, GetPtr(), size);
            AddPosition(size);
        }

        /// <summary> 토큰에 해당하는 메모리를 복사하여 생성한 변수 리스트를 반환합니다.  </summary>
        public void ReadNativeList<T>(ref NativeList<T> list) where T : unmanaged
        {
            int size = Read<int>();
            list.ResizeUninitialized(size / UnsafeUtility.SizeOf<T>());
            UnsafeUtility.MemCpy(list.GetUnsafePtr(), GetPtr(), size);
            AddPosition(size);
        }

        /// <summary> 0으로 채워진 여유공간을 스킵합니다 </summary>
        public void ReadPadding(int size)
        {
            AddPosition(size);
        }
        
        #region managermentCode
        /// <summary> 관리되는 Array<T>를 반환합니다 / 버스트미지원</summary>
        [BurstDiscard]
        public T[] ReadArray<T>() where T : unmanaged
        {
            int size = Read<int>();
            int typeSize = UnsafeUtility.SizeOf<T>();
            int len = size / typeSize;
            T[] result = new T[len];
            unsafe
            {
                fixed (void* dest = &result[0])
                {
                    UnsafeUtility.MemCpy(dest,GetPtr(),size);
                }
            }
            AddPosition(size);
            return result;
        }

        /// <summary> 문자열을 반환합니다. / 버스트미지원 </summary>
        [BurstDiscard]
        public string ReadString()
        {
            string result = "";
            int size = Read<int>();
            if (size == 0)
                return "";
            result = System.Text.Encoding.UTF8.GetString((byte*)GetPtr(), size);
            AddPosition(size);
            return result;
        }
        #endregion

        public void Dispose()
        {
            AllocatorManager.Free(Allocator.Persistent,_position);
        }
    }
}