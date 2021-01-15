using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Scanner scanner = new Scanner();

            Console.WriteLine("Waiting for client.");
            scanner.AcceptClient();
            Console.WriteLine("Client connected.");
            while (true)
            {
                scanner.Send(Console.ReadLine());
            }
        }
    }
}