using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";

            Server.Start(10, 38532);

            Console.ReadKey();
        }
    }
    static class Server
    {
        public static int MaxClient { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        private static TcpListener? listener;

        public static void Start(int _maxclients, int _port)
        {
            MaxClient = _maxclients;
            Port = _port;

            Console.WriteLine("Server Starting ...");

            DictSetup();
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(connectcallback), null);

            Console.WriteLine($"Server Started on: {Port}");
        }
        private static void connectcallback(IAsyncResult _resault)
        {
            TcpClient client = listener.EndAcceptTcpClient(_resault);
            listener.BeginAcceptTcpClient(new AsyncCallback(connectcallback), null);

            Console.WriteLine($"Income connnection from {client.Client.RemoteEndPoint}");
            for (int s = 1; s <= MaxClient; s++)
            {
                if (Clients[s].tcp.socket == null)
                {
                    Clients[s].tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine($"Server full faild to connect {client.Client.RemoteEndPoint} ...");
        }
        private static void DictSetup()
        {
            for (int s = 1; s <= MaxClient; s++)
            {
                Clients.Add(s, new Client(s));
            }
        }
    }
    class Client
    {
        public static int databuffersize = 4096; //4 MB
        public int id;
        public TCP tcp;

        public Client(int _id)
        {
            id = _id;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient? socket;

            private readonly int id;
            private NetworkStream? stream;
            private byte[]? buffer;
            private byte[]? sendbuffer;

            public TCP(int _id)
            {
                id = _id;
            }
            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = databuffersize;
                socket.SendBufferSize = databuffersize;

                stream = socket.GetStream();

                buffer = new byte[databuffersize];

                stream.BeginRead(buffer, 0, databuffersize, readcallback, null);
            }
            private void readcallback(IAsyncResult _result)
            {
                try
                {
                    int _bytecount = stream.EndRead(_result);
                    if (_bytecount <= 0)
                    {
                        //disconnect
                        return;
                    }

                    byte[] _data = new byte[_bytecount];
                    Array.Copy(buffer, _data, _bytecount);

                    //Data Hnadeling
                    Console.WriteLine($"{socket.Client.RemoteEndPoint} say {Encoding.ASCII.GetString(_data)}");

                    stream.BeginRead(buffer, 0, databuffersize, readcallback, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exaeption {e} occured !");
                }
            }
        }
    }
    class ServerSend
    {

    }
    class Packet
    {
        public static string MSG_string;
        public static byte[] MSG_byte;

        public static byte[] packet;
        public static int bytelenght { get; }

        private static int databuffersize;
        
        public Packet(int _databuffersize)
        {
            databuffersize = _databuffersize;
        }


        public static void Setpacket(string _string)
        {
            packet = new byte[databuffersize];
            packet = Encoding.ASCII.GetBytes(_string);
        }
        public static void GetPacket(string _output)
        {
            switch (_output)
            {
                case "byte":
                    MSG_byte = packet;
                    break;
                case "string":
                    MSG_string = Encoding.ASCII.GetString(packet);
                    break;
            }
        }

        public static string ToString(byte[] _bytes)
        {
            try
            {
                return Encoding.ASCII.GetString(_bytes);
            }
            catch (Exception e)
            {
                throw new Exception($"Sth happend... {e}");
            }
        }
        public static byte[] Tobyte(string _string)
        {
            try
            {
                return Encoding.ASCII.GetBytes(_string);
            }
            catch(Exception e)
            {
                throw new Exception($"sth happend... {e}");
            }
        }
    }
}