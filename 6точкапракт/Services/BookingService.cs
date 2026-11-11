using ClassroomBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassroomBooking.Services
{
    public class BookingService
    {
        private readonly List<Booking> _bookings = new();
        private int _nextId = 1;

        public List<Booking> GetAllBookings() => _bookings;

        public (bool Success, string Message) CreateBooking(int userId, int roomId, DateTime date,
            TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime)
                return (false, "Время начала должно быть раньше времени окончания.");

            bool hasConflict = _bookings.Any(b =>
                b.RoomId == roomId &&
                b.Date.Date == date.Date &&
                startTime < b.EndTime &&
                b.StartTime < endTime);

            if (hasConflict)
                return (false, "Конфликт бронирования: аудитория уже занята в это время.");

            _bookings.Add(new Booking
            {
                Id = _nextId++,
                UserId = userId,
                RoomId = roomId,
                Date = date,
                StartTime = startTime,
                EndTime = endTime
            });

            return (true, "Бронирование успешно создано!");
        }
    }
}