namespace ClassroomBooking.Configuration
{
    public class AppSettings
    {
        public string AppTitle { get; set; } = "Classroom Booking";
        public string WorkDayStart { get; set; } = "08:00";
        public string WorkDayEnd { get; set; } = "20:00";
        public int MinBookingMinutes { get; set; } = 30;
        public bool AllowUnavailableRooms { get; set; } = false;
        public string StatusMessage { get; set; } = "Система готова к работе";
    }
}