using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is for clearer logic not really needed
public class LoaderCallback : MonoBehaviour {
    private bool isFirstUpdate = true;

    private void Update() {
        if (isFirstUpdate) {
            isFirstUpdate = false;

            Loader.LoaderCallback();
        }
    }
}