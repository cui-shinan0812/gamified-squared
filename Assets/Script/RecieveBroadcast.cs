using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class RecieveBroadcast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(GetUDP());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GetUDP()
    {
        Socket sock = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint iep = new IPEndPoint(IPAddress.Any, 8200);
        sock.Bind(iep);
        EndPoint ep = (EndPoint)iep;
        Debug.Log("Ready to receive...");

        while (true)
        {
            Debug.Log(sock.Available);
            if (sock.Available > 0)
            {
                byte[] data = new byte[4096];
                int recv = sock.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, recv);
                Debug.Log("received: " + stringData + " from: " + ep.ToString());
            }

            yield return null; // wait for the next frame
        }
    }

}
