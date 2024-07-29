using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DLLTester : MonoBehaviour
{
    [DllImport("UnityPlugIn")]
    private static extern void init(int m, int n, int numPorts, int length, int controller_used, IntPtr portsDistribution, IntPtr configMap);

    [DllImport("UnityPlugIn")]
    private static extern void destroy();

    [DllImport("UnityPlugIn")]
    private static extern void displayFrame(IntPtr frame);

    [DllImport("UnityPlugIn")]
    private static extern IntPtr getSensors();

    [SerializeField] private Button TestInitButton;
    [SerializeField] private Button TestDestroyButton;
    [SerializeField] private Button TestDisplayFrameButton;
    [SerializeField] private Button TestGetSensorsButton;

    private int m = 2;
    private int n = 2;
    private int numPorts = 2;
    private int length = 4;
    private int controller_used = 1;
    private int[] portsDistribution = new int[] { 1, 2 };
    private int[][] configMap = new int[2][]
    {
        new int[] { 0, 1 },
        new int[] { 2, 4 }
    };

    private void Awake()
    {
        TestInitButton.onClick.AddListener(() =>
        {
            Debug.Log("Pressed init()");
            // Convert portsDistribution array to unmanaged memory
            IntPtr portsDistributionPtr = Marshal.AllocHGlobal(portsDistribution.Length * sizeof(int));
            Marshal.Copy(portsDistribution, 0, portsDistributionPtr, portsDistribution.Length);
        
            // Convert configMap array to unmanaged memory
            IntPtr configMapPtr = Marshal.AllocHGlobal(m * IntPtr.Size);
            for (int i = 0; i < m; i++)
            {
                IntPtr rowPtr = Marshal.AllocHGlobal(n * sizeof(int));
                Marshal.Copy(configMap[i], 0, rowPtr, n);
                Marshal.WriteIntPtr(configMapPtr, i * IntPtr.Size, rowPtr);
            }
        
            // Call init function
            init(m, n, numPorts, length, controller_used, portsDistributionPtr, configMapPtr);
        
            // Free unmanaged memory
            for (int i = 0; i < m; i++)
            {
                IntPtr rowPtr = Marshal.ReadIntPtr(configMapPtr, i * IntPtr.Size);
                Marshal.FreeHGlobal(rowPtr);
            }
            Marshal.FreeHGlobal(configMapPtr);
            Marshal.FreeHGlobal(portsDistributionPtr);
        });
        
        TestDestroyButton.onClick.AddListener(() =>
        {
            Debug.Log("Pressed destroy()");
            destroy();
        });
        
        TestDisplayFrameButton.onClick.AddListener(() =>
        {
            int[][] frameArray = new int[2][]
            {
                new int[] { 0, 1 },
                new int[] { 2, 4 }
            };

            // Flatten the jagged array to a 1D array
            int totalLength = 0;
            foreach (var row in frameArray)
            {
                totalLength += row.Length;
            }

            int[] flattenedArray = new int[totalLength];
            int index = 0;
            foreach (var row in frameArray)
            {
                foreach (var item in row)
                {
                    flattenedArray[index++] = item;
                }
            }

            // Allocate unmanaged memory and copy the data
            IntPtr framePtr = Marshal.AllocHGlobal(flattenedArray.Length * sizeof(int));
            Marshal.Copy(flattenedArray, 0, framePtr, flattenedArray.Length);

            // Call the displayFrame method
            displayFrame(framePtr);

            // Free the unmanaged memory
            Marshal.FreeHGlobal(framePtr);
        });

        
        TestGetSensorsButton.onClick.AddListener(() =>
        {
            // Call the getSensors method
            IntPtr sensorsPtr = getSensors();

            // Assuming the size of the matrix is known (e.g., 2x2 for this example)
            int rows = 2;
            int cols = 2;

            // Convert the IntPtr to a managed bool array
            bool[][] sensorsArray = new bool[rows][];
            for (int i = 0; i < rows; i++)
            {
                sensorsArray[i] = new bool[cols];
                IntPtr rowPtr = Marshal.ReadIntPtr(sensorsPtr, i * IntPtr.Size);
                for (int j = 0; j < cols; j++)
                {
                    sensorsArray[i][j] = Marshal.ReadByte(rowPtr, j) != 0;
                }
            }

            // Free the unmanaged memory if necessary
            // (Assuming the unmanaged memory needs to be freed, otherwise skip this step)
            for (int i = 0; i < rows; i++)
            {
                IntPtr rowPtr = Marshal.ReadIntPtr(sensorsPtr, i * IntPtr.Size);
                Marshal.FreeHGlobal(rowPtr);
            }
            Marshal.FreeHGlobal(sensorsPtr);

            // Display the sensors array for testing purposes
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(sensorsArray[i][j] + " ");
                }
                Console.WriteLine();
            }
        });

    }
    
}
