using Personal_Finance_Manager.Models;
using System.Collections.Generic;

namespace Personal_Finance_Manager.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } // Store hashed in real apps!
        public List<Transaction> Transactions { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Transactions = new List<Transaction>();
        }
    }
}
