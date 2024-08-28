using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DSM.ScreenReplication.Client.Models;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO.Compression;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Encoder = System.Drawing.Imaging.Encoder;

namespace DSM.ScreenReplication.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const int port = 12345;
            var listener = new TcpListener(IPAddress.Any, port);

            listener.Start();
            Console.WriteLine("Waiting connection...");

            while (true)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Client connected.");

                    // Object to send / receive data
                    NetworkStream stream = client.GetStream();

                    // Get data
                    byte[] data = SendCaptureImage();

                    // Send data
                    await stream.WriteAsync(data, 0, data.Length);

                    Console.WriteLine($"Data sent. Lenght: {data.Length}");

                    client.Close();
                }
                catch (Exception ex)
                {
                    string trace = ex.InnerException != null ? ex.InnerException.StackTrace : ex.Message;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(trace);
                }

                Thread.Sleep(1000 / 30);
            }
        }

        private static byte[] SendCaptureImage()
        {
            PrintScreen printScreen = new PrintScreen();
            Bitmap myBitmap = new Bitmap(printScreen.CaptureScreen());

            ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders().Where(item => item.MimeType == "image/jpeg").First();
            EncoderParameters encoderParameters = new EncoderParameters(2);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
            //encoderParameters.Param[1] = new EncoderParameter(Encoder.Quality, 100L);
            encoderParameters.Param[1] = new EncoderParameter(Encoder.Quality, 30L);

            using (MemoryStream originalStream = new MemoryStream())
            {
                myBitmap.Save(originalStream, encoder, encoderParameters);

                return CompressMemoryStreamToByteArray(originalStream);
            }
        }

        private static byte[] CompressMemoryStreamToByteArray(MemoryStream memoryStream)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    memoryStream.Position = 0;
                    memoryStream.CopyTo(gzipStream);
                }

                return compressedStream.ToArray();
            }
        }
    }
}
