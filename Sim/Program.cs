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
                Console.WriteLine("Crear client? (s/n) ");
                option = Console.ReadLine();

                if (option == "s")
                {
                    crearClient();
                }
                else
                {
                    exit = true;
                }
            }
        }

        static void crearClient()
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Users\Usuari1\Documents\Game\Client\bin\Debug\Client.exe";
            process.Start();
        }
    }
}
