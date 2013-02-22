﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Server
    {
        static int listenPort = 25565;
        public static Users users = new Users();
        public static TcpListener Listener; // Объект, принимающий TCP-клиентов
        static void ListenerTread()
        {
            while (true)
            {
                // Принимаем нового клиента
                TcpClient Client = Listener.AcceptTcpClient();
                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
            }
        }

        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }

        // Запуск сервера
        public Server(int Port)
        {
            // Создаем "слушателя" для указанного порта
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start(); // Запускаем его

            Thread Thread = new Thread(new ThreadStart(ListenerTread));
            // И запускаем этот поток
            Thread.Start();
            // В бесконечном цикле
            Console.WriteLine("Server start");
            while (true)
            {
                if (Console.ReadLine() == "stop")
                {
                   // Thread.Abort();
                    Serialize.Serialization("111.mdb", users);
                    Environment.Exit(0);
                }
            }
        }

        // Остановка сервера
        ~Server()
        {
            Serialize.Serialization("111.mdb", users);
            // Если "слушатель" был создан
           /* if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }*/
        }

        static void Main(string[] args)
        {
            users = (Users)Serialize.Deserialization("111.mdb", users);
            users.ClearStatus();


            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "port")
                    if (i + 1 < args.Length)
                    {
                        int.TryParse(args[i + 1], out listenPort);
                        Console.WriteLine("Set port {0}", listenPort);
                        break;
                    }
            }
            
            // Создадим новый сервер на порту 80
                new Server(listenPort);
            
        }
    }
}
