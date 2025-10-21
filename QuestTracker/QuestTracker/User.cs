using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroProject

{
    //User klass
    public class User
    {

        public string Username;
        public string Password;
        public string PhoneNumber;

        public void SetInfo(string username, string password, string phonenumber)
        {
            Username = username;
            Password = password;
            PhoneNumber = phonenumber;
        }


        public void ShowProfile()
        {
            Console.WriteLine($"Profile: {Username} - {PhoneNumber}");
        }
    }
}
