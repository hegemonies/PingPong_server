using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_server {
    class Sessions {
        public Player Left { get; private set; }
        public Player Right { get; private set; }
        public int GID { get; } // Game id
        public SessionStatus status { get; set; }

        public Sessions(Player Left, Player Right, int GID) {
            this.Left = Left;
            this.Right = Right;
            this.GID = GID;
            status = SessionStatus.Free;
        }

        public void AddPlayer(Player newPlayer) {
            if (Left == null) {
                Left = newPlayer;
            } else if (Right == null) {
                Right = newPlayer;
            }
        }

        public string toString() {
            string head = null;
            if (Left == null ) {
                head += "FreePlace";
                head += ',';
                head += Right.nickName;
            } else if (Right == null) {
                head += Left.nickName;
                head += ',';
                head += "FreePlace";
            } else {
                head += Left.nickName + ',' + Right.nickName;
            }
            return head + ',' + GID + ',' + status;
        }
    }

    enum SessionStatus {
        Free,
        Busy
    }
}
