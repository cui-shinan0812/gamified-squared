using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class UDPManager : MonoBehaviour
{
    public bool isDisplayFrame;
    public static UDPManager Instance { private set; get; }
    private int M = 4;
    private int N = 4;
    private int numOfPorts = 4;
    private int maxLength = 6;
    private int port = 8200;
    private UdpClient server;
    private string message;
    private bool[][] tempStepMap;
    
    // Define the signature of the function in the C++ DLL
    [DllImport("libUnityPlugIn", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ReceiveMessageFromUnity(string message);
    [DllImport("libUnityPlugIn")]
    private static extern IntPtr getSensors(IntPtr hardwareMatrix);

    void Start()
    {
        message = "";
        Instance = this;
        isDisplayFrame = false;
        
        // Create the UDP server
        server = new UdpClient(port);
        server.BeginReceive(ReceiveCallback, server);
        Debug.Log("[UDPManager] Server started on port: " + port);
    }

    void ReceiveCallback(IAsyncResult result)
    {
        // Receive the data from the client
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = server.EndReceive(result, ref clientEndPoint);

        ConvertTo2DBoolAry(data);
        
        // Convert byte array to hexadecimal string
        StringBuilder hex = new StringBuilder(data.Length * 2);
        foreach (byte b in data)
            hex.AppendFormat("{0:x2}", b);
        string message = hex.ToString();

        // Debug.Log("Received message: " + message);

        // Start listening for the next message
        server.BeginReceive(ReceiveCallback, server);
    }
    
    public static bool[][] PassMessageToCpp(string message)
    {
        int sRows = 4;
        int sCols = 4;
        IntPtr ptr = ReceiveMessageFromUnity(message);
        int[][] arr = new int[sRows][];

        // Marshal the array of pointers (which point to the arrays of bools)
        IntPtr[] ptrArray = new IntPtr[sRows];
        Marshal.Copy(ptr, ptrArray, 0, sRows);

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
        bool[][] hardwareMatrix = new bool[4][];
        for (int i = 0; i < hardwareMatrix.Length; i++)
        {
            hardwareMatrix[i] = new bool[4];
            for (int j = 0; j < hardwareMatrix[0].Length; j++)
            {
                hardwareMatrix[i][j] = arr[i][j] != 0;
            }
        }
        return hardwareMatrix;
    }

    void OnDestroy()
    {
        // Stop the server when the game object is destroyed
        server.Close();
    }
    
    private void ConvertTo2DBoolAry(byte[] data)
    {
        int maxCols = numOfPorts; // 4
        Debug.Log("[UDPManager] maxCols: " + maxCols);
        int maxRows = maxLength; // 6 
        Debug.Log("[UDPManager] maxRows: " + maxRows);
        int numOfControllersUsed = 1;
        bool[][] receivedData = new bool[maxCols][];
        for (int i = 0; i < maxCols; i++)
        {
            receivedData[i] = new bool[maxRows];
            for (int j = 0; j < maxRows; j++)
            {
                receivedData[i][j] = false;
            }
        }
        for (int i = 0; i < numOfControllersUsed; i++)
        {
            // Convert byte array to list of integers
            List<int> receivedMessage = new List<int>(Array.ConvertAll(data, Convert.ToInt32));
            int receiverNo = receivedMessage[1];
            receivedMessage.RemoveRange(0, 3);
            string msg = "[UDPManager] ";
            List<int> abIndex = new List<int>();
            for (int j = 0; j < receivedMessage.Count; j++)
            {
                if (receivedMessage[j] == 0xab || receivedMessage[j] == 0x00)
                {
                    abIndex.Add(j);
                    msg += j+ ", ";
                }
            }

            
            
            
            Debug.Log("[UDPManager] receivedMessage: \n"+msg);
            
            Debug.Log("[UDPManager] receivedData.Length: "+ receivedData.Length);
            Debug.Log("[UDPManager] receivedData[0].Length: "+ receivedData[0].Length);

            for (int x = 0; x < receivedData[0].Length; x++)
            {
                for (int y = 0; y < receivedData.Length; y++)
                {
                    int receivedMessageNo = y * 170 + (x+y);
                    if (receivedMessage[receivedMessageNo] == 0xab)
                    {
                        receivedData[y][x + 4 * receiverNo] = true;
                    }
                    else
                    {
                        receivedData[y][x + 4 * receiverNo] = false;
                    }
                }
            }
        }
        
        bool[][] testReceivedData = new bool[4][];
        for (int i = 0; i < 4; i++)
        {
            testReceivedData[i] = new bool[6];
            for (int j = 0; j < testReceivedData[0].Length; j++)
            {
                testReceivedData[i][j] = true;
            }
        }

        bool[][] reversedData = ReverseRowsAndCols(receivedData);

        PassToCppProcess(reversedData);

        Debug.Log("[UDPManager] [reversedData]----------------\n");
        DisplayAnswerViewMap(reversedData);
        Debug.Log("[UDPManager] [reversedData]----------------\n");
    }

    
    private void DisplayAnswerViewMap(bool[][] map)
    {
        string testDisplayMap;

        if (map.Length > 0)
        {
            for (int y = 0; y < map.Length; y++)
            {
                testDisplayMap = "[UDPManager] ";
                testDisplayMap += "[";
                for (int x = 0; x < map[0].Length; x++)
                {
                    testDisplayMap += " " + map[y][x];
                }

                if (y < map.Length - 1)
                {
                    testDisplayMap += " ], \n";
                }
                else
                {
                    testDisplayMap += " ] \n";
                }
                Debug.Log(testDisplayMap);
            }
        }
    }
    
    private bool[][] ReverseRowsAndCols(bool[][] receivedData)
    {
        int maxRows = receivedData.Length;
        int maxCols = receivedData[0].Length;

        bool[][] reversedData = new bool[maxCols][];

        for (int i = 0; i < maxCols; i++)
        {
            reversedData[i] = new bool[maxRows];
            for (int j = 0; j < maxRows; j++)
            {
                reversedData[i][j] = receivedData[j][i];
            }
        }

        return reversedData;
    }

    private void PassToCppProcess(bool[][] stepMap)
    {
        int m = stepMap.Length;
        int n = stepMap[0].Length;
        // Allocate unmanaged memory for the 2D array
        IntPtr[] rows = new IntPtr[m];
        for (int i = 0; i < m; ++i)
        {
            // Create a 1D array for the current row
            byte[] row = new byte[n]; // Use byte array to represent bool array
            for (int j = 0; j < n; ++j)
            {
                row[j] = stepMap[i][j] ? (byte)1 : (byte)0; // Convert bool to byte
            }

            // Allocate unmanaged memory for the row and copy the row data
            rows[i] = Marshal.AllocHGlobal(n * sizeof(byte)); // Allocate memory for bool/byte array
            Marshal.Copy(row, 0, rows[i], n); // Copy the byte array to unmanaged memory
        }

        IntPtr framePtr = Marshal.AllocHGlobal(m * IntPtr.Size);
        Marshal.Copy(rows, 0, framePtr, m);

        // Call the C++ function
        GetSensors(framePtr, 4, 4);
    
        // Free the unmanaged memory
        for (int i = 0; i < m; ++i)
        {
            Marshal.FreeHGlobal(rows[i]);
        }
        Marshal.FreeHGlobal(framePtr);
    }

    
    private void GetSensors(IntPtr framePtr, int sRows, int sCols)
    {
        // Call the C++ function and get the pointer to the bool array
        IntPtr arrPtr = getSensors(framePtr);
        bool[][] arr = new bool[sRows][];

        // Marshal the array of pointers (which point to the arrays of bools)
        IntPtr[] ptrArray = new IntPtr[sRows];
        Marshal.Copy(arrPtr, ptrArray, 0, sRows);

        for (int i = 0; i < sRows; i++)
        {
            arr[i] = new bool[sCols];
            // Now copy the bool values for each row
            IntPtr rowPtr = ptrArray[i];
            byte[] boolBytes = new byte[sCols];
            Marshal.Copy(rowPtr, boolBytes, 0, sCols);

            for (int j = 0; j < sCols; j++)
            {
                arr[i][j] = boolBytes[j] != 0; // Convert byte to bool (non-zero is true)
            }
        }

        tempStepMap = arr;
        Debug.Log("[UDPManager] arr---------------------");
        DisplayAnswerViewMap(arr);
        Debug.Log("[UDPManager] arr---------------------");
    }
    
    private void DisplayTestArray(int[][] targetArray)
    {
        string testDisplayMap;

        if (targetArray.Length > 0)
        {
            for (int y = 0; y < targetArray.Length; y++)
            {
                testDisplayMap = "[UDPManager] ";
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

        }
    }

    public bool[][] GetTempStepMap()
    {
        return tempStepMap;
    }
}
