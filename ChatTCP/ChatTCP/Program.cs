using Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ChatTCP
{
    class Program
    {
       static ServerObject server = null;

        public static void DisplayGatewayAddresses()
        {
            Console.WriteLine("Доступные адреса: ");
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }
            Console.WriteLine(IP4Address);
            Console.WriteLine("127.0.0.1");
                
            
        }// потока для прослушивания
        static void Main(string[] args)
       {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Console.WriteLine("Для запуска сервера выберите ip адрес");
            DisplayGatewayAddresses();
            Console.Write("Введите адрес: ");
            string ip = Console.ReadLine();
            Console.Write("Введите порт: ");
            int port = Int32.Parse(Console.ReadLine());
                                                                          // сервер
                Thread listenThread;
                try
                {
                    server = new ServerObject(ip,port);
                    listenThread = new Thread(new ThreadStart(server.Listen));
                    listenThread.Start();                                                           //старт потока
                }
                catch (Exception ex)
                {
                    server.Disconnect();
                    Console.WriteLine(ex.Message);
                }
           Console.ReadKey();
        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            server.Disconnect();
        }

    }
}

