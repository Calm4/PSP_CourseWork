using System;
using System.Net.Sockets;
using System.Text;


namespace Network
{
    public class GameClient
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public void ConnectToServer(string serverIp, int port)
        {
            _client = new TcpClient(serverIp, port);
            _stream = _client.GetStream();

            Console.WriteLine($"Connected to server {serverIp}:{port}");
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);

            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Server response: {response}");
        }

        public void Disconnect()
        {
            _stream.Close();
            _client.Close();
            Console.WriteLine("Disconnected from server");
        }
    }

}