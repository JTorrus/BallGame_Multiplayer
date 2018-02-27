using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using PosLibrary;

namespace Server
{
    class Player
    {
        public int IdJugador { get; set; }
        public NetworkStream NetworkStream { get; set; }
        public Position Position { get; set; }

        public Player(int IdJugador, NetworkStream NetworkStream)
        {
            this.IdJugador = IdJugador;
            this.NetworkStream = NetworkStream;
        }
    }

    class Program
    {
        private static IPAddress ServerIp;
        private static int CurrentNumOfPlayers = 0;
        private static readonly object locker = new object();
        private static List<Player> players = new List<Player>();

        static void Main(string[] args)
        {
            int serverPort = 50000;
            ServerIp = IPAddress.Parse("127.0.0.1");

            TcpListener server = new TcpListener(ServerIp, serverPort);
            Console.WriteLine("Server has been created");

            server.Start();
            Console.WriteLine("Server started");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected");

                Thread clientThread = new Thread(ServerResponse);
                clientThread.Start(client);
            }
        }

        static void ServerResponse(Object o)
        {
            NetworkStream serverNs = null;
            TcpClient client = (TcpClient)o;

            try
            {
                serverNs = client.GetStream();
                string generatedIdString = GeneratePlayer(serverNs).ToString();

                byte[] idBytes = Encoding.UTF8.GetBytes(generatedIdString);

                serverNs.Write(idBytes, 0, idBytes.Length);
            } catch (Exception e)
            {
                serverNs.Close();
                client.Close();

                Console.WriteLine("Hi ha hagut un error amb la connexió");
            }

            if (serverNs != null)
            {
                while (true)
                {
                
                    byte[] localBuffer = new byte[256];
                    int receivedBytes = serverNs.Read(localBuffer, 0, localBuffer.Length);

                    Position receivedPosition;
                    receivedPosition = (Position) Position.Deserialize(localBuffer);

                    Console.WriteLine(receivedPosition.PosX);
                    Console.WriteLine(receivedPosition.PosY);

                    for (int i = 0; i < players.Count; i++)
                    {
                        Player player = players[i];

                        if (player.NetworkStream != serverNs)
                        {
                            player.NetworkStream.Write(localBuffer, 0, receivedBytes);
                        }
                    }
                }
            }
        }

        static int GeneratePlayer(NetworkStream clientNs)
        {
            Player player;

            lock (locker)
            {
                player = new Player(CurrentNumOfPlayers, clientNs);
                players.Add(player);
                CurrentNumOfPlayers++;
            }

            return player.IdJugador;
        }
    }
}
