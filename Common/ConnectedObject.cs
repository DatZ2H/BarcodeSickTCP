using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    /// <summary>
    /// Wrapper for client connections
    /// </summary>
    public class ConnectedObject
    {
        #region Properties
        // Client socket
        public Socket Socket { get; set; }
        // Size of receive buffer
        public int BufferSize { get; set; } = 1024;
        // Receive buffer
        public int BarcodeLengthSize { get; set; } = 2;
        public byte[] Buffer { get; set; }
        public byte[] testArray { get; set; }

        // Tao ArrayList cho buffer

        public List<byte> BufferList { get; set; }

        public List<string> BufferFrameList { get; set; }


        // Received data string
        private StringBuilder IncomingMessage { get; set; }
        // Message to be sent
        private StringBuilder OutgoingMessage { get; set; }
        // Terminator for each message
        public string MessageTerminatorStart { get; set; } = "<START>";
        public string MessageTerminatorStop { get; set; } = "<STOP>";

        public List<byte> MsgTerminatorStart { get; set; }

        public List<byte> MsgTerminatorStop { get; set; }

        #endregion

        #region Constructors
        public ConnectedObject()
        {
            Buffer = new byte[BufferSize];
            BufferList = new List<Byte>();
            testArray = new byte[] { 0x09, 0x00,0x00,0x00};
            BufferFrameList = new List<string>();
            MsgTerminatorStart = new List<byte>() { 0x02, 0x3c, 0x53, 0x54, 0x41, 0x52, 0x54, 0x3e }; //"STX<START>"
            MsgTerminatorStop = new List<byte>() { 0x3c, 0x53, 0x54, 0x4f, 0x50, 0x3e, 0x03 };        //"<STOP>ETX"

            IncomingMessage = new StringBuilder();
            OutgoingMessage = new StringBuilder();
        }
        #endregion

        #region Outgoing Message Methods
        /// <summary>
        /// Converts the outgoing message to bytes
        /// </summary>
        /// <returns></returns>
        public byte[] OutgoingMessageToBytes()
        {

            return Encoding.ASCII.GetBytes(OutgoingMessage.ToString());
        }


        /// <summary>
        /// Creates a new outgoing message
        /// </summary>
        /// <param name="msg"></param>
        public void CreateOutgoingMessage(string msg)
        {
            OutgoingMessage.Clear();
            OutgoingMessage.Append(Encoding.ASCII.GetString(MsgTerminatorStart.ToArray(), 0, MsgTerminatorStart.Count));
            OutgoingMessage.Append(msg);
            OutgoingMessage.Append(Encoding.ASCII.GetString(MsgTerminatorStop.ToArray(), 0, MsgTerminatorStop.Count));
        }

        #endregion

        #region Incoming Message Methods
        /// <summary>
        /// Converts the buffer to a string ans stores it
        /// </summary>
        public void BuildIncomingMessage()
        {
           

            if (BufferList.Count > (MsgTerminatorStart.Count + BarcodeLengthSize + MsgTerminatorStop.Count)) {
                while (BufferList.Count>(MsgTerminatorStart.Count+MsgTerminatorStop.Count+BarcodeLengthSize)) { 
                
                bool IsMsgTerminatorStart = true;
                bool IsMsgTerminatorStop = true;
                for (int i = 0; i < MsgTerminatorStart.Count; i++)
                {
                    if (BufferList[i] != MsgTerminatorStart[i])
                    {
                        IsMsgTerminatorStart = false;
                        ClearBufferList(0, 1);
                        break;
                    } 
                }
                    
                    if (IsMsgTerminatorStart)
                    {
                        int BarcodeLength = 0;

                        BarcodeLength = ConvertByteArrayToInt16(BufferList.GetRange(MsgTerminatorStart.Count, BarcodeLengthSize).ToArray());
                        // Console.WriteLine($" gia tri cuar con ver la {ConvertByteArrayToInt32(BufferList.GetRange(MsgTerminatorStart.Count, BarcodeLengthSize).ToArray())} ");
                        int LengthToMsgStop = MsgTerminatorStart.Count + BarcodeLengthSize + BarcodeLength;

                        for (int i = LengthToMsgStop; i < (LengthToMsgStop + MsgTerminatorStop.Count); i++)
                        {
                            
                            if (BufferList[i] != MsgTerminatorStop[i - LengthToMsgStop])
                            {
                                IsMsgTerminatorStop = false;
                                ClearBufferList(0, i);
                                
                                break;

                            }
                        }
                        
                        if (IsMsgTerminatorStart && IsMsgTerminatorStop)
                        {
                            BufferFrameList.Add(Encoding.ASCII.GetString(BufferList.ToArray(), MsgTerminatorStart.Count + BarcodeLengthSize, (int)BarcodeLength));
                            ClearBufferList(0, (LengthToMsgStop + MsgTerminatorStop.Count));
                            
                        }

                    }
                }



            }

            
        }

        /// <summary>
        /// Determines if the message was fully received
        /// </summary>
        /// <returns></returns>
        public bool MessageReceived()
        {
            

            BufferFrameList.ForEach(Console.WriteLine);

            

            return IncomingMessage.ToString().IndexOf(MessageTerminatorStop) > -1;
        }

        /// <summary>
        /// Clears the current incoming message so that we can start building for the next message
        /// </summary>
        public void ClearIncomingMessage()
        {
            IncomingMessage.Clear();
        }
        public void ClearBufferList(int StartIndex, int Index)
        {
            BufferList.RemoveRange(StartIndex, Index);
        }
        public static byte[] ConvertInt32ToByteArray(Int32 I32)
        {
            return BitConverter.GetBytes(I32);
        }
        public static byte[] ConvertIntToByteArray16(Int16 I16)
        {
            return BitConverter.GetBytes(I16);
        }
        public static byte[] ConvertIntToByteArray(Int64 I64)
        {
            return BitConverter.GetBytes(I64);
        }
        public static byte[] ConvertIntToByteArray(int I)
        {
            return BitConverter.GetBytes(I);
        }
        public static int ConvertByteArrayToInt16(byte[] b)
        {
            return BitConverter.ToInt16(b, 0);
        }

        /// <summary>
        /// Gets the length of the incoming message
        /// </summary>
        /// <returns></returns>
        public int IncomingMessageLength()
        {
            return IncomingMessage.Length;
        }
        #endregion

        #region Connected Object Methods
        /// <summary>
        /// Closes the connection
        /// </summary>
        public void Close()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            catch (Exception)
            {
                System.Console.WriteLine("connection already closed");
            }
        }

        public string GetRemoteEndPoint()
        {
            return Socket.RemoteEndPoint.ToString();
        }

        /// <summary>
        /// Print the details of the current incoming message
        /// </summary>
        public void PrintMessage()
        {
            string divider = new String('=', 60);
            Console.WriteLine();
            Console.WriteLine(divider);
            Console.WriteLine("Message Received");
            Console.WriteLine(divider);
            Console.WriteLine($"Read {IncomingMessageLength()} bytes from socket.");
            Console.WriteLine($"Message: {IncomingMessage.ToString()}");
        }
        #endregion
    }
}