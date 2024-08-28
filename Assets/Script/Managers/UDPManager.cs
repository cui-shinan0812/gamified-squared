using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

// UDPManager is responsible to receive and further process the received data from UDP message.
public class UDPManager : MonoBehaviour
{
    public bool isDisplayFrame;
    public static UDPManager Instance { private set; get; }
    private int numOfPorts;
    private int maxLength;
    private int port = 8200;
    private int maxPossibleLedConnectEachController = 170;
    private UdpClient server;
    private bool[][] tempStepMap;
    
    void Start()
    {
        numOfPorts = PlayerPrefs.GetInt(DllInitiater.NUM_OF_PORTS);
        maxLength = PlayerPrefs.GetInt(DllInitiater.MAX_LENGTH);
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
        
        // Debug code
        // Convert byte array to hexadecimal string
        // StringBuilder hex = new StringBuilder(data.Length * 2);
        // foreach (byte b in data)
        //     hex.AppendFormat("{0:x2}", b);
        // string message = hex.ToString();

        // Debug.Log("Received message: " + message);

        // Start listening for the next message
        server.BeginReceive(ReceiveCallback, server);
    }

    void OnDestroy()
    {
        // Stop the server when the game object is destroyed
        server.Close();
    }
    
    private void ConvertTo2DBoolAry(byte[] data)
    {
        int maxCols = numOfPorts;
        Debug.Log("[UDPManager] maxCols: " + maxCols);
        int maxRows = maxLength; 
        Debug.Log("[UDPManager] maxRows: " + maxRows);
        int numOfControllersUsed = PlayerPrefs.GetInt(DllInitiater.CONTROLLER_USED);
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
                    int receivedMessageNo = y * maxPossibleLedConnectEachController + (x+y);
                    if (receivedMessage[receivedMessageNo] == 0xab)
                    {
                        receivedData[y][x + PlayerPrefs.GetInt(DllInitiater.NUM_OF_PORTS) * receiverNo] = true;
                    }
                    else
                    {
                        receivedData[y][x + PlayerPrefs.GetInt(DllInitiater.NUM_OF_PORTS) * receiverNo] = false;
                    }
                }
            }
        }
        
        // bool[][] testReceivedData = new bool[4][];
        // for (int i = 0; i < 4; i++)
        // {
        //     testReceivedData[i] = new bool[6];
        //     for (int j = 0; j < testReceivedData[0].Length; j++)
        //     {
        //         testReceivedData[i][j] = true;
        //     }
        // }

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

        int h = PlayerPrefs.GetInt(DllInitiater.HEIGHT);
        int w = PlayerPrefs.GetInt(DllInitiater.WIDTH);
        
        // Call the C++ function
        GetSensors(framePtr, h, w);
    
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
        IntPtr arrPtr = DllManager.getSensors(framePtr);
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

    public bool[][] GetTempStepMap()
    {
        return tempStepMap;
    }
}
