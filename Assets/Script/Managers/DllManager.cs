using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// DllManager is responsible on calling other functions from the dll
public class DllManager : MonoBehaviour
{
    public static DllManager Instance { get; private set; }
    [DllImport("libUnityPlugIn")]
    private static extern void displayFrameUnity(IntPtr frame);

    private void Start()
    {
        Instance = this;
    }

    // DisplayFrame() is responsible to pass the 2D array to dll function displayFrameUnity()
    public void DisplayFrame(int[][] map, int m, int n)
    {

            // Allocate unmanaged memory for the 2D array
            IntPtr[] rows = new IntPtr[m];
            for (int i = 0; i < m; ++i)
            {
                // Create a 1D array for the current row
                int[] row = new int[n];
                for (int j = 0; j < n; ++j)
                {
                    row[j] = map[i][j];
                }

                // Allocate unmanaged memory for the row and copy the row data
                rows[i] = Marshal.AllocHGlobal(n * sizeof(int));
                Marshal.Copy(row, 0, rows[i], n);
            }

            IntPtr framePtr = Marshal.AllocHGlobal(m * IntPtr.Size);
            Marshal.Copy(rows, 0, framePtr, m);

            // Call the C++ function
            displayFrameUnity(framePtr);
            
            // Free the unmanaged memory
            for (int i = 0; i < m; ++i)
            {
                Marshal.FreeHGlobal(rows[i]);
            }
            Marshal.FreeHGlobal(framePtr);
    }
}
