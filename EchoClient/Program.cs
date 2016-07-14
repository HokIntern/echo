using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace EchoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient[] echocs;
            String host = "127.0.0.1";  //Default
            String port = "30000";      //Default
            int clientInstances = 1;
            int bytecount;

            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--help")
                {
                    Console.WriteLine("Format: EchoClient -h [peer host] -p [peer port] -n [number of clients]");
                    Environment.Exit(0);
                }
                else if (args[i] == "-h")
                    host = args[++i];
                else if (args[i] == "-p")
                    port = args[++i];
                else if (args[i] == "-n")
                {
                    if (!int.TryParse(args[++i], out clientInstances))
                        Console.Error.WriteLine("Error: must be int");
                }
                else
                {
                    Console.Error.WriteLine("ERROR: incorrect inputs \nFormat: EchoClient -h [peer host] -p [peer port] -n [number of clients]");
                    Environment.Exit(0);
                }
            }

            /* initialize and connect all clients to server */
            echocs = new TcpClient[clientInstances];
            for (int i = 0; i < clientInstances; i++)
                echocs[i] = new TcpClient(host, port);

            String msg;
            do
            {
                Console.Write("msg: ");
                msg = Console.ReadLine();
                /* reverse msg */
                char[] revMsgChar = msg.ToCharArray();
                Array.Reverse(revMsgChar);
                String revMsg = new string(revMsgChar);

                /* each client sends and receives from server */
                for (int i = 0; i < clientInstances; i++)
                {
                    /* get bytes */
                    byte[] bytes = new byte[256];
                    String comm = (i % 2 == 0 ? msg : revMsg);
                    bytes = Encoding.UTF8.GetBytes(comm);

                    /* send */
                    bytecount = echocs[i].so.Send(bytes);
                    Console.WriteLine("Sent {0}bytes to {1}:{2} - {3}", bytecount, IPAddress.Parse(((IPEndPoint)echocs[i].so.RemoteEndPoint).Address.ToString()), ((IPEndPoint)echocs[i].so.RemoteEndPoint).Port.ToString(), Encoding.UTF8.GetString(bytes));

                    /* receive */
                    bytecount = echocs[i].so.Receive(bytes);
                    Console.WriteLine("Received {0}bytes from {1}:{2} - {3} \n", bytecount, IPAddress.Parse(((IPEndPoint)echocs[i].so.RemoteEndPoint).Address.ToString()), ((IPEndPoint)echocs[i].so.RemoteEndPoint).Port.ToString(), Encoding.UTF8.GetString(bytes));
                }

                if (msg == "bye")
                    break;
            } while (msg != null);

            Console.WriteLine("\n\nClosing Connection..........");

            for (int i = 0; i < clientInstances; i++)
            {
                echocs[i].so.Shutdown(SocketShutdown.Both);
                echocs[i].so.Close();
            }
        }
    }
}
