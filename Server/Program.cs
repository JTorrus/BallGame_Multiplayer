using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Server
{
    class Player
    {
        private int idJugador { get; set; }
        private NetworkStream networkStream { get; set; }

        public Player(int idJugador, NetworkStream networkStream)
        {
            this.idJugador = idJugador;
            this.networkStream = networkStream;
        }
    }

    class Program
    {
        private static IPAddress serverIp;
        private static int currentNumOfPlayers;
        private static readonly object locker; //TODO: Hacer bien la instancia del locker

        static void Main(string[] args)
        {
            int serverPort = 50000;
            serverIp = IPAddress.Parse("127.0.0.1");

            TcpListener server = new TcpListener(serverIp, serverPort);
            Console.WriteLine("Server has been created");

            server.Start();
            Console.WriteLine("Server started");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected");

                Thread clientThread = new Thread(serverResponse);
                clientThread.Start(client);
            }
        }

        static void serverResponse(Object o)
        {
            TcpClient client = (TcpClient)o;

            while (true)
            {
                NetworkStream serverNs = null;

                try
                {
                    serverNs = client.GetStream();
                    //TODO: Enviar respuesta al cliente (punto 4 hacia delante)
                }
            }
        }

        static int generatePlayer()
        {
            // TODO: Generar datos del jugador
            Player player = new Server.Player()
        }
    }
}
