using System;

using System.IO;
using System.Net;
using Common;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ScannerSimulator
{
    
    public class Scanner
    {
        public ConnectedObject BarcodeSimulator;
        TcpListener _listener = new TcpListener(IPAddress.Parse("192.168.1.23"), 2112);
        TcpClient _client;
        NetworkStream _stream;
        StreamWriter _writer;
        StreamReader _reader;

        public Scanner()
        {
            _listener.Start();
        }

        public void AcceptClient()
        {
            _client = _listener.AcceptTcpClient();
            _stream = _client.GetStream();
            _writer = new StreamWriter(_stream);
            _reader = new StreamReader(_stream);
            Task.Run(() => Read());
        }

        public void Send(string data)
        {
            BarcodeSimulator = new ConnectedObject();
            byte[] barcodelength = ConnectedObject.ConvertIntToByteArray16((short)data.Length);

            _writer.Write(Encoding.ASCII.GetString(BarcodeSimulator.MsgTerminatorStart.ToArray(), 0, BarcodeSimulator.MsgTerminatorStart.Count));
            _writer.Write(Encoding.ASCII.GetString(barcodelength, 0,barcodelength.Length));
            _writer.Write(data);
            _writer.Write(Encoding.ASCII.GetString(BarcodeSimulator.MsgTerminatorStop.ToArray(), 0, BarcodeSimulator.MsgTerminatorStop.Count));

            _writer.Flush();
        }

        public void Read()
        {
            while (true)
            {
                Console.WriteLine("Readed: {0}", _reader.ReadLine());
            }
        }
    }
}