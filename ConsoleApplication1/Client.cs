using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                string loginCode = PushString.GetString("loginCode", request);
                int index=-1;
                if (int.TryParse(loginCode, out index))
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
            }
            else if (PushString.ContainTag("check", request))
            {
                string loginCode = PushString.GetString("loginCode", request);
                int index=-1;
                if (int.TryParse(loginCode, out index))
                {

                    if(index>=0&&index<Server.users.usersList.Count)
                    request = PushString.SetTag("message") + Server.users.usersList[index].message + PushString.SetValue("status", Server.users.usersList[index].status) + PushString.SetValue("onlineList", Serialize.Serialization(Server.users.GetOnline()));
                }
            }
            if (PushString.ContainTag("invite", request))
            {

            }
        }
    }
}
