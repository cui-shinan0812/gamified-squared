using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class InitTester : MonoBehaviour
{
    // Import the init function from the DLL
    [DllImport("libUnityPlugIn")]
    private static extern void init(int m, int n, int numPorts, int length, int controller_used, int[] portsDistribution, IntPtr configMap);
    void Start()
    {
        // // Define the input parameters
        // int M = 4;
        // int N = 4;
        // int numOfPorts = 4;
        // int maxLength = 6;
        // int[] portsDistribution = new int[1] { numOfPorts };
        // string[,] distribution = new string[4, 4]
        // {
        //     { "A0", "B0", "C0", "D0" },
        //     { "A1", "B1", "C1", "D1" },
        //     { "A2", "B2", "C2", "C5" },
        //     { "A3", "A4", "C3", "C4" }
        // };
        // // Create a 2D array of IntPtr to hold the string pointers
        // IntPtr[] configMap = new IntPtr[M];
        // for (int i = 0; i < M; ++i)
        // {
        //     IntPtr[] row = new IntPtr[N];
        //     for (int j = 0; j < N; ++j)
        //     {
        //         row[j] = Marshal.StringToHGlobalAnsi(distribution[i, j]);
        //     }
        //     configMap[i] = Marshal.UnsafeAddrOfPinnedArrayElement(row, 0);
        // }
        // // Allocate memory for the configMap pointer array
        // IntPtr configMapPtr = Marshal.UnsafeAddrOfPinnedArrayElement(configMap, 0);
        // // Call the init function
        // init(M, N, numOfPorts, maxLength, 0, portsDistribution, configMapPtr);
        // // Free the allocated memory
        // for (int i = 0; i < M; ++i)
        // {
        //     for (int j = 0; j < N; ++j)
        //     {
        //         Marshal.FreeHGlobal(Marshal.ReadIntPtr(configMap[i], j * IntPtr.Size));
        //     }
        // }
    }
}
