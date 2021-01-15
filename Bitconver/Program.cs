using System;

namespace BitConverterSample
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("Int and byte arrays conversion sample.");
            // Create double to a byte array  
            Int32 i32 = 111111112;
            Console.WriteLine("Int value: " + i32.ToString());
            byte[] bytes = ConvertInt32ToByteArray(i32);
            Console.WriteLine("Byte array value:");
            Console.WriteLine(BitConverter.ToString(bytes));
            Console.WriteLine("Byte array back to Int32:");
            // Create byte array to Int32  
            double dValue = ConvertByteArrayToInt32(bytes);
            Console.WriteLine(dValue.ToString());
            Console.ReadLine();
        }
        public static byte[] ConvertInt32ToByteArray(Int32 I32)
        {
            return BitConverter.GetBytes(I32);
        }
        public static byte[] ConvertIntToByteArray(Int16 I16)
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
        public static double ConvertByteArrayToInt32(byte[] b)
        {
            return BitConverter.ToInt32(b, 0);
        }
    }
}