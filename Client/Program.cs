using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using PosLibrary;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace Client
{
    class Program
    {
        private static IPAddress serverIp;
        private static int clientPort;
        private static int playerId;
        private static Position position;
        
        static void Main(string[] args)
        {
            clientPort = 50000;
            serverIp = IPAddress.Parse("127.0.0.1");

            TcpClient client = new TcpClient();
            client.Connect(serverIp, clientPort);

            if (client.Connected) 
            {
                Console.WriteLine("Connected");
                NetworkStream clientNs = client.GetStream();

                byte[] localBuffer = new byte[256];
                int idBytes = clientNs.Read(localBuffer, 0, localBuffer.Length);

                string receivedId = "";
                receivedId = Encoding.UTF8.GetString(localBuffer, 0, idBytes);
                playerId = Int32.Parse(receivedId);

                Thread receivingThread = new Thread(receiveFromServer);
                Thread responsingThread = new Thread(responseToServer);

                receivingThread.Start(clientNs);
                responsingThread.Start(clientNs);
            }
        }

        static void receiveFromServer(object clientNs)
        {
            NetworkStream current = (NetworkStream)clientNs;

            while (true)
            {
                byte[] localBuffer = new byte[256];
                int receivedBytes = current.Read(localBuffer, 0, localBuffer.Length);

                String receivedFrase = "";
                receivedFrase = Encoding.UTF8.GetString(localBuffer, 0, receivedBytes);

                Console.WriteLine(receivedFrase);
            } 
        }

        static void responseToServer(object clientNs)
        {
            NetworkStream current = (NetworkStream)clientNs;

            position = new Position(2 * playerId + 10, 50);
            
            byte[] fraseToBytes = Position.Serialize(position);
            current.Write(fraseToBytes, 0, fraseToBytes.Length);

        }
    }
}
