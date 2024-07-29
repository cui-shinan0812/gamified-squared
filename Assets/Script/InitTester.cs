using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class InitTester : MonoBehaviour
{
    [DllImport("UnityPlugIn", CallingConvention = CallingConvention.Cdecl)]
    private static extern void init(int m, int n, int numPorts, int length, int controller_used, IntPtr portsDistribution, IntPtr configMap);

    // Example test values
    private int[] portsDistribution = new int[] { 1, 2, 3, 4 };
    private int[][] configMap = new int[][] {
        new int[] { 1, 0 },
        new int[] { 0, 1 }
    };

    void Start()
    {
        // TestInitMethod();
    }

    private void TestInitMethod()
    {
        int m = configMap.Length;
        int n = configMap[0].Length; // Assuming all rows are of the same length
        int numPorts = portsDistribution.Length;
        int length = 10; // Example value, set to your actual 'length'
        int controller_used = 1; // Example value, set to your actual 'controller_used'

        // Marshal the portsDistribution array to unmanaged memory
        IntPtr portsDistributionPtr = Marshal.AllocHGlobal(portsDistribution.Length * sizeof(int));
        Marshal.Copy(portsDistribution, 0, portsDistributionPtr, portsDistribution.Length);

        // Marshal the configMap jagged array to unmanaged memory
        IntPtr configMapPtr = Marshal.AllocHGlobal(m * IntPtr.Size);
        for (int i = 0; i < m; i++)
        {
            IntPtr rowPtr = Marshal.AllocHGlobal(n * sizeof(int));
            Marshal.Copy(configMap[i], 0, rowPtr, n);
            Marshal.WriteIntPtr(configMapPtr, i * IntPtr.Size, rowPtr);
        }

        // Call the init function from the DLL
        init(m, n, numPorts, length, controller_used, portsDistributionPtr, configMapPtr);

        // Clean up unmanaged memory
        for (int i = 0; i < m; i++)
        {
            IntPtr rowPtr = Marshal.ReadIntPtr(configMapPtr, i * IntPtr.Size);
            Marshal.FreeHGlobal(rowPtr);
        }
        Marshal.FreeHGlobal(configMapPtr);
        Marshal.FreeHGlobal(portsDistributionPtr);
    }
}
