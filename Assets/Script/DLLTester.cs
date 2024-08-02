using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DLLTester : MonoBehaviour
{
    [DllImport("libUnityPlugIn")]
    private static extern void init(int m, int n, int numPorts, int length, int controller_used, int[] portsDistribution, IntPtr configMap);

    [DllImport("libUnityPlugIn")]
    private static extern void destroy();

    [DllImport("libUnityPlugIn")]
    private static extern void displayFrameUnity(IntPtr frame);
    
    [DllImport("libUnityPlugIn", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr getSensors();

    [SerializeField] private Button TestInitButton;
    [SerializeField] private Button TestDestroyButton;
    [SerializeField] private Button TestDisplayFrameButton;
    [SerializeField] private Button TestGetSensorsButton;

    private int m = 4;
    private int n = 4;
    private int numPorts = 4;
    private int length = 6;
    private int controller_used = 1;
    // private int[] portsDistribution = new int[] { numPorts };
    // private int[][] configMap = new int[2][]
    // {
    //     new int[] { 0, 1 },
    //     new int[] { 2, 4 }
    // };

    private void Awake()
    {
        ///*
        TestInitButton.onClick.AddListener(() =>
        {
            Debug.Log("Pressed init()");
            // Define the input parameters
            int M = 4;
            int N = 4;
            int numOfPorts = 4;
            int maxLength = 6;
            int[] portsDistribution = new int[1] { numOfPorts };
            string[,] distribution = new string[4, 4]
            {
                { "A0", "B0", "C0", "D0" },
                { "A1", "B1", "C1", "D1" },
                { "A2", "B2", "C2", "C5" },
                { "A3", "A4", "C3", "C4" }
            };
            // Create a 2D array of IntPtr to hold the string pointers
            IntPtr[] configMap = new IntPtr[M];
            for (int i = 0; i < M; ++i)
            {
                IntPtr[] row = new IntPtr[N];
                for (int j = 0; j < N; ++j)
                {
                    row[j] = Marshal.StringToHGlobalAnsi(distribution[i, j]);
                }
                configMap[i] = Marshal.UnsafeAddrOfPinnedArrayElement(row, 0);
            }
            // Allocate memory for the configMap pointer array
            IntPtr configMapPtr = Marshal.UnsafeAddrOfPinnedArrayElement(configMap, 0);
            // Call the init function
            init(M, N, numOfPorts, maxLength, 0, portsDistribution, configMapPtr);
            // Free the allocated memory
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    Marshal.FreeHGlobal(Marshal.ReadIntPtr(configMap[i], j * IntPtr.Size));
                }
            }
        });
        
        TestDestroyButton.onClick.AddListener(() =>
        {
            Debug.Log("Pressed destroy()");
            destroy();
        });
        
        //*/
        
        TestDisplayFrameButton.onClick.AddListener(() =>
        {
            /*
            int[][] frameArray = new int[4][]
            {
                new int[] { 0, 1, 2, 3 },
                new int[] { 0, 1, 2, 3 },
                new int[] { 0, 1, 2, 3 },
                new int[] { 0, 1, 2, 3 },
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
            
            */
            
            
            // int[,] frame = new int[4, 4]
            // {
            //     { 0, 1, 2, 3},
            //     { 3, 2, 1, 0 },
            //     { 2, 1, 3, 0 },
            //     { 3, 1, 2, 0 }
            // };
            //
            //
            // // Create a 2D array of IntPtr to hold the integer pointers
            // IntPtr[] frameptr = new IntPtr[m];
            // for (int i = 0; i < m; ++i)
            // {
            //     IntPtr[] row = new IntPtr[n];
            //     for (int j = 0; j < n; ++j)
            //     {
            //         // Allocate memory for the integer
            //         row[j] = Marshal.AllocHGlobal(sizeof(int));
            //
            //         // Copy the integer to the allocated memory
            //         Marshal.WriteInt32(row[j], frame[i, j]);
            //     }
            //     frameptr[i] = Marshal.UnsafeAddrOfPinnedArrayElement(row, 0);
            // }
            //
            // // Assuming configMap is defined somewhere else
            // IntPtr frameptrModified = Marshal.UnsafeAddrOfPinnedArrayElement(frameptr, 0);
            //
            //
            // // Call the displayFrame method
            // // displayFrameUnity(frameptrModified);
            //
            // // Free the unmanaged memory
            // // Marshal.FreeHGlobal(frameptrModified);
            
            int[,] frame = new int[4, 4]
            {
                { 0, 1, 2, 3},
                { 3, 2, 1, 0 },
                { 2, 1, 3, 0 },
                { 3, 1, 2, 0 }
            };

            // Allocate unmanaged memory for the 2D array
            IntPtr[] rows = new IntPtr[m];
            for (int i = 0; i < m; ++i)
            {
                // Create a 1D array for the current row
                int[] row = new int[n];
                for (int j = 0; j < n; ++j)
                {
                    row[j] = frame[i, j];
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
        });

        
        // TestGetSensorsButton.onClick.AddListener(() =>
        // {
        //     // Call the getSensors method
        //     IntPtr sensorsPtr = getSensors();
        //
        //     // Assuming the size of the matrix is known (e.g., 2x2 for this example)
        //     // Convert the IntPtr to a managed bool array
        //     bool[][] sensorsArray = new bool[m][];
        //     for (int i = 0; i < m; i++)
        //     {
        //         sensorsArray[i] = new bool[n];
        //         for (int j = 0; j < n; j++)
        //         {
        //             // Calculate the pointer offset and read the byte value
        //             IntPtr bytePtr = IntPtr.Add(sensorsPtr, (i * n + j) * Marshal.SizeOf(typeof(byte)));
        //             byte byteValue = Marshal.PtrToStructure<byte>(bytePtr);
        //
        //             // Convert the byte value to a bool
        //             sensorsArray[i][j] = byteValue != 0;
        //         }
        //     }
        //
        //
        //     // Free the unmanaged memory if necessary
        //     // (Assuming the unmanaged memory needs to be freed, otherwise skip this step)
        //     for (int i = 0; i < m; i++)
        //     {
        //         IntPtr rowPtr = Marshal.ReadIntPtr(sensorsPtr, i * IntPtr.Size);
        //         Marshal.FreeHGlobal(rowPtr);
        //     }
        //     Marshal.FreeHGlobal(sensorsPtr);
        //
        //     // Display the sensors array for testing purposes
        //     Debug.Log("Dimension: " + sensorsArray.Length + " * " + sensorsArray[0].Length);
        //     for (int i = 0; i < m; i++)
        //     {
        //         for (int j = 0; j < n; j++)
        //         {
        //             Debug.Log("sensorsArray: " + sensorsArray[i][j] + " ");
        //         }
        //     }
        //
        // });
        TestGetSensorsButton.onClick.AddListener(() =>
        {
            // IntPtr sensorsPtr = getSensors();
            // bool[,] sensors = new bool[4, 4]; // You need to know the number of rows and columns
            //
            // for (int i = 0; i < 4; i++)
            // {
            //     for (int j = 0; j < 4; j++)
            //     {
            //         // Calculate the offset for each element in the 2D array
            //         int offset = (i * 4 + j) * sizeof(bool);
            //         sensors[i, j] = Marshal.ReadByte(sensorsPtr, offset) != 0; // Read each byte and convert it to bool
            //         Debug.Log("sensors["+i+", "+j+"]: " + Marshal.ReadByte(sensorsPtr, offset));
            //     }
            // }
            
            const int rows = 4;
            const int cols = 4;
            // Call the C++ function to get the dynamic integer array
            IntPtr ptr = getSensors();
            // Marshal the returned pointer to a 2D integer array
            int[][] result = new int[rows][];
            for (int i = 0; i < rows; i++) {
                result[i] = new int[cols];
                Marshal.Copy(Marshal.ReadIntPtr(ptr, i * IntPtr.Size), result[i], 0, cols);
            }
            
            // Interpret integers as booleans
            bool[][] boolResult = new bool[rows][];
            for (int i = 0; i < rows; i++) {
                boolResult[i] = new bool[cols];
                for (int j = 0; j < cols; j++) 
                {
                    boolResult[i][j] = result[i][j] != 0; // Convert non-zero values to true
                }
            }
            // Output the boolean values
            for (int y = 0; y < boolResult.Length; y++)
            {
                for (int x = 0; x < boolResult[0].Length; x++)
                {
                    Debug.Log("boolResult[" + y + "][" + x + "]: " + boolResult[y][x]);
                }
            }
            // Clean up the memory allocated in C++
            for (int i = 0; i < rows; i++) {
                Marshal.FreeCoTaskMem(Marshal.ReadIntPtr(ptr, i * IntPtr.Size));
            }
            Marshal.FreeCoTaskMem(ptr);
            
        });
        
        

    }
    
    public static bool[,] GetSensors()
    {
        IntPtr sensorsPtr = getSensors();
        int M = 4; // Replace with actual value
        int N = 4; // Replace with actual value
        bool[,] sensors = new bool[M, N];
        for (int i = 0; i < M; i++)
        {
            IntPtr rowPtr = Marshal.ReadIntPtr(sensorsPtr, i * IntPtr.Size);
            for (int j = 0; j < N; j++)
            {
                sensors[i, j] = Marshal.ReadByte(rowPtr, j) != 0;
                Debug.Log("sensors["+i+", "+j+"]: " + sensors[i, j]);
            }
        }
        // Free the unmanaged memory
        // freeSensors(sensorsPtr, M);
        return sensors;
    }
    
}
