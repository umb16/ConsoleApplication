using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    class Client
    {
        string request;
        public Client(TcpClient сlient)
        {
            //Console.WriteLine("Client connect!");
            request = "";
            byte[] Buffer = new byte[1024];
            int Count;
            while ((Count = сlient.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                request += Encoding.ASCII.GetString(Buffer, 0, Count);
                if (request.IndexOf("\r\n\r\n") >= 0 || request.Length > 4096)
                {
                    break;
                }
            }
            Match ReqMatch = Regex.Match(request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");



            // Получаем строку запроса
            request = ReqMatch.Groups[1].Value;
            Parser();
            string Str = request;
            // Приведем строку к виду массива байт
            Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            сlient.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            сlient.Close();
            //  Console.WriteLine("Disconnect!");
        }

        void Parser()
        {
            int index = -1;
            if (request.Contains("[loginCode:"))
            {
                if (!int.TryParse(PushString.GetString("loginCode", request), out index))
                    index = -1;
                if (index >= 0 && index < Server.users.usersList.Count)
                    Server.users.usersList[index].timer = DateTime.Now.Second;
            }
            if (PushString.ContainTag("login", request))
            {
                string login = PushString.GetString("login", request);
                string password = PushString.GetString("password", request);
                // Request = PushString.setValue("loginCode", checkLogin(login, password));
                request = PushString.SetValue("status", Server.users.CheckLoginInfo(login, password)) + PushString.SetValue("loginCode", Server.users.GetIndex(login)) + PushString.SetValue("onlineList", Serialize.Serialization(Server.users.GetOnline()));
            }
            else if (PushString.ContainTag("registr", request))
            {
                string login = PushString.GetString("login", request);
                string password = PushString.GetString("password", request);
                request = PushString.SetValue("status", Server.users.Registr(login, password));
            }
            else if (PushString.ContainTag("logout", request))
            {
                if (index > -1)
                {
                    if (index < Server.users.usersList.Count)
                    {
                        Server.users.usersList[index].status = Constants.zero;
                        request = PushString.SetValue("status", Constants.zero);
                        Console.WriteLine("{0} logout", Server.users.usersList[index].login);
                    }
                }
            }
            else if (PushString.ContainTag("check", request))
            {
                if (index >= 0 && index < Server.users.usersList.Count)
                {
                    request = PushString.SetTag("message") + Server.users.usersList[index].message + PushString.SetValue("status", Server.users.usersList[index].status) + PushString.SetValue("onlineList", Serialize.Serialization(Server.users.GetOnline()));
                }
            }
            if (PushString.ContainTag("inviteAbort", request))
            {
                request = PushString.SetValue("status", Constants.loginOk);
                Server.users.usersList[index].status = Constants.loginOk;

            }
            if (PushString.ContainTag("invite", request))
            {
                if (Server.users.sendInvite(PushString.GetString("login", request), PushString.SetValue("inviter", Server.users.usersList[index].login)))
                {
                    if (index >= 0 && index < Server.users.usersList.Count)
                    {
                        request = PushString.SetValue("status", Constants.waitInviteResponse);
                        Server.users.usersList[index].status = Constants.waitInviteResponse;
                    }
                }
                else
                {
                        request = PushString.SetValue("status", Server.users.usersList[index].status);
                }
            }
        }
    }
}
