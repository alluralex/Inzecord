using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;

namespace MyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IsWebView2RuntimeInstalled();

            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;


            var env = CoreWebView2Environment.CreateAsync();

            //Если приложение не запускается - выведется ошибка в messagebox
            
            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (!e.IsSuccess)
                {
                    MessageBox.Show($"Ошибка инициализации WebView2: {e.InitializationException.Message}");
                }
            };
            webView.Source = new Uri("https://app.inzecord.ru/login.html");
        }

        //Проверка на то, что установлено ли у человека MicrosoftEdge
        bool IsWebView2RuntimeInstalled()
        {
            string runtimeKey = @"SOFTWARE\Microsoft\EdgeUpdate\Clients";
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runtimeKey))
            {
                return key != null && key.GetSubKeyNames().Length > 0;
            }
        }
        
        #region верхний тулбар
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
                default:
                    break;
            }
        }
        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Toolbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        #endregion


        //Если будет ошибка на стороне приложения, оно выведет её
        private async void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            else
            {
                MessageBox.Show("Ошибка инициализации WebView2: " + e.InitializationException.Message);
            }
        }

        //если загрузка сайта была успешна, то экран загрузки исчезнет и всё будет хорошо, в противном случае будет показана ошибка
        private void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                LoadingScreen.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Ошибка загрузки сайта: " + e.WebErrorStatus);
            }
        }
    }
}