using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_server {
    class Player {
        //List<byte[]> gameData;
        public string nickName { get; }
        public Socket socket { get; }
        PlayerStatus status;

        public Player(Socket socket, string nickName, PlayerStatus status) {
            this.socket = socket;
            this.nickName = nickName;
            this.status = status;
        }
        /*
        public void Turn() {
            while (true) {
                //Thread.Sleep(1000);
                NetworkStream stream = new NetworkStream(socket);
                //Random rand = new Random();
                //list.Add(Encoding.Default.GetBytes(String.Concat(Enumerable.Repeat("a", rand.Next()))));
                byte[] buffer = new byte[10];
                stream.Read(buffer, 0, 10);
                gameData.Add(buffer);
            }
        }
        */
        //public void setData()

        public void Receive(byte[] buffer) {
            socket.Receive(buffer);
        }

        public void Send() {
        }
    }

    enum PlayerStatus {
        Smoker,
        Player,
        Watcher
    }
}
