using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MemoryPack;
using SheetData.Scripts.Parsing;
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
            NativeArray<int> a;
            NativeList<float> bb;
            NativeHashMap<char, float> bbs;
            
            testButton.onClick.AddListener(() =>
            {
                //ParserFormatter.
                var listType = typeof(List<int>);
                var list2Type = typeof(List<float>);
                var arrayType = typeof(int[]);
                var primaryType = typeof(int);
                var primaryType2 = typeof(string);
                var twogenericType = typeof(Dictionary<string, int>);
                var twogenericType2 = typeof(Dictionary<int, char>);
                ParserFormatter.RefreshParser();
                /*BenchMark m = new BenchMark();
                m.Start();*/
                SerializeTest t = new();
                t.Start();

            });
        }



        
    }
}