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
        private static NetworkStream clientNs;
        private DispatcherTimer dTimer = new DispatcherTimer();
        int NumBalls = 0;
        List<Ball> Balls = new List<Ball>();
        private static bool clientIsHere = false;

        public MainWindow()
        {
            InitializeComponent();
            Loop();

            clientPort = 50000;
            serverIp = IPAddress.Parse("127.0.0.1");

            TcpClient client = new TcpClient();
            client.Connect(serverIp, clientPort);

            if (client.Connected)
            {
                clientNs = client.GetStream();

                byte[] localBuffer = new byte[256];
                int idBytes = clientNs.Read(localBuffer, 0, localBuffer.Length);

                string receivedId = "";
                receivedId = Encoding.UTF8.GetString(localBuffer, 0, idBytes);
                playerId = Int32.Parse(receivedId);

                CreateBall();
                CreateSecondBall();

                Thread receivingThread = new Thread(receiveFromServer);
                receivingThread.Start(clientNs);

                Position positionToSend = new Position(Balls[0].PosX, Balls[0].PosY);
                ResponseToServer(positionToSend);
            }
        }

        void receiveFromServer(object clientNs)
        {
            NetworkStream current = (NetworkStream)clientNs;

            byte[] receivedBuffer = new byte[256];
            int bytesFromBuffer = current.Read(receivedBuffer, 0, receivedBuffer.Length);
            clientIsHere = true;
            
            Position otherPosition = Position.Deserialize(receivedBuffer) as Position;
            Balls[1].PosX = otherPosition.PosX;
            Balls[1].PosY = otherPosition.PosY;
            while (true)
            {
                byte[] localBuffer = new byte[256];
                int receivedBytes = current.Read(localBuffer, 0, localBuffer.Length);

                Position receivedPos = Position.Deserialize(localBuffer) as Position;
                Balls[1].PosX = receivedPos.PosX;
                Balls[1].PosY = receivedPos.PosY;
            }
        }

        void ResponseToServer(Position position)
        {
            byte[] positionToBytes = Position.Serialize(position);
            clientNs.Write(positionToBytes, 0, positionToBytes.Length);
        }

        public void CreateBall()
        {
            Color color = Colors.Green;

            if (playerId == 0)
            {
                color = Colors.Violet;
            }

            //Creem una bola
            Ball ball = new Game.Ball(200 + 200 * playerId, 325, 50, 50, color);

            //Dibuixem la bola al taulell
            CanvasBalls.Children.Add(ball.BallDraw.ShapeBall);
            DrawBall(ball);
            
            //Guardem informació de la bola
            Balls.Add(ball);
            NumBalls++;
        }

        public void CreateSecondBall()
        {
            Color color = Colors.Green;

            if (playerId != 0)
            {
                color = Colors.Violet;
            }

            Ball ball = new Ball(0, 0, 0, 0, color);

            CanvasBalls.Children.Add(ball.BallDraw.ShapeBall);
            DrawBall(ball);

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
            foreach (Ball ball in Balls)
            {
                DrawBall(ball);
            }
        }

        public void DrawBall(Ball infoBall)
        {
            Canvas.SetLeft(infoBall.BallDraw.ShapeBall, infoBall.PosX);
            Canvas.SetTop(infoBall.BallDraw.ShapeBall, infoBall.PosY);

            if (clientIsHere)
            {
                infoBall.BallDraw.ShapeBall.Height = 50;
                infoBall.BallDraw.ShapeBall.Width = 50;
            }
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
            ResponseToServer(position);
        }
    }
}

