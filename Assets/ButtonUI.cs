using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices; // Ensure this using directive is at the top

public class ButtonUI : MonoBehaviour
{
    // Assuming "myDynamicLibrary.dll" exports a function named "send_controllight"
    // that takes two parameters: a string (for IP) and an int (for port).
    [DllImport("myDynamicLibrary", CallingConvention = CallingConvention.Cdecl)]
    private static extern void send_controllight([MarshalAs(UnmanagedType.LPWStr)] string targetIP, int targetPort);    
    string targetIP = "169.254.161.94";
    int targetPort = 4628;

    // // Start is called before the first frame update
    // public void Start()
    // {
        
    //     send_controllight(targetIP, targetPort);
    // }

    // // Update is called once per frame
    // public void Update()
    // {
    //     send_controllight(targetIP, targetPort);
    // }

    public void Clickonbutton()
    {
        send_controllight(targetIP, targetPort);
        Debug.Log("Called");
    }
}
