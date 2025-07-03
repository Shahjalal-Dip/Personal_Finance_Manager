using System;
using System.Text;

namespace Personal_Finance_Manager.Utils
{
    public static class SecurityHelper
    {
        public static string HashPassword(string plainPassword)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainPassword));
        }

        public static string UnhashPassword(string hashedPassword)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(hashedPassword));
        }
    }
}
