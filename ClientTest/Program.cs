using Common;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Program
    {

        static void Main(string[] args)
        {
            ClientTest Barcode = new ClientTest();
            Console.Title = "BarcodeClient ";
            Barcode.Connect();

            Console.ReadLine();

        }

        /// <summary>
        /// Starts multiple instances of this app
        /// </summary>


        /// <summary>
        /// Attempts to connect to a server
        /// </summary>

    }
}