using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    class Server
    {
        static int frequency = 60;
        static int saveTimer = DateTime.Now.Second;
        static int listenPort = 25565;
        public static Users users = new Users();
        public static TcpListener Listener; // Объект, принимающий TCP-клиентов
        static void ListenerThread()
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

        static void checkOnlineUsersThread()
        {
            while (true)
            {
                for (int i = 0; i < users.usersList.Count; i++)
                {
                    if(users.usersList[i].status != Constants.zero)
                    if (users.usersList[i].timer + 5 < DateTime.Now.Second)
                    {
                        
                        users.usersList[i].status = Constants.zero;
                        Console.WriteLine("{0} timeout kick", Server.users.usersList[i].login);
                    }
                }
                if (saveTimer + frequency < DateTime.Now.Second)
                {
                    saveTimer = DateTime.Now.Second;
                    Serialize.Serialization("111.mdb", users);
                    Console.WriteLine("Save ok");

                }
                Thread.Sleep(1000);
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

            Thread thread = new Thread(new ThreadStart(ListenerThread));
            // И запускаем этот поток
            thread.Start();

            Thread thread2 = new Thread(new ThreadStart(checkOnlineUsersThread));
            // И запускаем этот поток
            thread2.Start();

            // В бесконечном цикле
            Console.WriteLine("Server start");
            while (true)
            {
                if (Console.ReadLine() == "stop")
                {
                    Serialize.Serialization("111.mdb", users);
                    Environment.Exit(0);
                }
            }
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
                        if (int.TryParse(args[i + 1], out listenPort))
                            Console.WriteLine("Set port {0}", listenPort);
                        break;
                    }
            }

            // Создадим новый сервер на порту 80
            new Server(listenPort);

        }
    }
}
