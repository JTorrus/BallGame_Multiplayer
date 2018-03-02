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
    // Creem la classe Player on li possem com a paràmetres un Id, un NetworkStream i una Posició associada
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
            // Iniciem el servidor amb una Ip i un Port
            int serverPort = 50000;
            ServerIp = IPAddress.Parse("127.0.0.1");

            TcpListener server = new TcpListener(ServerIp, serverPort);
            Console.WriteLine("Server has been created");

            server.Start();
            Console.WriteLine("Server started");

            // Creem un bucle infinit com a listener per acceptar connexions entrants de clients (Game)
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected");

                // Iniciem un fil que s'encarregarà de enviar les dades corresponents a cada jugador que estigui connectat mitjançant el mètode ServerResponse
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
                // Primerament generem un objecte jugador associat al client que es connecti i li enviem les dades generades, en aquest cas, la seva Id associada
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
                    // Si el NetworkStream del server no és null, creem un bucle infinit que s'encarrega d'anar escoltant dades dels jugadors connectats i reenviar-les a tots els altres
                    byte[] localBuffer = new byte[256];
                    int receivedBytes = serverNs.Read(localBuffer, 0, localBuffer.Length);

                    // Les dades que envia un Jugador serà la posició de la seva bola, si ho volem mostrar per la consola del server, necessitem Deserialitzar l'objecte rebut i accedir als seus atributs
                    Position receivedPosition;
                    receivedPosition = (Position) Position.Deserialize(localBuffer);

                    Console.WriteLine(receivedPosition.PosX);
                    Console.WriteLine(receivedPosition.PosY);

                    // Per reenviar la posició als altres jugadors excepte el client origen, fem un recorregut de la llista de Jugadors i enviem la posició a tots els jugadors que tinguin un
                    // NetworkStream diferent al de l'origen
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
                // La id del jugador dependrà de la quantitat de jugadors que hi han 
                // actualment a la llista de Jugadors, això ho controlem amb una variable que s'encarrega de incrementar un valor quan un Jugador es afegit a la llista
                player = new Player(CurrentNumOfPlayers, clientNs);
                players.Add(player);
                CurrentNumOfPlayers++;
            }

            return player.IdJugador;
        }
    }
}
