using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MemoryPack;
using Rui.IO.Serialization;
using SheetData.Generator;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class TestBehaviour : MonoBehaviour
    {
        public Button testButton;
        private Stopwatch stp = new Stopwatch();
        private void Start()
        {
            testButton.onClick.AddListener(() =>
            {
                /*BenchMark m = new BenchMark();
                m.Start();*/
                SerializeTest t = new();
                t.Start();

            });
        }



        
    }
}