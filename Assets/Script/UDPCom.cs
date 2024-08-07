using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class UDPCom : MonoBehaviour
{
    [Header("Arduino Info")]
    public string IP = "255.255.255.255";
    public int IPport = 8200;

    [Header("Receiver Info")]
    public int returnPort = 8200;

    [Header("Interactions")] 
    // public GameObject Leds;

    [Header("Info")] 
    [TextArea(2, 3)] 
    public string setup = "some description";

    private string packetData = null;
    private UdpClient UdpClient;

    private void Start()
    {
        // StartCoroutine(ReceiveStatus('S'));
    }


    private void Update()
    {
        
    }

    public void SendUdpLedOn()
    {
        packetData = "L1, on";
        SendUDP(packetData);
    }

    public void SendUdpLedOff()
    {
        packetData = "L0, off";
        SendUDP(packetData);
    }
    
    public void SendUdpLedSwitch()
    {
        packetData = "L2 and L1, switch state";
        SendUDP(packetData);
    }

    private void UpdateUDPStatus()
    {
        packetData = "S1, returns status";
        SendUDP(packetData);
        
        ReceiveStatus('S');
    }

    private IEnumerator ReceiveStatus(char targetLetter)
    {
        UdpClient minUdpClient = new UdpClient(returnPort);
        UdpClient = minUdpClient;
        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            // Debug.Log("HI");
            try
            {
                Debug.Log(minUdpClient.Available > 0);
                if (minUdpClient.Available > 0)
                {
                    Byte[] receiveBytes = minUdpClient.Receive(ref remoteIPEndPoint);
                    String returnData = Encoding.ASCII.GetString(receiveBytes);
                    FindData(returnData, targetLetter);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            yield return null; // wait for the next frame
        }
    }


    private void FindData(string searchString, char target)
    {
        for (int i = 0; i < searchString.Length; i++)
        {
            // if (searchString[i] == target)
            // {
            //     int io = i + 1;
            //     UpdateLight(searchString[io]);
            // }
            Debug.Log("searchString["+i+"]" + searchString[i]);
        }
    }

    private void UpdateLight(char io)
    {
        // switch (io)
        // {
        //     case '0':
        //         Leds.GetComponent<Renderer>().material.color = Color.white;
        //         break;
        //     case '1' :
        //         Leds.GetComponent<Renderer>().material.color = Color.yellow;
        //         break;
        //     default:
        //         Leds.GetComponent<Renderer>().material.color = Color.white;
        //         break;
        // }

        return;
    }

    private void SendUDP(string sendData)
    {
        byte[] myData = System.Text.ASCIIEncoding.ASCII.GetBytes(sendData);
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP), IPport);
        Socket myClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        myClient.SendTo(myData, ep);
        myClient.Close();
    }
    
    // public void OnApplicationQuit()
    // {
    //     UdpClient.Close();
    // }

}
