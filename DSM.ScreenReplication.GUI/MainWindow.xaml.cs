using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSM.ScreenReplication.GUI
{
    public partial class MainWindow : Window
    {
        private const int Port = 12345;
        private const string ServerIp = "127.0.0.1";

        public MainWindow()
        {
            InitializeComponent();
            ReceiveData();
        }

        private async void ReceiveData()
        {
            try
            {
                while (true)
                {
                    using (TcpClient client = new TcpClient())
                    {
                        await client.ConnectAsync(ServerIp, Port);

                        NetworkStream stream = client.GetStream();

                        byte[] buffer = new byte[5000000];

                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);

                        imgScreen.Source = ByteArrayToImageSource(receivedData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ImageSource ByteArrayToImageSource(byte[] imageData)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(imageData))
            {
                stream.Seek(0, SeekOrigin.Begin);
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            return bitmap;
        }
    }
}
