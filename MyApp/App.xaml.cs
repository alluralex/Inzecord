using System.Configuration;
using System.Data;
using System.Windows;

namespace MyApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isNewInstance;

            // Создаем Mutex с уникальным именем
            _mutex = new Mutex(true, "MyApp_UniqueMutexName", out isNewInstance);

            if (!isNewInstance)
            {
                Current.Shutdown();
                MessageBox.Show("Приложение уже запущено!", "Inzeworld");
                return;
            }

            base.OnStartup(e);
        }
    }
}
