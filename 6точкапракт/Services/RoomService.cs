using ClassroomBooking.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClassroomBooking.Services
{
    public class RoomService
    {
        private readonly List<Room> _rooms = new()
{
    new Room { Id = 1, Number = "301", ComputerCount = 15, IsAvailable = true,  MaxGroupSize = 15 },
    new Room { Id = 2, Number = "302", ComputerCount = 20, IsAvailable = true,  MaxGroupSize = 20 },
    new Room { Id = 3, Number = "405", ComputerCount = 10, IsAvailable = false, MaxGroupSize = 10 }
};

        public List<Room> GetAllRooms() => _rooms;
        public Room GetRoomById(int id) => _rooms.FirstOrDefault(r => r.Id == id)!;
    }
}
