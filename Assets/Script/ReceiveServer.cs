using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
public class ReceiveServer : MonoBehaviour
{
    private int port = 8200;
    private UdpClient server;
    void Start()
    {
        // Create the UDP server
        server = new UdpClient(port);
        server.BeginReceive(ReceiveCallback, server);
        Debug.Log("Server started on port: " + port);
    }
    void ReceiveCallback(IAsyncResult result)
    {
        // Receive the data from the client
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = server.EndReceive(result, ref clientEndPoint);
        string message = Encoding.UTF8.GetString(data);
        Debug.Log("Received message: " + message);
        // Start listening for the next message
        server.BeginReceive(ReceiveCallback, server);
    }
    void OnDestroy()
    {
        // Stop the server when the game object is destroyed
        server.Close();
    }
}