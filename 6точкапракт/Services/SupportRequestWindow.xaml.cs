using System;
using System.IO;
using System.Text;
using System.Windows;
using ClassroomBooking.Services;

namespace ClassroomBooking
{
    public partial class SupportRequestWindow : Window
    {
        private readonly string appVersion;
        private readonly string logPath;

        public SupportRequestWindow(string version, string logFilePath)
        {
            InitializeComponent();
            appVersion = version;
            logPath = logFilePath;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string subject = SubjectTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Заполните тему и описание проблемы.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClassroomBooking", "SupportRequests");
                Directory.CreateDirectory(folder);
                string ticketId = "SUP-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string filePath = Path.Combine(folder, ticketId + ".txt");

                string report = $"ID: {ticketId}\nДата: {DateTime.Now}\nВерсия: {appVersion}\nТема: {subject}\nОписание: {description}\nЖурнал: {logPath}";
                File.WriteAllText(filePath, report, Encoding.UTF8);

                SupportLogger.Info("Создано обращение: " + ticketId);
                MessageBox.Show("Обращение сохранено: " + ticketId, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                SupportLogger.Error("Не удалось сохранить обращение.", ex);
                MessageBox.Show("Ошибка сохранения обращения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}