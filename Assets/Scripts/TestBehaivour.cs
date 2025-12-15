using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MemoryPack;
using Rui.IO.Serialization;
using SheetData.Generator;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class TestBehaviour : MonoBehaviour
    {
        private Stopwatch stp = new Stopwatch();
        private void Start()
        {
            /*NativeBinaryWriter writer = new NativeBinaryWriter(100, Allocator.Persistent);
            
            var a = new ExampleClass();
            var b = new ExampleSturct();

            writer.Write(a.localizeName);
            writer.Write(a.time);
            writer.Write(a.property);
            writer.Write(a.arr);
            writer.Write(a.properties);
            
            
            var a_mem = MemoryPackSerializer.Serialize(a).Length;
            var a_tofu = writer.Length;
            writer.Dispose();
            writer = new NativeBinaryWriter(100, Allocator.Persistent);
            
            writer.Write(b.direction);
            writer.Write(b.speed);
            writer.Write(b.nativeArr);
            writer.Write(b.testfxStr);
            //writer.Write(b.a);
            var b_mem = MemoryPackSerializer.Serialize(b);
            var b_tofu = writer.Length;*/

            
        }


        private void Update()
        {
            
            if(Keyboard.current.aKey.wasPressedThisFrame)
                Test();
        }

        async Task Test()
        {
            int cycle = 1000000;

            int capacity = cycle * (Marshal.SizeOf<ExampleSturct>() + 8);
            var _memoryPackWriter = new ArrayBufferWriter<byte>(capacity); // 1MB 초기 용량
            var _nativeBuffer = new NativeBinaryWriter(capacity, Allocator.Persistent);
            
            await Task.Delay(1000);
            WriteTofuMem(cycle, _nativeBuffer, new ExampleSturct());
            GC.Collect();
            await Task.Delay(1000);
            WriteMemPack(cycle, _memoryPackWriter, new ExampleSturct());
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

        private void WriteTofuMem(int count, NativeBinaryWriter writer, ExampleSturct b)
        {
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                writer.Write(b.direction);
                writer.Write(b.speed);
                writer.Write(b.nativeArr);
                writer.Write(b.testfxStr);
                writer.Write(b.testfxStr2);
                writer.Write(b.asdasd);
                writer.Write(b.aaaa);
            }
            stp.Stop();
            Debug.Log($"{nameof(WriteTofuMem)} {count:N0} cycle({writer.Length/1000000} MB), {stp.ElapsedMilliseconds} ms");
        }

        private void WriteMemPack(int count, ArrayBufferWriter<byte> arr, ExampleSturct b)
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
            var data = MemoryPackSerializer.Serialize(new ExampleSturct());
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                var a = MemoryPackSerializer.Deserialize<ExampleSturct>(data);
            }
            stp.Stop();
            Debug.Log($"{nameof(ReadMemPack)} {count:N0} cycle, {stp.ElapsedMilliseconds} ms");
        }
        
        private void ReadTofu(int count)
        {
            ExampleSturct strudummy = new ();
            NativeBinaryWriter writer = new NativeBinaryWriter(512, Allocator.Persistent);
            writer.Write(strudummy.direction);
            writer.Write(strudummy.speed);
            writer.Write(strudummy.nativeArr);
            writer.Write(strudummy.testfxStr);
            writer.Write(strudummy.testfxStr2);
            writer.Write(strudummy.asdasd);
            writer.Write(strudummy.aaaa);
            NativeBinaryReader reader = new NativeBinaryReader(writer, Allocator.Persistent);
            
            stp.Restart();
            for (int i = 0; i < count; i++)
            {
                ExampleSturct stru = new ();
                stru.direction = reader.Read<Vector2>();
                stru.speed = reader.Read<float>();
                stru.nativeArr = reader.Read<NativeList<int>>();
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