using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            String host = null;     //Default
            String port = "30000";  //Default
            Socket      s1;
            TcpServer   echos;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--help")
                {
                    Console.WriteLine("Format: EchoServer -h [local host] -p [local port]");
                    Environment.Exit(0);
                }
                else if (args[i] == "-h")
                    host = args[++i];
                else if (args[i] == "-p")
                    port = args[++i];
                else
                {
                    Console.Error.WriteLine("ERROR: incorrect inputs \nFormat: EchoServer -h [local host] -p [local port]");
                    Environment.Exit(0);
                }
            }

            /* if only given port, host is ANY */
            echos = new TcpServer(host, port);

            while(true)
            {
                s1 = echos.so.Accept();
                ClientHandle client = new ClientHandle(s1);
            }
        }
    }
}
