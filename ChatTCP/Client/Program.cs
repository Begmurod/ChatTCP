using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
       
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            
            Console.Write("Добро пожаловать, введите ip адрес для подключения к серверу: ");
            string host = Console.ReadLine();
            Console.Write("Введите порт: ");
            int port = Int32.Parse(Console.ReadLine());
            Console.Write("Введите свое имя: ");
            string userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                if (null != host && 0 != host.Length)
                {
                    client.Connect(host, port);                                         // Подключение клиента
                    stream = client.GetStream();                                        // Получаем поток
                }

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                if (null != receiveThread)
                {
                    receiveThread.Start();                                                  //старт потока
                    Console.WriteLine($"{userName}, вы успешо подключились к серверу, всего доброго!");
                    SendMessage();
                }
                else
                {
                    Console.WriteLine("Соединение разорвано");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }


        // отправка сообщений
        static void SendMessage()
        {
            
            while (true)
            {
                Console.WriteLine("Введите ваше сообщение: ");
                string message = Console.ReadLine();
                if (null != message && 0 != message.Length)
                {
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }

        }


        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];                                    // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine($"{message}");                                   //вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Соединение было разорвано, сожалеем!");            //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();                                                  //отключение потока
            if (client != null)
                Console.WriteLine("До скорой встречи!");
            client.Close();                                                  //отключение клиента
            Environment.Exit(0);                                                 //завершение процесса
        }

    }

}
