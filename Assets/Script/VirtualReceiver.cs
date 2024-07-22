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
    public event EventHandler OnSteppingChanged;
    private Color32[,] virtualTiles;
    private bool[,] steppedTiles;
    
    
    // Start is called before the first frame update
    private void Awake()
    {
        // virtualTiles = Array.Empty<Color32[]>();
        // steppedTiles = Array.Empty<bool[]>();
        
    }

    void Start()
    {
        GameplayManager.Instance.OnStateChanged += GameplayManager_OnStateChanged;
    }

    private void GameplayManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameplayManager.Instance.IsGamePlay())
        {
            RandomStepping();
        }
    }

    // U    pdate is called once per frame
    void Update()
    {
        if (GameplayManager.Instance.IsGamePlay())
        {
            //invake something to tell UI to give array
        }
    }
    
    // public bool init(string ConfigType, callback: CallbackFunction) {
    //     
    // }
    public void displayFrame(Color32[,] frame)
    {
        if (frame.Length > 0)
        {
            virtualTiles = frame;
            int height = frame.Length;
            // int width = frame[0,0].Length;
            // int width = 
            for (int y = 0; y < height; y++)
            {
                Debug.Log(y + ". row: " );
                // for (int x = 0; x < width; x++)
                // {
                //     Debug.Log(frame[y][x]);
                // }
            }   
        }
    }

    public void setColor(Color32[] clr, int x, int y)
    {
        Debug.Log("send: Color32[] clr[" + x + "][" + y + "]");
    }

    public void RandomStepping()
    {
        // steppedTiles[Random.Range(0, steppedTiles.Length),Random.Range(0, steppedTiles[0].Length)] = true;
        OnSteppingChanged?.Invoke(this, EventArgs.Empty);
    }

    private void InitialSteppedTiles()
    {
        if (steppedTiles.Length > 0)
        {
            // if (steppedTiles[0].Length > 0)
            // {
            //     for (int y = 0; y < steppedTiles.Length; y++)
            //     {
            //         for (int x = 0; x < steppedTiles[y].Length; x++)
            //         {
            //             steppedTiles[y][x] = false;
            //         }
            //     }
            // }
        }
    }

    public bool[,] GetSteppedTiles()
    {
        return steppedTiles;
    }

    public void DoSomething()
    {
        Debug.Log("Something is done.");
    }
    
}
