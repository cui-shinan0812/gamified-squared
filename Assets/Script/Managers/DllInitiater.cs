using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DllInitiater : MonoBehaviour
{
    [DllImport("libUnityPlugIn")]
    private static extern void init(int m, int n, int numPorts, int length, int controller_used, int[] portsDistribution, IntPtr configMap);

    private void Start()
    {
        JsonManager.Instance.ExtractData(out int M, out int N, out int NumOfPorts, out int MaxLength, out int ControllerUsed,
            out int[] PortsDistribution, out string[][] Distribution);
        Debug.Log("Starting init()");
        
        // hard coded config for testing
        // int M = 4;
        // int N = 4;
        // int NumOfPorts = 4;
        // int MaxLength = 6;
        // int ControllerUsed = 1;
        // int[] PortsDistribution = new int[1] { numOfPorts };
        // string[,] Distribution = new string[4, 4]
        // {
        //     { "A0", "B0", "C0", "D0" },
        //     { "A1", "B1", "C1", "D1" },
        //     { "A2", "B2", "C2", "D2" },
        //     { "A3", "B3", "C3", "D3" }
        // };
        
        // Create a 2D array of IntPtr to hold the string pointers
        IntPtr[] configMap = new IntPtr[M];
        for (int i = 0; i < M; ++i)
        {
            IntPtr[] row = new IntPtr[N];
            for (int j = 0; j < N; ++j)
            {
                row[j] = Marshal.StringToHGlobalAnsi(Distribution[i][j]);
            }
            configMap[i] = Marshal.UnsafeAddrOfPinnedArrayElement(row, 0);
        }
        // Allocate memory for the configMap pointer array
        IntPtr configMapPtr = Marshal.UnsafeAddrOfPinnedArrayElement(configMap, 0);
        // Call the init function
        init(M, N, NumOfPorts, MaxLength, ControllerUsed, PortsDistribution, configMapPtr);
        // Free the allocated memory
        for (int i = 0; i < M; ++i)
        {
            for (int j = 0; j < N; ++j)
            {
                Marshal.FreeHGlobal(Marshal.ReadIntPtr(configMap[i], j * IntPtr.Size));
            }
        }
    }
}
