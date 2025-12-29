using System;
using LWBinarySerializer;
using LWSerializer;
using SheetData.Editor.Parsing;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public struct TestStructChild
    {
        public int i1;//4
        public int i2;//8
        public float fl;//12
        public long lon;//18
        public decimal deci;//26

        public TestStructChild(int i1, int i2, float fl, long lon, decimal deci)
        {
            this.i1 = i1;
            this.i2 = i2;
            this.fl = fl;
            this.lon = lon;
            this.deci = deci;
        }
    }
    
    public class TestStruct : ILwSerializable
    {
        public TestStructChild child; //26
        public Vector2 vec; //34
        public int integer; //38
        public float fl; //42
        public long lon; //50
        public decimal deci; //58
        public float3 f3; //70

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(child, vec, integer, fl, lon, deci, f3);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out child, out vec, out integer, out fl, out lon, out deci, out f3);
        }
    }
    
    public class SerializeTest
    {
        public void Start()
        {
            /*var datas = Write(5);
            Read(datas);
            var unmanaArr = Example_Unmanaged.Write(new Example_Unmanaged.ExampleStruct()
            {
                m_bool = false,
                m_float = 2.5f,
                m_int = 123123
            });
            var unmana = Example_Unmanaged.Read(unmanaArr);
            var managedArr = Example_Managed.Write(new Example_Managed.ExampleClass()
                { m_float = 5.5f, m_arr = new string[2] { "bbbbb", "ccccc" } });
            var mana = Example_Managed.Read(managedArr);
            
            var bytes = LwUtility.To("hello world");
            var data = LwUtility.From<string>(bytes);
            var hash = LwUtility.ToXxHash64(bytes);

            NativeArray<float> floatArr = new NativeArray<float>(new []{0.2f,1.2f}, Allocator.Temp);
            var arr  = LwUtility.To(floatArr);*/
            
        }

        public void Read(byte[] datas)
        {
            LwBinaryReader reader = new LwBinaryReader(datas);
            reader.Read(out int count);
            for (int i = 0; i < count; i++)
            {
                TestStruct nStruct;
                reader.Read(out nStruct);
            }
            Debug.Log($"Hash - {reader.GetXxHash64(15)}");
            reader.Dispose();
        }

        public byte[] Write(int count)
        {
            TestStruct[] structs = new TestStruct[count];
            for (int i = 0; i < structs.Length; i++)
            {
                structs[i] = new TestStruct()
                {
                    child = new TestStructChild(i, i, i*i, 1000, Decimal.MaxValue),
                    vec = new Vector2(1000, 1000),
                    integer = i,
                    fl = i,
                    lon = i,
                };
            }

            LwBinaryWriter writer = new LwBinaryWriter(128);
            writer.Write(count);
            for (int i = 0; i < structs.Length; i++)
                writer.Write(structs[i]);
            var result = writer.ToArray();
            Debug.Log($"Hash - {writer.GetXxHash64(15)}");
            writer.Dispose();
            return result;
            //writer
        }
        
    }
}