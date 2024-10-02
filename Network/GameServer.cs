using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Network
{
    public class GameServer
    {
        private TcpListener _listener;
        private bool _isRunning;

        public void StartServer(string ipAddress, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _listener.Start();
            _isRunning = true;

            Console.WriteLine($"Server started on {ipAddress}:{port}");

            Thread serverThread = new Thread(new ThreadStart(HandleClients));
            serverThread.Start();
        }

        private void HandleClients()
        {
            while (_isRunning)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine("Client connected!");

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientCommunication));
                clientThread.Start(client);
            }
        }

        private void HandleClientCommunication(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                // Здесь мы можем обработать сообщение (например, перемещение игрока) и отправить обновлённое состояние игры

                // Отправляем сообщение обратно клиенту
                byte[] response = Encoding.ASCII.GetBytes("Server received your message");
                stream.Write(response, 0, response.Length);
            }

            client.Close();
            Console.WriteLine("Client disconnected");
        }

        public void StopServer()
        {
            _isRunning = false;
            _listener.Stop();
            Console.WriteLine("Server stopped");
        }
    }

}