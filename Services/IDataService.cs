using Personal_Finance_Manager.Models;
using System.Collections.Generic;

namespace Personal_Finance_Manager.Services
{
    public interface IDataService
    {
        void SaveUsers(List<User> users);
        List<User> LoadUsers();
    }
}
