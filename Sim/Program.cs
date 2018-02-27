using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Sim
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            string option;

            while (exit != true)
            {
                Console.WriteLine("Vols crear un nou jugador? (s/n) ");
                option = Console.ReadLine();

                if (option == "s")
                {
                    joinPlayer();
                }
                else
                {
                    exit = true;
                }
            }
        }

        static void joinPlayer()
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Users\Usuari1\Music\BallGame_Multiplayer-dev_javi\Game\bin\Debug\CatchGame.exe";
            process.Start();
        }
    }
}
