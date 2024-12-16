using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class VirtualReceiver : MonoBehaviour
{
    public static VirtualReceiver Instance;
    
    private int[][] virtualTiles;
    private bool[][] steppedTiles;

    public event EventHandler OnSteppingChanged;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        virtualTiles = new[]
        {
            new []{4,4}, 
            new []{4,4}
        };
            
        steppedTiles = new[]
        {
            new []{false,false}, 
            new []{false,false}
        };
        // virtualTiles = Array.Empty<Color32[]>();
        // steppedTiles = Array.Empty<bool[]>();

    }

    void Start()
    {
        // GameplayManager.Instance.OnStateChanged += GameplayManager_OnStateChanged;
    }

    // private void GameplayManager_OnStateChanged(object sender, EventArgs e)
    // {
    //     if (GameplayManager.Instance.IsGamePlay())
    //     {
    //         RandomStepping();
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.Instance.IsGamePlay())
        {
            //invoke something to tell UI to give array
        }
    }

    public void Init()
    {
        
    }
    
    // public bool init(string ConfigType, callback: CallbackFunction) {
    //     
    // }
    public void displayFrame(int[][] frame)
    {
        if (frame.Length > 0)
        {
            virtualTiles = frame;
            int height = frame.Length;
            int width = frame[0].Length;
            for (int y = 0; y < height; y++)
            {
                Debug.Log("Display: " + y + ". row: " );
                for (int x = 0; x < width; x++)
                {
                    Debug.Log("Display: " + frame[y][x]);
                }
            }
            RandomStepping();
        }
    }

    public void setColor(Color32[] clr, int x, int y)
    {
        Debug.Log("send: Color32[] clr[" + x + "][" + y + "]");
    }

    public void RandomStepping()
    {
        steppedTiles[Random.Range(0, steppedTiles.Length)][Random.Range(0, steppedTiles[0].Length)] = true;
        OnSteppingChanged?.Invoke(this, EventArgs.Empty);
    }

    private void InitialSteppedTiles()
    {
        Debug.Log(steppedTiles.Length);
        if (steppedTiles.Length > 0)
        {
            for (int y = 0; y < steppedTiles.Length; y++)
            {
                for (int x = 0; x < steppedTiles[0].Length; x++)
                {
                    steppedTiles[y][x] = false;
                }
            }
        }
    }

    public bool[][] GetSteppedTiles()
    {
        return steppedTiles;
    }

    public void DoSomething()
    {
        Debug.Log("Something is done.");
    }
    
}
