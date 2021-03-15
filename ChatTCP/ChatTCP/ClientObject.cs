using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ClientObject
    {
        String userName;
        TcpClient client;
        ServerObject server;                                                        //Объект сервера
        WriteLog logger = new WriteLog();
        protected internal String Id
        {
            get;
            private set;
        }

        protected internal NetworkStream Stream
        {
            get;
            private set;
        }

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                String message = GetMessage();   //Получаем имя пользователя
                userName = message;
                logger.Write($"Пользователь {this.Id}, присоеденился к серверу под именем: {userName}");
                message = userName + " вошёл в чат";
                server.BroadcastMessage(message, this.Id);  //Посылаем сообщение о входе в чат всем подключенным пользователям
                Console.WriteLine(message);

                while (true)      //В бесконечном цикле получаем сообщения от клиента
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        logger.Write($"Получено сообщение от {this.Id} с именем {message}");
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", userName);
                        logger.Write($"Пользователь {this.Id}, под именем {userName}. Разорвал соединение и покинул чат");
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}