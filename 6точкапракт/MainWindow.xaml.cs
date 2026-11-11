using System;
using System.Collections.Generic;
using System.Windows;
using ClassroomBooking.Models;
using ClassroomBooking.Services;
using ClassroomBooking.Configuration;

namespace ClassroomBooking
{
    public partial class MainWindow : Window
    {
        private UserService _userService;
        private RoomService _roomService;
        private BookingService _bookingService;
        private readonly ConfigService _configService = new();
        private AppSettings _settings;

        public MainWindow()
        {
            InitializeComponent();

            _settings = _configService.Load();
            Title = _settings.AppTitle;
            StatusTextBlock.Text = _settings.StatusMessage;

            _userService = new UserService();
            _roomService = new RoomService();
            _bookingService = new BookingService();

            cmbUsers.ItemsSource = _userService.GetAllUsers();
            cmbRooms.ItemsSource = _roomService.GetAllRooms();
            dpDate.SelectedDate = DateTime.Today;

            SupportLogger.Info($"Запуск приложения. Режим: {_settings.StatusMessage}; Время: {_settings.WorkDayStart}–{_settings.WorkDayEnd}");
            RefreshBookings();
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            SupportLogger.Info("Пользователь начал создание бронирования.");

            if (cmbUsers.SelectedItem == null || cmbRooms.SelectedItem == null)
            {
                SupportLogger.Warning("Бронирование отклонено: не выбран пользователь или аудитория.");
                MessageBox.Show("Выберите пользователя и аудиторию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int userId = (int)cmbUsers.SelectedValue;
            int roomId = (int)cmbRooms.SelectedValue;
            DateTime date = dpDate.SelectedDate ?? DateTime.Today;

            if (!TimeSpan.TryParse(txtStartTime.Text, out TimeSpan startTime) || !TimeSpan.TryParse(txtEndTime.Text, out TimeSpan endTime))
            {
                SupportLogger.Warning("Бронирование отклонено: неверный формат времени.");
                MessageBox.Show("Неверный формат времени (ЧЧ:ММ).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ПРОВЕРКА КОНФИГУРАЦИИ
            if (!TimeSpan.TryParse(_settings.WorkDayStart, out TimeSpan workStart) || !TimeSpan.TryParse(_settings.WorkDayEnd, out TimeSpan workEnd))
            {
                MessageBox.Show("Ошибка конфигурации рабочего времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (startTime < workStart || endTime > workEnd)
            {
                SupportLogger.Warning("Отказ: время вне рабочего диапазона.");
                MessageBox.Show($"Бронирование доступно с {_settings.WorkDayStart} до {_settings.WorkDayEnd}.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double duration = (endTime - startTime).TotalMinutes;
            if (duration < _settings.MinBookingMinutes)
            {
                SupportLogger.Warning("Отказ: слишком короткая бронь.");
                MessageBox.Show($"Минимальная длительность - {_settings.MinBookingMinutes} минут.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var room = _roomService.GetRoomById(roomId);
            if (room == null) return;

            if (!room.IsAvailable && !_settings.AllowUnavailableRooms)
            {
                SupportLogger.Warning($"Отказ: аудитория {room.Number} недоступна.");
                MessageBox.Show("Выбранная аудитория недоступна.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = _bookingService.CreateBooking(userId, roomId, date, startTime, endTime);

                if (result.Success)
                {
                    SupportLogger.Info($"Бронирование создано. UserId={userId}; RoomId={roomId}");
                    MessageBox.Show(result.Message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshBookings();
                }
                else
                {
                    SupportLogger.Warning($"Отказ сервиса: {result.Message}");
                    MessageBox.Show(result.Message, "Отказано", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                SupportLogger.Error("Непредвиденная ошибка при бронировании.", ex);
                MessageBox.Show("Не удалось выполнить операцию. Обратитесь в поддержку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshBookings()
        {
            var displayList = new List<string>();
            foreach (var b in _bookingService.GetAllBookings())
            {
                var user = _userService.GetUserById(b.UserId);
                var room = _roomService.GetRoomById(b.RoomId);
                displayList.Add($"{b.Date:dd.MM.yyyy} | Ауд. {room?.Number} | {user?.Name} | {b.StartTime:hh\\:mm} - {b.EndTime:hh\\:mm}");
            }
            lstBookings.ItemsSource = displayList;
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SupportRequestWindow("1.0.0", SupportLogger.LogFilePath);
            window.Owner = this;
            window.ShowDialog();
        }
    }
}