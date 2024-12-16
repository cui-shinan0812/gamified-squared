using System;
using System.Runtime.InteropServices;
using UnityEngine;
public class UDPReceiver : MonoBehaviour
{
    [DllImport("libUnityPlugIn", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartUDPReceiver();
    [DllImport("libUnityPlugIn", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopUDPReceiver();
    [DllImport("libUnityPlugIn", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetReceivedData(out IntPtr data, out int length);
    private void Start()
    {
        // int result = StartUDPReceiver();
        // if (result != 0)
        // {
        //     Debug.LogError("Failed to start UDP receiver.");
        // }
    }
    private void Update()
    {
        IntPtr dataPtr;
        int length;
        int result = GetReceivedData(out dataPtr, out length);
        if (result == 0)
        {
            byte[] data = new byte[length];
            Marshal.Copy(dataPtr, data, 0, length);
            // Process the received data
            Debug.Log($"Received {length} bytes of data.");
            // Free the memory allocated by the C++ function
            Marshal.FreeHGlobal(dataPtr);
        }
    }
    private void OnApplicationQuit()
    {
        int result = StopUDPReceiver();
        if (result != 0)
        {
            Debug.LogError("Failed to stop UDP receiver.");
        }
    }
}