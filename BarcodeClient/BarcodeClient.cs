using System;
using Common;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace Client
{
    class BarcodeClient
    {
        private ConnectedObject client;

        public void Connect()
        {
            client = new ConnectedObject();
            // Create a new socket
            client.Socket = ConnectionManager.CreateSocket();
            int attempts = 0;

            // Loop until we connect (server could be down)
            while (!client.Socket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);

                    // Attempt to connect
                    client.Socket.Connect(ConnectionManager.EndPoint);
                }
                catch (SocketException)
                {

                    Console.Clear();
                }
            }

            // Display connected status
            Console.Clear();
            PrintConnectionState($"Socket connected to {client.Socket.RemoteEndPoint.ToString()}");

            // Start sending & receiving
            //Thread sendThread = new Thread(() => Send());
            //Thread receiveThread = new Thread(() => Receive(client));
            Thread CheckIncomingMessageThread = new Thread(() => CheckIncomingMessage());
             Thread TriggerThread = new Thread(() => Trigger());

            //sendThread.Start();
            //receiveThread.Start();
            TriggerThread.Start();

            Receive();
            // Console.WriteLine($"Gia tri cua Buffer list   {client.BufferList.Count}");
            CheckIncomingMessageThread.Start();



            // Listen for threads to be aborted (occurs when socket looses it's connection with the server)
            //while (sendThread.IsAlive && receiveThread.IsAlive) { }


            // Attempt to reconnect
            //Connect();
        }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <param name="client"></param>
        public void Send(String MsgSend)
        {
            // Build message
            client.CreateOutgoingMessage(MsgSend);
            byte[] data = client.OutgoingMessageToBytes();

            // Send it on a 1 second interval



            Thread.Sleep(3000);
            try
            {
                client.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
            }
            catch (SocketException)
            {
                Console.WriteLine("Server Closed");
                client.Close();
                Thread.CurrentThread.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.CurrentThread.Abort();
            }

        }

        /// <summary>
        /// Message sent handler
        /// </summary>
        /// <param name="ar"></param>
        public void SendCallback(IAsyncResult ar)
        {
            Console.WriteLine("Message Sent");
        }

        private void Receive()
        {

            try
            {
                client.Socket.BeginReceive(client.Buffer, 0, client.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            }
            catch (SocketException)
            {
                // Client was forcebly closed on the client side
                //CloseClient(client);

                Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private  void ReceiveCallback(IAsyncResult ar)
        {
            int bytesRead;

            // Read message from the client socket
            try
            {
                bytesRead = client.Socket.EndReceive(ar);

                // todo
                for (int i = 0; i < bytesRead; i++)
                {
                    client.BufferList.Add(client.Buffer[i]);
                }


                client.Socket.BeginReceive(client.Buffer, 0, client.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            }
            catch (SocketException)
            {
                // Client was forcebly closed on the client side
                //  CloseClient(client);
                //CloseClient(client);

                Connect();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }



        }
        private  void CheckIncomingMessage()
        {

          
            while (client.Socket.Connected)
            {
                if (client.BufferList.Count > (client.MsgTerminatorStart.Count + client.BarcodeLengthSize + client.MsgTerminatorStop.Count))
                {
                   
                    client.BuildIncomingMessage();
                    client.MessageReceived();
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }
        public void Trigger()
        {

            while (true)
            {



                Thread.Sleep(3000);
                Console.WriteLine("Trigger off");
                Send(client.MsgTriggerOFF);
                try
                {
                    Send(client.MsgTriggerON);
                    Console.WriteLine("Trigger pedding");
                }
                catch (SocketException)
                {
                    Console.WriteLine("Server Closed");
                    client.Close();
                    Thread.CurrentThread.Abort();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.CurrentThread.Abort();
                }
                Console.WriteLine("Trigger ON");
            }
        }


        /// <summary>
        /// Prints connection 'connected' or 'disconnected' states
        /// </summary>
        /// <param name="msg"></param>
        public static void PrintConnectionState(string msg)
        {
            string divider = new String('*', 60);
            Console.WriteLine();
            Console.WriteLine(divider);
            Console.WriteLine(msg);
            Console.WriteLine(divider);
        }
    }
}
