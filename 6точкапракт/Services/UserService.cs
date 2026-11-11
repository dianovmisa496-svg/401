using ClassroomBooking.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClassroomBooking.Services
{
    public class UserService
    {
        private List<User> _users = new List<User>
        {
            new User { Id = 1, Name = "Иван Петров", Login = "ipetrov", Role = "Студент" },
            new User { Id = 2, Name = "Мария Сидорова", Login = "msidorova", Role = "Преподаватель" },
            new User { Id = 3, Name = "Алексей Иванов", Login = "aivanov", Role = "Студент" }
        };

        public List<User> GetAllUsers() => _users;
        public User GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id)!;
    }
}
