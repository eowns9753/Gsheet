using System;
using MemoryPack;
using SheetData.Generator;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestBehaviour : MonoBehaviour
    {
        private void Start()
        {
            var a = new ExampleClass();
            var b = new ExampleSturct();
            a.time = 1.2232f;
            b.direction = new Vector2(2, 3);

            var a_bin = MemoryPackSerializer.Serialize(a);
            var b_bin = MemoryPackSerializer.Serialize(b);
        }
    }
}