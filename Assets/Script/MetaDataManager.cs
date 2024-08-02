using System;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MetaDataManager : MonoBehaviour
{
    public static MetaDataManager Instance;
    [SerializeField] private Button testReadMetaDataButton;
    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        // testReadMetaDataButton.onClick.AddListener(() =>
        // {
        //     ExtractData();
        // });
    }

    public class Root
    {
        public List<List<List<int>>> map { get; set; }

        [JsonProperty("meta-data")]
        public MetaData metaData { get; set; }
    }

    public class MetaData
    {
        public string category { get; set; }
        public string difficulty { get; set; } // Changed from int to string to match the JSON
        public string name { get; set; }
    }

    public void ExtractData(out int[][][] map, out string name, out string category, out int difficulty)
    {
        map = new int[][][] { };
        name = null;
        category = null;
        difficulty = 0;
        // Open file browser
        string path = EditorUtility.OpenFilePanel("Select JSON file", "", "json");
        if (path.Length != 0)
        {
            // Read the JSON file
            string json = File.ReadAllText(path);

            Root root = JsonConvert.DeserializeObject<Root>(json);

            // Convert List<List<List<int>>> to int[][][]
            map = root.map.Select(a => a.Select(b => b.ToArray()).ToArray()).ToArray();

            category = root.metaData.category;
            difficulty = Int32.Parse(root.metaData.difficulty);
            name = root.metaData.name;

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Name is empty in the JSON file");
                return;
            }
            Debug.Log("name: " + name);
            Debug.Log("category: " + category);
            Debug.Log("difficulty: " + difficulty);

            for (int frame = 0; frame < map.Length; frame++)
            {
                Debug.Log("Frame: " + frame);
                for (int y = 0; y < map[frame].Length; y++)
                {
                    for (int x = 0; x < map[frame][y].Length; x++)
                    {
                        Debug.Log(map[frame][y][x]);
                    }
                }
            }
        }
        
    }

}
