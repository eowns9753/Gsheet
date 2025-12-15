using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Burst;
using Unity.Collections;

namespace Rui.IO.Serialization
{
    ///일단 라이브러리화 하고 추후 프로토버퍼처럼 백/프 동기화 지원
    /// 재미니를통해 잠재적 위험요소 제거해가면서 개선작업 후 배포
    /// <summary>
    /// 변수를 바이너리 데이터로 변경하여 작성하는데 사용되는 구조체 입니다
    /// 버스트 기능을 지원합니다. / 작성완료시 Dispose 필요
    /// 구조체이지만 참조형식처럼 사용해도 됩니다
    /// </summary>
    public unsafe struct NativeBinaryWriter : IDisposable
    {
        private const uint MAXCAPACITY = uint.MaxValue;
        private const int CacheLineSize = 64;
        
        private IntPtr _array;
        private uint _length;
        private uint _capacity;

        public uint Length => _length;
        public uint Position => _length;
        
        public NativeBinaryWriter(int capacity)
        {
            _array = Marshal.AllocHGlobal(new IntPtr(capacity));
            _capacity = (uint)capacity;
            _length = 0;
        }
        
        public NativeBinaryWriter(uint capacity)
        {
            _array = Marshal.AllocHGlobal(new IntPtr(capacity));
            _capacity = capacity;
            _length = 0;
        }

        public void* GetPtr() => _array.ToPointer();

        private void* W(uint byteLength)
        {
            if (_capacity < (byteLength + _length))
            {
                uint t_capacity = _capacity + (_capacity >> 1);
                t_capacity += t_capacity % CacheLineSize;
                if (t_capacity >= MAXCAPACITY)
                    throw new OutOfMemoryException();
                _array = Marshal.ReAllocHGlobal(_array, new IntPtr(t_capacity));
                _capacity = t_capacity;
            }
            uint pos = _length;
            _length += byteLength;
            return (byte*)_array + pos;
        }
        
        private void* W(int byteLength) => W((uint)byteLength);

        
        /// <summary> 구조체로 구성된 데이터를 작성합니다 </summary>
        public void Write<T>(T data) where T : unmanaged
        {
            int size =  Unsafe.SizeOf<T>();
            (*(T*)W(size)) = data;
        }

        public void Write(IntPtr ptr, uint len, uint size)
        {
            uint byteLen = len * size;
            Write(byteLen);
            void* destPtr = W(byteLen);
            void* srcPtr = (void*)(ptr);
            Unsafe.CopyBlock(destPtr, srcPtr, byteLen);
        }
        
        /// <summary> 1byte로 채워지는 여유 공간을 작성합니다</summary>
        public void WritePadding(int byteLen)
        {
            void* destPtr =  W(byteLen);
            Unsafe.InitBlock(destPtr, 0, (uint)byteLen);
        }

        public void Write<T>(T[] data) where T : unmanaged
        {
            int byteLen = Unsafe.SizeOf<T>() * data.Length;
            Write(byteLen); //add SizeData
            void* destPtr = W(byteLen);
            fixed (void* srcPtr = &data[0])
            {
                Unsafe.CopyBlock(destPtr, srcPtr, (uint)byteLen);
            }
        }
        
        public void Write<T>(List<T> data) where T : unmanaged
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            void* dataPtr = (void*)handle.AddrOfPinnedObject();
            int size = sizeof(T) * data.Count;
            Write(size); 
            void* destPtr = W(size);
            Unsafe.CopyBlock(dataPtr, destPtr, (uint)size);
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }
        
        public void Write(string str)
        {
            if(string.IsNullOrEmpty(str))
                Write(0);
            else
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                Write(bytes);
            }
        }
        
        #region T1....T10

        /*/// <summary> 구조체로 구성된 데이터를 작성합니다 </summary>
        public void Write<T1, T2>(T1 _1, T2 _2) where T1 : unmanaged where T2 : unmanaged
        {
            Unsafe.WriteUnaligned(); <<<이거로 대체
            (*(T1*)W(Unsafe.SizeOf<T1>())) = _1;
            (*(T2*)W(Unsafe.SizeOf<T2>())) = _2;
        }
        
        /// <summary> 구조체로 구성된 데이터를 작성합니다 </summary>
        public void Write<T1, T2, T3>(T1 _1, T2 _2, T3 _3) where T1 : unmanaged
        {
            (*(T1*)W(Unsafe.SizeOf<T1>())) = _1;
            (*(T2*)W(Unsafe.SizeOf<T2>())) = _2;
            (*(T3*)W(Unsafe.SizeOf<T3>())) = _3;
        }*/

        #endregion
        
        public void Dispose()
        {
            Marshal.FreeHGlobal(_array);
            _length = _capacity = 0;
        }
    }
}