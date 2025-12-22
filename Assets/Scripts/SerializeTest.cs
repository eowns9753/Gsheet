using System;
using Rui.IO.Serialization;
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
    
    public class TestStruct : INativeBinaryable
    {
        public TestStructChild child; //26
        public Vector2 vec; //34
        public int integer; //38
        public float fl; //42
        public long lon; //50
        public decimal deci; //58
        public float3 f3; //70

        void INativeBinaryable.OnNativeWrite(NativeBinaryWriter writer)
        {
            writer.Write(child, vec, integer, fl, lon, deci, f3);
        }

        void INativeBinaryable.OnNativeRead(NativeBinaryReader reader)
        {
            reader.Read(out child, out vec, out integer, out fl, out lon, out deci, out f3);
        }
    }
    
    public class SerializeTest
    {
        public void Start()
        {
            var datas = Write(5);
            Read(datas);
           
        }

        public void Read(byte[] datas)
        {
            NativeBinaryReader reader = new NativeBinaryReader(datas);
            reader.Read(out int count);
            for (int i = 0; i < count; i++)
            {
                TestStruct nStruct = new TestStruct();
                reader.ReadRef(nStruct);
            }
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

            NativeBinaryWriter writer = new NativeBinaryWriter(128);
            writer.Write(count);
            for (int i = 0; i < structs.Length; i++)
                writer.WriteRef(structs[i]);
            var result = writer.ToArray();
            writer.Dispose();
            return result;
            //writer
        }
        
    }
}