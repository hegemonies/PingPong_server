using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_server {
    class Player {
        public string nickName { get; }
        public Socket socket { get; }
        public Socket chatSocket { get; }
        PlayerStatus status;

        public Player(Socket socket, Socket chatSocket, string nickName, PlayerStatus status) {
            this.socket = socket;
            this.chatSocket = chatSocket;
            this.nickName = nickName;
            this.status = status;
        }
    }

    enum PlayerStatus {
        Smoker,
        Player,
        Watcher
    }
}
