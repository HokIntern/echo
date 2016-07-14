using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
{
    class ClientHandle
    {
        Socket so;
        int bytecount;

        public ClientHandle(Socket s)
        {
            so = s;
            Thread chThread = new Thread(echo);
            chThread.Start();
        }

        private void echo()
        {
            String remoteHost = ((IPEndPoint)so.RemoteEndPoint).Address.ToString();
            String remotePort = ((IPEndPoint)so.RemoteEndPoint).Port.ToString();
            Console.WriteLine("Connection established with {0}:{1}\n", remoteHost, remotePort);

            for (;;)
            {
                /* Receive */
                byte[] bytes = new byte[256];
                bytecount = so.Receive(bytes);
                Console.WriteLine("Received {0}bytes from {1}:{2} - {3}", bytecount, remoteHost, remotePort, Encoding.UTF8.GetString(bytes));

                if (!isConnected())
                {
                    Console.WriteLine("Connection lost with {0}:{1}", remoteHost, remotePort);
                    break;
                }

                /* Echo */
                String ss = Encoding.UTF8.GetString(bytes).Substring(0, bytecount);
                byte[] sendBytes = Encoding.UTF8.GetBytes(ss);
                bytecount = so.Send(sendBytes);
                //Console.WriteLine("Sent     {0}bytes to {1}:{2} - {3} \n", bytecount, remoteHost, remotePort, Encoding.UTF8.GetString(bytes));
            }
            Console.WriteLine("Closing connection with {0}:{1}", remoteHost, remotePort);
            so.Shutdown(SocketShutdown.Both);
            so.Close();
            Console.WriteLine("Connection closed\n");
        }

        private bool isConnected()
        {
            try
            {
                return !(so.Poll(1, SelectMode.SelectRead) && so.Available == 0);
            }
            catch(SocketException) { return false; }
        }
    }
}
