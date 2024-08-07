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
    
    // Import the receiveBroadcastSignal function from the DLL
    [DllImport("libUnityPlugIn")]
    private static extern IntPtr receiveBroadcastSignal(int BROADCAST_PORT, int BUFFER_SIZE);

    [SerializeField] private Button TestInitButton;
    [SerializeField] private Button TestDestroyButton;
    [SerializeField] private Button TestDisplayFrameButton;
    [SerializeField] private Button TestGetSensorsButton;

    private int m = 4;
    private int n = 4;
    // private int numPorts = 4;
    // private int length = 6;
    // private int controller_used = 1;

    private bool isGetSensors;
    
    // private int[] portsDistribution = new int[] { numPorts };
    // private int[][] configMap = new int[2][]
    // {
    //     new int[] { 0, 1 },
    //     new int[] { 2, 4 }
    // };

    private void Awake()
    {
        isGetSensors = false;
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

        
        TestGetSensorsButton.onClick.AddListener(() =>
        {
            Debug.Log("Pressed TestSensors");
            
            // isGetSensors = isGetSensors == false;
            // ReceiveBroadcastSignal(8200, 1500);
            // int broadcastPort = 8200; // Example port number
            // int bufferSize = 1500; // Example buffer size
            // List<int> signalData = CallReceiveBroadcastSignal(broadcastPort, bufferSize);
            // // Process the received signal data as needed
            // foreach (int data in signalData)
            // {
            //     Debug.Log("Received data: " + data);
            // }
            
            // int broadcastPort = 8200;
            // int bufferSize = 1500;
            // GetBroadcastSignal(broadcastPort, bufferSize);
            
            // GetSensors();
        });
        
        

    }

    private void Update()
    {
        if (isGetSensors)
        {
            GetSensors();
        }
    }

    private void DisplayTestArray(int[][] targetArray)
    {
        string testDisplayMap;

        if (targetArray.Length > 0)
        {
            Debug.Log("--------------- \n");
            for (int y = 0; y < targetArray.Length; y++)
            {
                testDisplayMap = "";
                testDisplayMap += "[";
                for (int x = 0; x < targetArray[0].Length; x++)
                {
                    testDisplayMap += " " + targetArray[y][x];
                }

                if (y < targetArray.Length - 1)
                {
                    testDisplayMap += " ], \n";
                }
                else
                {
                    testDisplayMap += " ] \n";
                }
                Debug.Log(testDisplayMap);
            }

            Debug.Log("--------------- \n");
        }
    }
    
    private void GetSensors()
    {
        const int sRows = 4;
        const int sCols = 4;
        IntPtr arrPtr = getSensors();
        int[][] arr = new int[sRows][];

        // Marshal the array of pointers (which point to the arrays of bools)
        IntPtr[] ptrArray = new IntPtr[sRows];
        Marshal.Copy(arrPtr, ptrArray, 0, sRows);

        for (int i = 0; i < sRows; i++)
        {
            arr[i] = new int[sCols];
            // Now copy the bool values for each row
            IntPtr rowPtr = ptrArray[i];
            byte[] boolBytes = new byte[sCols];
            Marshal.Copy(rowPtr, boolBytes, 0, sCols);

            for (int j = 0; j < sCols; j++)
            {
                arr[i][j] = boolBytes[j];
                // Debug.Log("arr["+ i +"]["+j+"]: " + arr[i][j]);
            }
        }
        DisplayTestArray(arr);
    }

    public static List<int> ReceiveBroadcastSignal(int broadcastPort, int bufferSize)
    {
        IntPtr signalDataPtr = receiveBroadcastSignal(broadcastPort, bufferSize);
        if (signalDataPtr == IntPtr.Zero)
            return new List<int>();
        List<int> signalData = new List<int>();
        unsafe
        {
            int* dataPtr = (int*)signalDataPtr;
            int size = *dataPtr++;
            for (int i = 0; i < size; i++)
            {
                signalData.Add(*dataPtr++);
            }
        }
        Marshal.FreeHGlobal(signalDataPtr);
        return signalData;
    }
    
    // Define a helper method to convert IntPtr to List<int>
    private static List<int> MarshalIntPtrToList(IntPtr ptr, int count)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < count; i++)
        {
            list.Add(Marshal.ReadInt32(ptr, i * sizeof(int)));
        }
        return list;
    }
    
    // Call the native function and convert the returned IntPtr to List<int>
    public List<int> CallReceiveBroadcastSignal(int broadcastPort, int bufferSize)
    {
        IntPtr ptr = receiveBroadcastSignal(broadcastPort, bufferSize);
        List<int> signalData = MarshalIntPtrToList(ptr, bufferSize);
        // Free the allocated memory in the native code
        Marshal.FreeCoTaskMem(ptr);
        return signalData;
    }
    
    // Function to convert the received signal data from IntPtr to a List<int>
    private List<int> ConvertSignalData(IntPtr signalDataPtr, int length)
    {
        List<int> signalData = new List<int>();
        for (int i = 0; i < length; i++)
        {
            int value = Marshal.ReadInt32(signalDataPtr, i * sizeof(int));
            signalData.Add(value);
        }
        return signalData;
    }
    
    // Function to call the receiveBroadcastSignal and process the data
    public void GetBroadcastSignal(int broadcastPort, int bufferSize)
    {
        IntPtr signalDataPtr = receiveBroadcastSignal(broadcastPort, bufferSize);
        if (signalDataPtr == IntPtr.Zero)
        {
            Debug.LogError("Failed to receive broadcast signal.");
            return;
        }
        // Assuming the length of the signal data is known or can be determined
        int length = bufferSize / sizeof(int); // Example calculation
        List<int> signalData = ConvertSignalData(signalDataPtr, length);
        // Process the received signal data
        foreach (int data in signalData)
        {
            Debug.Log("Received Signal Data: " + data);
        }
    }
}
