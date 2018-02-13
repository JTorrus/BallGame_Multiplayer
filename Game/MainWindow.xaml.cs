using PosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game
{
    public class Ball
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public BallGraphics BallDraw;

        public Ball (int posX, int posY, int width, int height, Color color)
        {
            PosX = posX;
            PosY = posY;

            BallDraw = new BallGraphics(width, height, color);
        }
    }

    public class BallGraphics
    {
        public Ellipse ShapeBall { get; set; }
        public Color Color { get; set; }

        public BallGraphics(int width, int height, Color color)
        {
            ShapeBall = new Ellipse();
            ShapeBall.Width = width;
            ShapeBall.Height = height;
            Color = color;
            ShapeBall.Fill = new SolidColorBrush(Color);

        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPAddress serverIp;
        private int clientPort;
        private int playerId;
        private Position position;
        private NetworkStream clientNs;
        private DispatcherTimer dTimer = new DispatcherTimer();
        int NumBalls = 0;
        List<Ball> Balls = new List<Ball>();

        public MainWindow()
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

            InitializeComponent();
            
            CreateBall();
            Loop();
        }

        void receiveFromServer(object clientNs)
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

        void responseToServer(object clientNs)
        {
            NetworkStream current = (NetworkStream)clientNs;

            position = new Position(2 * playerId + 10, 50);

            byte[] fraseToBytes = Position.Serialize(position);
            current.Write(fraseToBytes, 0, fraseToBytes.Length);

        }

        public void CreateBall()
        {
            //Creem una bola
            Ball ball = new Game.Ball(200, 325, 50, 50, Colors.Red);

            //Dibuixem la bola al taulell
            CanvasBalls.Children.Add(ball.BallDraw.ShapeBall);
            DrawBall(ball);
            
            //Guardem informació de la bola
            Balls.Add(ball);
            NumBalls++;
        }

        public void Loop()
        {
            dTimer.Interval = TimeSpan.FromMilliseconds(30);
            dTimer.Tick += Timer_Tick;
            dTimer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            DrawBall(Balls[0]);
        }

        public void DrawBall(Ball infoBall)
        {
            Canvas.SetLeft(infoBall.BallDraw.ShapeBall, infoBall.PosX);
            Canvas.SetTop(infoBall.BallDraw.ShapeBall, infoBall.PosY);
        }


        void CanvasKeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.D:
                    Balls[0].PosX= Balls[0].PosX + 4;
                    break;
                case Key.A:
                    Balls[0].PosX = Balls[0].PosX - 4;
                    break;
                case Key.X:
                    Balls[0].PosY = Balls[0].PosY + 4;
                    break;
                case Key.W:
                    Balls[0].PosY = Balls[0].PosY - 4;
                    break;
                default:                    
                    break;
            }

            Position position = new Position(Balls[0].PosX, Balls[0].PosY);
            //Position.;

            //TODO ENVIAR SERIALITZADA LA POSICIÓ AL SERVER

            //clientNs.Write( 0)
            
        }
    }
}

