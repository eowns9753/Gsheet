using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading.Tasks;
using MemoryPack;
using Rui.IO.Serialization;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    [MemoryPackable]
    public partial class BenchExample
    {
        public Vector2 direction { get; set; }
        public float speed { get; set; }
        public FixedString32Bytes testfxStr { get; set; }
        public FixedString32Bytes testfxStr2 { get; set; }
        public float asdasd { get; set; }

        public string aaaa { get; set; }

        public Vector2[] VecArr { get; set; } = new Vector2[10];
        //public StructNameTest a { get; private set; }

    }
    
    public class BenchMark
    {
        private Stopwatch stp = new Stopwatch();
        public async Task Start()
        {
            Debug.Log("Test start");
            int cycle = 300000;

            var _memoryPackWriter = new ArrayBufferWriter<byte>(64); // 1MB 초기 용량
            var _nativeBuffer = new NativeBinaryWriter(64);
            
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

        private void WriteTofuMem(int count, NativeBinaryWriter writer, BenchExample b)
        {
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                writer.Write(b.direction, b.speed, b.testfxStr, b.testfxStr2, b.asdasd);
                writer.Write(b.aaaa);
                writer.Write(b.VecArr);
            }

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
            BenchExample strudummy = new ();
            NativeBinaryWriter writer = new NativeBinaryWriter(512);
            writer.Write(strudummy.direction);
            writer.Write(strudummy.speed);
            writer.Write(strudummy.testfxStr);
            writer.Write(strudummy.testfxStr2);
            writer.Write(strudummy.asdasd);
            writer.Write(strudummy.aaaa);
            NativeBinaryReader reader = new NativeBinaryReader(writer, Allocator.Persistent);
            
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                BenchExample stru = new ();
                stru.direction = reader.Read<Vector2>();
                stru.speed = reader.Read<float>();
                stru.testfxStr = reader.Read<FixedString32Bytes>();
                stru.testfxStr2 = reader.Read<FixedString32Bytes>();
                stru.asdasd = reader.Read<float>();
                stru.aaaa = reader.ReadString();
                reader.SetPosition(0);
            }
            stp.Stop();
            Debug.Log($"{nameof(ReadTofu)} {count:N0} cycle, {stp.ElapsedMilliseconds} ms");
            
            reader.Dispose();
            writer.Dispose();
        }
    }
}