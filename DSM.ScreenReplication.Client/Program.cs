using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSM.ScreenReplication.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const int port = 12345; // Puerto que usarás para la comunicación
            var listener = new TcpListener(IPAddress.Any, port);

            listener.Start();
            Console.WriteLine("Esperando conexión...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Cliente conectado.");

                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes("Hola desde la aplicación de consola");
                await stream.WriteAsync(data, 0, data.Length);

                client.Close();
            }
        }
    }
}
