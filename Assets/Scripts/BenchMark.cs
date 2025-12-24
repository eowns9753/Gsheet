using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading.Tasks;
using LWSerializer;
using MemoryPack;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    [MemoryPackable]
    public partial class BenchExample : ILwSerializable
    {
        public Vector2 direction;
        public float speed;
        public FixedString32Bytes testfxStr;
        public FixedString32Bytes testfxStr2;
        public float asdasd;

        public string aaaa;

        public Vector2[] VecArr = new Vector2[10];
        //public StructNameTest a { get; private set; }

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(direction, speed, testfxStr, testfxStr2, asdasd);
            writer.Write(aaaa);
            writer.Write(VecArr);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out direction, out speed, out testfxStr, out testfxStr2, out asdasd);
            reader.Read(out aaaa);
            reader.Read(out VecArr);
        }
    }
    
    public class BenchMark
    {
        private Stopwatch stp = new Stopwatch();
        public async Task Start()
        {
            Debug.Log("Test start");
            int cycle = 300000;

            
            var _memoryPackWriter = new ArrayBufferWriter<byte>(64); // 1MB 초기 용량
            var _nativeBuffer = new LwBinaryWriter(64);
            
            await Task.Delay(1000);
            WriteTofuMem(cycle, _nativeBuffer, new BenchExample());
            GC.Collect();
            await Task.Delay(1000);
            WriteMemPack(cycle, _memoryPackWriter, new BenchExample());
            GC.Collect();

            
            _memoryPackWriter.Clear();
            _nativeBuffer.Dispose();

            await Task.Delay(500);
            ReadTofu(cycle); 
            GC.Collect();
            await Task.Delay(500);
            ReadMemPack(cycle);
            GC.Collect();
        }

        private void WriteTofuMem(int count, LwBinaryWriter writer, BenchExample b)
        {
            stp.Restart();
            for (int i = 0; i < count; i++)
                writer.WriteRef(b);
            stp.Stop();
            Debug.Log($"{nameof(WriteTofuMem)} {count:N0} cycle({writer.Length/1000000} MB), {stp.ElapsedMilliseconds} ms");
        }

        private void WriteMemPack(int count, ArrayBufferWriter<byte> arr, BenchExample b)
        {
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                MemoryPackSerializer.Serialize(arr, b);
            }
            stp.Stop();
            Debug.Log($"{nameof(WriteMemPack)} {count:N0} cycle({arr.WrittenCount/1000000} MB), {stp.ElapsedMilliseconds} ms");
        }

        private void ReadMemPack(int count)
        {
            var data = MemoryPackSerializer.Serialize(new BenchExample());
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                var a = MemoryPackSerializer.Deserialize<BenchExample>(data);
            }
            stp.Stop();
            Debug.Log($"{nameof(ReadMemPack)} {count:N0} cycle, {stp.ElapsedMilliseconds} ms");
        }
        
        private void ReadTofu(int count)
        {
            BenchExample strudummy = new()
            {
                direction =  Vector2.down,
                speed = 10,
                aaaa = "aasdasdasd",
                asdasd = 0.5f,
                VecArr = new Vector2[10]
            };
            LwBinaryWriter writer = new LwBinaryWriter(512);
            writer.WriteRef(strudummy);
            
            LwBinaryReader reader = new LwBinaryReader(writer.ToPtr());
            
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                BenchExample stru = new ();
                reader.ReadRef(stru);
                reader.SetPosition(0);
            }
            stp.Stop();
            Debug.Log($"{nameof(ReadTofu)} {count:N0} cycle, {stp.ElapsedMilliseconds} ms");
            
            reader.Dispose();
            writer.Dispose();
        }
    }
}