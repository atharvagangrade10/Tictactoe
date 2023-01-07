using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Tictactoe
{
    public class server
    {
        public static Socket StartServer()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress iPAddress = host.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 5000);
            try
            {
                Socket listner = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listner.Bind(endPoint);
                listner.Listen(10);

                Console.WriteLine("Waiting for Connection.....");
                Socket handle = listner.Accept();
                Console.WriteLine("Client is connected");
                return handle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
        public static string RecieveMessage(Socket handle)
        {
            string data = null;
            byte[] bytes = new byte[1024];
            int bytesRecive = handle.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRecive);
            return data;
        }
        public static void SendMessage(Socket handle,string data)
        {
            byte[] msg = Encoding.ASCII.GetBytes(data);
            handle.Send(msg);
        }
        public static void CloseConnection(Socket handle)
        {
            handle.Shutdown(SocketShutdown.Both);
            handle.Disconnect(true);
            handle.Close();
        }

    }
}
