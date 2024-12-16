using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using Random = System.Random;

// public class CppMain : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
// }

namespace ControlFunctions
{
    class CppMain : MonoBehaviour
    {
        private void Start()
        {
            string[] args = new[] {""};
            Main(args);
        }

        static void Main(string[] args)
        {
            string targetIP = "169.254.255.255";
            // string targetIP = "169.254.169.94";
            int targetPort = 4628;
            int localPort = 8200;
            int bufferSize = 1500;

            Random rd = new Random();
            int[,,] gencolor = new int[2, 2, 100];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 100; k++)
                    {
                        gencolor[i, j, k] = rd.Next(0, 3);
                    }
                }
            }

            send_broadcast(targetPort);

            int frame_no = 0;

            // write a while loop untill time equals to depth
            while (frame_no < 100)
            {
                int[,] frame = new int[2, 2];
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        frame[i, j] = gencolor[i, j, frame_no];
                    }
                }

                bool[,] step_state = new bool[2, 2];
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        step_state[i, j] = false;
                    }
                }

                DateTime[,] step_start_time = new DateTime[2, 2];
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        step_start_time[i, j] = DateTime.MinValue;
                    }
                }

                int step_row = 0;
                int step_col = 0;

                DateTime startTime = DateTime.Now;

                int counter = 0;
                while (true)
                {
                    counter++;
                    send_startframe(targetIP, targetPort);
                    send_controlnum(targetIP, targetPort, 4);
                    send_controllight_oneframe(targetIP, targetPort, frame, 2, 2);
                    send_endframe(targetIP, targetPort);

                    List<byte> receivedMessage = receiveMessage(localPort, bufferSize);

                    List<byte> receivedData_modified = receivedMessage.GetRange(3, 4);

                    List<List<byte>> reshapedreceivedData_modified = new List<List<byte>>();
                    for (int i = 0, k = 0; i < 2; i++)
                    {
                        reshapedreceivedData_modified.Add(new List<byte>());
                        for (int j = 0; j < 2; j++, k++)
                        {
                            reshapedreceivedData_modified[i].Add(receivedData_modified[k]);
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (reshapedreceivedData_modified[i][j] != 0x00 && step_state[i, j] == false && frame[i, j] == 2)
                            {
                                frame[i, j] = 4;
                            }

                            if (reshapedreceivedData_modified[i][j] != 0x00 && step_state[i, j] == false && frame[i, j] == 1)
                            {
                                DateTime yellowstepTime = DateTime.Now;
                                step_start_time[i, j] = yellowstepTime;

                                frame[i, j] = 3;

                                step_row = i;
                                step_col = j;

                                step_state[i, j] = true;
                            }
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (step_start_time[i, j] == DateTime.MinValue)
                            {
                                continue;
                            }

                            DateTime currentTime_yellow = DateTime.Now;
                            TimeSpan elapsed_yellow = currentTime_yellow - step_start_time[i, j];
                            if (elapsed_yellow.TotalSeconds >= 0.3 && step_state[i, j] == true)
                            {
                                frame[i, j] = 4;

                                send_startframe(targetIP, targetPort);
                                send_controlnum(targetIP, targetPort, 4);
                                send_controllight_oneframe(targetIP, targetPort, frame, 2, 2);
                                send_endframe(targetIP, targetPort);

                                step_start_time[i, j] = DateTime.MinValue;
                            }
                        }
                    }

                    DateTime currentTime = DateTime.Now;
                    TimeSpan elapsed = currentTime - startTime;

                    if (elapsed.TotalSeconds >= 5)
                    {
                        Console.WriteLine("\n\nbreak:  " + counter + "\n\n");
                        counter = 0;
                        break;
                    }
                }

                frame_no++;
            }
        }

        static void send_broadcast(int targetPort)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            udpClient.Send(new byte[] { 0x00, 0x00, 0x00 }, 3, new IPEndPoint(IPAddress.Broadcast, targetPort));
            udpClient.Close();
        }

        static void send_startframe(string targetIP, int targetPort)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Send(new byte[] { 0x00, 0x00, 0x01 }, 3, new IPEndPoint(IPAddress.Parse(targetIP), targetPort));
            udpClient.Close();
        }

        static void send_controlnum(string targetIP, int targetPort, int controlNum)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Send(new byte[] { 0x00, 0x00, 0x02, (byte)controlNum }, 4, new IPEndPoint(IPAddress.Parse(targetIP), targetPort));
            udpClient.Close();
        }

        static void send_controllight_oneframe(string targetIP, int targetPort, int[,] frame, int rows, int cols)
        {
            UdpClient udpClient = new UdpClient();
            byte[] data = new byte[rows * cols + 3];
            data[0] = 0x00;
            data[1] = 0x00;
            data[2] = 0x03;
            for (int i = 0, k = 3; i < rows; i++)
            {
                for (int j = 0; j < cols; j++, k++)
                {
                    data[k] = (byte)frame[i, j];
                }
            }
            udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(targetIP), targetPort));
            udpClient.Close();
        }

        static void send_endframe(string targetIP, int targetPort)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Send(new byte[] { 0x00, 0x00, 0x04 }, 3, new IPEndPoint(IPAddress.Parse(targetIP), targetPort));
            udpClient.Close();
        }

        static List<byte> receiveMessage(int localPort, int bufferSize)
        {
            UdpClient udpClient = new UdpClient(localPort);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, localPort);
            byte[] receivedBytes = udpClient.Receive(ref remoteEP);
            udpClient.Close();
            return receivedBytes.ToList();
        }
    }
}
