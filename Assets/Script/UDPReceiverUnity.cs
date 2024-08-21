using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class UDPReceiverUnity : MonoBehaviour
{
    [DllImport("libUnityPlugIn")] // Replace with the name of your DLL
    private static extern void StartUDPReceiver();
    [DllImport("libUnityPlugIn")] // Replace with the name of your DLL
    
    private static extern IntPtr GetReceivedMessage();

    [SerializeField] private Button testUDPButton;
    private bool flag;
    void Start()
    {
        flag = false;
        testUDPButton.onClick.AddListener(() =>
        {
            StartUDPReceiver();
            flag = flag != true;
        });
    }
    void Update()
    {
        if (flag)
        {
            IntPtr messagePtr = GetReceivedMessage();
            string message = Marshal.PtrToStringAnsi(messagePtr);
            Debug.Log("Received message: " + message);
        }
    }
}