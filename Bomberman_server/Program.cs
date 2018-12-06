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
        static private int self_port = 7902;
        //static private int n = 10;
        //static public List<byte[]> list = new List<byte[]>();
        static GameZone gamezone = new GameZone();
        static void Main(string[] args) {
            Console.WriteLine("Server up\t" + DateTime.Now);

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, self_port);
            Socket serv_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            try {
                serv_socket.Bind(ipPoint);
                serv_socket.Listen(10);

                while (true) {
                    try {
                        Socket client = serv_socket.Accept();

                        Console.WriteLine("New connection from " + ((IPEndPoint)client.RemoteEndPoint).Address.ToString());

                        byte[] buf = new byte[20];
                        client.Receive(buf);
                        string nickName = Encoding.Default.GetString(buf);
                        nickName = Helper.DeleteSpaces(nickName);
                        Player player = new Player(client, nickName, PlayerStatus.Smoker);

                        Console.WriteLine("{0} entered", nickName);


                        new Thread(gamezone.SmokeRoom).Start((object)player);

                        //new Thread(player.Turn).Start();

                        //while (true) {
                        //    Thread.Sleep(900);
                        //    Console.WriteLine(list.Count);
                        //    for (int i = 0; i < list.Count; i++) {
                        //        Console.Write(Encoding.Default.GetString(list.ElementAt(i)));
                        //    }
                        //    Console.WriteLine();
                        //    Console.WriteLine();
                        //}

                        //NetworkStream stream = new NetworkStream(client);

                        //byte[] message = new byte[n];
                        //stream.Read(message, 0, n * n);

                        //byte[] message = new byte[n * n];
                        //byte[] old_message = new byte[n * n];
                        //bool wasChange = true;
                        //long ticks = 0;

                        //while (client.Connected) {
                        //    Thread.Sleep(100);

                        //    stream.Read(message, 0, n * n);

                        //    wasChange = !ByteArrayCompareWithSequenceEqual(old_message, message);

                        //    if (wasChange) {
                        //        Console.Clear();

                        //        for (int i = 0; i < n; i++) {
                        //            byte[] tmp = new byte[n];
                        //            Array.Copy(message, i * n, tmp, 0, n);
                        //            Console.WriteLine(Encoding.Default.GetString(tmp));
                        //        }
                        //        Console.WriteLine();
                        //    }
                        //    Array.Copy(message, old_message, n * n);
                        //    Console.WriteLine(ticks);
                        //    ticks++;
                        //}

                        //Console.WriteLine("Receive message: " + Encoding.Default.GetString(message));

                        //    Console.WriteLine("End");

                        //    client.Shutdown(SocketShutdown.Both);
                        //    client.Close();
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
