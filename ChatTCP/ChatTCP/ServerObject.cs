using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
   public class ServerObject
    {
        WriteLog logger = new WriteLog();
        string ip;
        int port;
        static TcpListener tcpListener; //Сервер для прослушивания 
        List<ClientObject> clients = new List<ClientObject>(); //Все подключения
       public ServerObject(string IP, int Port)
        {
            ip = IP;
            port = Port;
        }
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(String id)
        {
            //Получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);

            //И удаляем его из списка подключений
            if (client != null)
            {
                clients.Remove(client);
            }

        }

        //Прослушивание входящих сообщений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse(ip), port);
                tcpListener.Start();
                Console.WriteLine($"Сервер запущен. {ip}:{port} \nОжидание подключений... ");
                logger.Write($"Сервер запущен. {ip}:{port}");
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Disconnect();   //Отключение связи
            }
        }


        //Трансляция сообщения подключённым клиентам
        protected internal void BroadcastMessage(String message, String id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)   //Если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length);   //Передача данных
                    logger.Write($"Пользователю {clients[i]}, отправлено сообщение: {message}");
                }
            }
        }


        //Отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop();                                                                 //Остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();                                                             //Отключение клиента
                logger.Write($"Сервер оборвал соеденинение с пользователем {clients[i]}");
            }
            logger.Write($"Сервер остановлен");
            Environment.Exit(0);                                                                //Завершение процесса
        }
    }
}
