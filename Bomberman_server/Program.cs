using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong_server {
    //class Menu {
    //    List<Socket> list;
    //}
    class Program {
        static private int gamePort = 7902;
        static private int chatPort = 7903;
        //static private int n = 10;
        //static public List<byte[]> list = new List<byte[]>();
        static GameZone gamezone = new GameZone();
        static void Main(string[] args) {
            Console.WriteLine("Server up\t" + DateTime.Now);

            IPEndPoint ipPoint = null;
            Socket serv_socket = null;

            try {
                ipPoint = new IPEndPoint(IPAddress.Any, gamePort);
                serv_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            } catch (Exception exc) {
                Console.WriteLine("Error up game server: " + exc.Message);
            }

            IPEndPoint chatIpPoint = null;
            Socket chat_serv_socket = null;
            
            try {
                 chatIpPoint = new IPEndPoint(IPAddress.Any, chatPort);
                 chat_serv_socket  = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            } catch (Exception exc) {
                Console.WriteLine("Error up char server: " + exc.Message);
            }

            try {
                serv_socket.Bind(ipPoint);
                serv_socket.Listen(10);

                chat_serv_socket.Bind(chatIpPoint);
                chat_serv_socket.Listen(10);

                while (true) {
                    try {
                        Socket client = serv_socket.Accept();
                        Socket chatClient = chat_serv_socket.Accept();

                        Console.WriteLine("New connection from " + ((IPEndPoint)client.RemoteEndPoint).Address.ToString());

                        byte[] buf = new byte[20];
                        client.Receive(buf);
                        string nickName = Encoding.Default.GetString(buf);
                        nickName = Helper.DeleteSpaces(nickName);
                        Player player = new Player(client, chatClient, nickName, PlayerStatus.Smoker);

                        Console.WriteLine("{0} entered", nickName);

                        new Thread(gamezone.SmokeRoom).Start((object)player);
                    } catch {

                    }
                }
            } catch (Exception exc) {
                Console.WriteLine("Error: " + exc.Message);
            }
            Console.ReadKey();
        }

        private static bool ByteArrayCompareWithSequenceEqual(byte[] p_BytesLeft, byte[] p_BytesRight) {
            return p_BytesLeft.SequenceEqual(p_BytesRight);
        }
    }
}
