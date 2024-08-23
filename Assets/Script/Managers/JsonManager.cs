using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

// JsonManager responsible on Json file handling
public class JsonManager : MonoBehaviour
{
    public static JsonManager Instance { get; private set; }
    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        // testReadMetaDataButton.onClick.AddListener(() =>
        // {
        //     ExtractData();
        // });
    }

    public class Config
    {
        public List<List<List<int>>> map { get; set; }

        public string M { get; set; }
        public string N { get; set; }
        public string NumOfPorts { get; set; }
        public string MaxLength { get; set; }
        public string ControllerUsed { get; set; }
        public List<string> PortsDistribution { get; set; }
        public List<List<string>> Distribution { get; set; }
    }

    public void ExtractData(out int M, out int N, out int NumOfPorts, out int MaxLength, out int ControllerUsed,
        out int[] PortsDistribution, out string[][] Distribution)
    {
        M = 0;
        N = 0;
        NumOfPorts = 0;
        MaxLength = 0;
        ControllerUsed = 0;
        PortsDistribution = Array.Empty<int>();
        Distribution = Array.Empty<string[]>();
        // Open file browser
        string path = EditorUtility.OpenFilePanel("Select JSON file", "", "json");
        if (path.Length != 0)
        {
            // Read the JSON file
            string json = File.ReadAllText(path);

            Config config = JsonConvert.DeserializeObject<Config>(json);

            M = Int32.Parse(config.M);
            N = Int32.Parse(config.N);
            NumOfPorts = Int32.Parse(config.NumOfPorts);
            MaxLength = Int32.Parse(config.MaxLength);
            ControllerUsed = Int32.Parse(config.ControllerUsed);
            
            PortsDistribution = config.PortsDistribution.Select(int.Parse).ToArray();
            Distribution = config.Distribution.Select(list => list.ToArray()).ToArray();
        }
        
    }
}
