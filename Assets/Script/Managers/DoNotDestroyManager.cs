using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To help perform seamless music plays cross scene
public class DoNotDestroyManager : MonoBehaviour
{
    private const string BGMUSIC = "BgMusic";
    // Start is called before the first frame update
    private void Awake()
    {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag(BGMUSIC);
        if (musicObj.Length > 1)
        {
            // Destroy it when more than 1 game object is loaded
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
