using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";

            Client client = new Client();

            client.start();
            client.connecttoserver();

            Console.ReadKey();
        }
    }
    class Client
    {
        public static Client instance;
        public static int databuffersize = 4096; //4 MB
        public static int counter = 0;

        public string ip = "127.0.0.1";
        public int port = 38532;
        public int muid = 0;
        public TCP tcp;

        public void start()
        {
            if (instance == null)
            {
                instance = this;
            }

            tcp = new TCP();
        }
        public void connecttoserver()
        {
            tcp.connect();
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private byte[] buffer;

            public void connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = databuffersize,
                    SendBufferSize = databuffersize,
                };

                buffer = new byte[databuffersize];
                socket.BeginConnect(instance.ip, instance.port, connectcallback, socket);
            }
            private void connectcallback(IAsyncResult _resault)
            {
                socket.EndConnect(_resault);
                if (socket.Connected == false)
                {
                    return;
                }

                stream = socket.GetStream();
                stream.BeginRead(buffer, 0, buffer.Length, readcallback, null);
            }
            
            private void readcallback(IAsyncResult _resault)
            {
                try
                {
                    int _bytecount = stream.EndRead(_resault);
                    if (_bytecount <= 0)
                    {
                        //disconnect
                        return;
                    }

                    byte[] _data = new byte[_bytecount];
                    Array.Copy(buffer, _data, _bytecount);

                    //Data Hnadeling
                    Console.WriteLine($"Server said {Encoding.ASCII.GetString(_data)}");

                    stream.BeginRead(buffer, 0, databuffersize, readcallback, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exaeption {e} occured !");
                }
            }
        }
    }
}