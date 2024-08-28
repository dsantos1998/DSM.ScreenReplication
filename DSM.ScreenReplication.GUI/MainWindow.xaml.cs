using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int Port = 12345; // Debe coincidir con el puerto de la aplicación de consola
        private const string ServerIp = "127.0.0.1"; // La dirección IP del servidor (localhost)

        public MainWindow()
        {
            InitializeComponent();
            ReceiveData();
        }

        private async void ReceiveData()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ServerIp, Port);
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    tbkTest.Text = $"Datos recibidos: {receivedData}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
