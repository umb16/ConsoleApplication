using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class User
    {
        public User()
        {

        }
        public User(string login, string password)
        {
            this.login = login;
            this.password = password;
            status = 0;
        }
        public string message="";
        public string login;
        public string password;
        public int status;
    }

    public class Users
    {
        public List<User> usersList = new List<User>();

        public int GetIndex(string login)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].login == login)
                    return i;
            }
            return -1;
        }

        public void ClearStatus()
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                usersList[i].status = Constants.zero;
            }
        }

        public Users GetOnline()
        {
            Users tempOnlineUsers = new Users();
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].status != 0)
                    tempOnlineUsers.usersList.Add(usersList[i]);
            }
            return tempOnlineUsers;
        }

        public int CheckLoginInfo(string login, string password)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].login == login)
                    if (usersList[i].password == password)
                    {
                        if (usersList[i].status == 0)
                        {
                            usersList[i].status = Constants.loginOk;
                            Console.WriteLine("{0} login", Server.users.usersList[i].login);
                            return Constants.loginOk;
                        }
                        else
                            return Constants.doubleLogining;
                    }
                    else
                        return Constants.incorrectPassword;
            }
            return Constants.loginNotFound;
        }

        public int Registr(string login, string password)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].login == login)
                    return Constants.loginOccupied;
            }
            usersList.Add(new User(login, password));
            return Constants.accountCreateSuccess;
        }
    }

}
