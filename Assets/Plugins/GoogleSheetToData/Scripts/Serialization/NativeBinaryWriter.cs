using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Rui.IO.Serialization
{
    /// <summary>
    /// 변수를 바이너리 데이터로 변경하여 작성하는데 사용되는 구조체 입니다
    /// 버스트 기능을 지원합니다. / 작성완료시 Dispose 필요
    /// 구조체이지만 참조형식처럼 사용해도 됩니다
    /// MAX 4GB
    /// </summary>
    public unsafe class NativeBinaryWriter : IDisposable
    {
        private const uint MAXCAPACITY = uint.MaxValue;
        private const uint CacheLineSize = 64;
        
        private IntPtr _array;
        private long _length;
        private long _capacity;

        public long Length => _length;
        public long Position => _length;
        
        public NativeBinaryWriter(long capacity)
        {
            _array = Marshal.AllocHGlobal(new IntPtr(capacity));
            _capacity = (uint)capacity;
            _length = 0;
        }
        
        /// <summary>
        /// 언제든 바뀝니다 조심
        /// </summary>
        /// <returns></returns>
        public void* GetPtr() => _array.ToPointer();
        
        #region BeginWrite / EnsureCapacity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void* BeginWrite(int byteLength) => BeginWrite((uint)byteLength);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void* BeginWrite(uint byteLength)
        {
            long required = _length + byteLength;
            CheckAndEnsureCapacity(required);
            long pos = _length;
            _length += byteLength;
            return (byte*)_array + pos;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private void CheckAndEnsureCapacity(long required)
        {
            if (_capacity < required)
                EnsureCapacity(required);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)] 
        private void EnsureCapacity(long required)
        {
            long t_capacity = _capacity + (_capacity >> 1); // 1.5배
            if (t_capacity < required) t_capacity = required;
            // Align Up to CacheLineSize (64)
            t_capacity = (t_capacity + CacheLineSize - 1) & ~(CacheLineSize - 1);
            if (t_capacity >= MAXCAPACITY)
                throw new OutOfMemoryException();
            _array = Marshal.ReAllocHGlobal(_array, new IntPtr(t_capacity));
            _capacity = t_capacity;
        }
        #endregion
        
        /// <summary> 구조체로 구성된 데이터를 작성합니다 </summary>
        public void Write<T>(T data) where T : unmanaged
        {
            (*(T*)BeginWrite(Unsafe.SizeOf<T>())) = data;
        }

        public void Write(IntPtr ptr, uint len, uint size)
        {
            uint byteLen = len * size;
            Write(len);
            void* destPtr = BeginWrite(byteLen);
            void* srcPtr = (void*)(ptr);
            Unsafe.CopyBlock(destPtr, srcPtr, byteLen);
        }
        
        /// <summary> 1byte로 채워지는 여유 공간을 작성합니다</summary>
        public void WritePadding(int byteLen)
        {
            void* destPtr =  BeginWrite(byteLen);
            Unsafe.InitBlock(destPtr, 0, (uint)byteLen);
        }

        public void Write<T>(ReadOnlySpan<T> data) where T : unmanaged
        {
            int byteLen = Unsafe.SizeOf<T>() * data.Length;
            Write(data.Length); // 개수 기록
            void* destPtr = BeginWrite(byteLen);
            fixed (void* srcPtr = &MemoryMarshal.GetReference(data))
            {
                Unsafe.CopyBlock(destPtr, srcPtr, (uint)byteLen);
            }
        }

        public void Write<T>(T[] data) where T : unmanaged
        {
            int byteLen = Unsafe.SizeOf<T>() * data.Length;
            Write(data.Length);
            void* destPtr = BeginWrite(byteLen);
            fixed (void* srcPtr = &data[0])
            {
                Unsafe.CopyBlock(destPtr, srcPtr, (uint)byteLen);
            }
        }
        
        public void Write(string str)
        {
            if(string.IsNullOrEmpty(str))
                Write(0);
            else
            {
                int byteCount = Encoding.UTF8.GetByteCount(str);
                Write(byteCount); 
                byte* ptr = (byte*)BeginWrite(byteCount);
                fixed (char* charPtr = str)
                {
                    Encoding.UTF8.GetBytes(charPtr, str.Length, ptr, byteCount);
                }
            }
        }
        
        public void Dispose()
        {
            Marshal.FreeHGlobal(_array);
            _length = _capacity = 0;
        }
        
        #region T1....T7
        public void Write<T1, T2>(T1 _1, T2 _2)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            int allLen = sizeof(T1) + sizeof(T2);
            CheckAndEnsureCapacity(_length + allLen);
            byte* start = (byte*)_array + _length;
            *(T1*)start = _1;
            *(T2*)(start + sizeof(T1)) = _2;
            _length += allLen;
        }

        public void Write<T1, T2, T3>(T1 _1, T2 _2, T3 _3) 
            where T1 : unmanaged 
            where T2 : unmanaged
            where T3 : unmanaged
        {
            int allLen =  sizeof(T1) + sizeof(T2) + sizeof(T3);
            CheckAndEnsureCapacity(_length + allLen);
            byte* start = (byte*)_array + _length;
            *(T1*)start = _1;
            *(T2*)(start + sizeof(T1)) = _2;
            *(T3*)(start + sizeof(T1) + sizeof(T2)) = _3;
            _length += allLen;
        }
        
        public void Write<T1, T2, T3, T4>(T1 _1, T2 _2, T3 _3, T4 _4) 
            where T1 : unmanaged 
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
        {
            int  allLen =  sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4);
            CheckAndEnsureCapacity(_length + allLen);
            byte* start = (byte*)_array + _length;
            *(T1*)start = _1;
            *(T2*)(start + sizeof(T1)) = _2;
            *(T3*)(start + sizeof(T1) + sizeof(T2)) = _3;
            *(T4*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3)) = _4;
            _length += allLen;
        }

        public void Write<T1, T2, T3, T4, T5>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
        {
            int allLen =  sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4) + sizeof(T5);
            CheckAndEnsureCapacity(_length + allLen);
            byte* start = (byte*)_array + _length;
            *(T1*)start = _1;
            *(T2*)(start + sizeof(T1)) = _2;
            *(T3*)(start + sizeof(T1) + sizeof(T2)) = _3;
            *(T4*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3)) = _4;
            *(T5*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4)) = _5;
            _length += allLen;
        }
        public void Write<T1, T2, T3, T4, T5, T6>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
        {
            int allLen =  sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4) + sizeof(T5) + sizeof(T6);
            CheckAndEnsureCapacity(_length + allLen);
            byte* start = (byte*)_array + _length;
            *(T1*)start = _1;
            *(T2*)(start + sizeof(T1)) = _2;
            *(T3*)(start + sizeof(T1) + sizeof(T2)) = _3;
            *(T4*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3)) = _4;
            *(T5*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4)) = _5;
            *(T6*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4) + sizeof(T5)) = _6;
            _length += allLen;
        }
        public void Write<T1, T2, T3, T4, T5, T6, T7>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
            where T7 : unmanaged
        {
            int allLen =  sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4) + sizeof(T5) + sizeof(T6) + sizeof(T7);
            CheckAndEnsureCapacity(_length + allLen);
            byte* start = (byte*)_array + _length;
            *(T1*)start = _1;
            *(T2*)(start + sizeof(T1)) = _2;
            *(T3*)(start + sizeof(T1) + sizeof(T2)) = _3;
            *(T4*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3)) = _4;
            *(T5*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4)) = _5;
            *(T6*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4) + sizeof(T5)) = _6;
            *(T7*)(start + sizeof(T1) + sizeof(T2) + sizeof(T3) + sizeof(T4) + sizeof(T5) + sizeof(T6)) = _7;
            _length += allLen;
        }
        #endregion
    }
}