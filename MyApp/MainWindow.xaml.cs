﻿using System.Runtime.InteropServices;
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
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.IO;

namespace MyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Mutex _mutex;

        string appName = "Inzecord";
        string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        public MainWindow()
        {
            InitializeComponent();

            StartLoadingAnimation();

            IsWebView2RuntimeInstalled();

            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;


            var env = CoreWebView2Environment.CreateAsync();

            //Если приложение не запускается - выведется ошибка в messagebox

            webView.CoreWebView2InitializationCompleted += (sender, args) =>
            {
                if (args.IsSuccess)
                {
                    var coreWebView2 = webView.CoreWebView2;

                    // Отключить DevTools
                    coreWebView2.Settings.AreDevToolsEnabled = false;

                    // Скрыть статусную строку
                    coreWebView2.Settings.IsStatusBarEnabled = false;

                    // Заблокировать изменение масштаба
                }
                else
                {
                    MessageBox.Show("Ошибка инициализации WebView2!");
                }
            };
            webView.Source = new Uri("https://app.inzecord.ru/channels/@me");
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


        //Анимация кручения загрузки
        private void StartLoadingAnimation()
        {
            // Создать анимацию вращения
            var rotationAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(3),
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Запустить анимацию
            LoadingRotate.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, rotationAnimation);
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            // Если окно скрыто, показываем его
            if (this.Visibility != Visibility.Visible)
            {
                this.Show();
                this.WindowState = WindowState.Normal; // Восстанавливаем, если свернуто
            }

            // Делаем окно активным
            this.Activate();
        }

        #region верхний тулбар
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
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
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else if (WindowState == WindowState.Normal && e.ClickCount == 2)
                {
                    WindowState = WindowState.Maximized;
                }
                DragMove();
            }
        }
        #endregion


        //Если будет ошибка на стороне приложения, оно выведет её
        private async void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
                fadeOut.Completed += (s, _) =>
                {
                    LoadingScreen.Visibility = Visibility.Collapsed;
                    webView.Visibility = Visibility.Visible;
                };
                LoadingScreen.BeginAnimation(UIElement.OpacityProperty, fadeOut);
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

        private void OpenApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AutoAppPlay_Click(object sender, RoutedEventArgs e)
        {
            EnableAutoStart(appName, appPath);
        }

        public static void EnableAutoStart(string appName, string appPath)
        {
            // Открываем ключ реестра для автозагрузки текущего пользователя
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                // Если ключ существует, добавляем приложение
                if (key != null)
                {
                    key.SetValue(appName, appPath);
                }
            }
        }

        public static void DisableAutoStart(string appName)
        {
            // Открываем ключ реестра для автозагрузки текущего пользователя
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                // Если ключ существует, удаляем приложение
                if (key != null && key.GetValue(appName) != null)
                {
                    key.DeleteValue(appName);
                }
            }
        }
        private void Anti_AutoAppPlay_Click(object sender, RoutedEventArgs e)
        {
            DisableAutoStart(appName);
        }
    }
}