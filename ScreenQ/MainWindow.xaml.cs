using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ScreenQ
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
        [In] IntPtr hWnd,
        [In] int id,
        [In] uint fsModifiers,
        [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        private TaskbarIcon tb;
        private bool enabled = true;

        public MainWindow()
        {
            InitializeComponent();
            this.Visibility = Visibility.Hidden;
            tb = (TaskbarIcon)FindResource("MyNotifyIcon");

            if(Convert.ToBoolean(Properties.Settings.Default["PopupIsChecked"]) == true)
            ShowStandardBalloon();
        }

        private void ShowStandardBalloon()
        {
            string title = "ScreenQ - Info";
            string text = "I'm running in the system tray.";
            tb.ShowBalloonTip(title, text, BalloonIcon.None);
            tb.HideBalloonTip();
        }

        private void CloseCommand(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public static BitmapSource capture;

        private void TakeScreenshot()
        {
            using (var screenBmp = new Bitmap(
            (int)SystemParameters.PrimaryScreenWidth,
            (int)SystemParameters.PrimaryScreenHeight,
            PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                    IntPtr hBitmap = screenBmp.GetHbitmap();
                    capture = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    DeleteObject(hBitmap);
                    Clipboard.SetImage(capture);
                }
            }
            if (Convert.ToBoolean(Properties.Settings.Default["PopupIsChecked"]) == true)
            {
                string title = "ScreenQ - Info";
                string text = "Screenshot taken!";
                tb.ShowBalloonTip(title, text, BalloonIcon.None);
                tb.HideBalloonTip();
            }

            var editWindow = new Edit();
            editWindow.Show();
            editWindow.Activate();

        }

        private void ToggleActive(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.OriginalSource;

            if (enabled == true)
            {
                enabled = false;
                menuItem.Header = "Enable";
                UnregisterHotKey();

            }
            else
            {
                enabled = true;
                menuItem.Header = "Disable";
                RegisterHotKey();
            }
        }

        private void ShowConfig(object sender, RoutedEventArgs e)
        {
            var configWindow = new Configuration();
            configWindow.Show();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_SNAPSHOT = 0x2C;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, 0, VK_SNAPSHOT))
            {
                // handle error
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            if(enabled == true)
            {
               TakeScreenshot();
            }
            
        }
    }

}
