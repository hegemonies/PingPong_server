using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_server {
    class GameRules {
        int gameZoneWidth = 60;
        int gameZoneHeight = 24;
        int velocityX = 1;
        int velocityY = 1;
        int leftPlaceGoal = 0;
        int rightPlaceGoal = 59;

        public string Turn(int posLeft, int posRight, int[] posBall) {
            int x = posBall[0];
            int y = posBall[1];

            if (x == 0) {
                return "LEFTLOSE";
            } else if (x == 59) {
                return "RIGHTLOSE";
            } else if (x == 0 && y == posLeft - 1) {
                velocityY = Helper.getRandomY();
            } else if (x == 59 && y == posRight - 1) {
                velocityX = -1;
                velocityY = Helper.getRandomY();
            } else if (y == 0) {
                velocityY = -1;
            } else if (y == 23) {
                velocityY = 1;
            }

            x += velocityX;
            y += velocityY;

            posBall[0] = x;
            posBall[1] = y;

            return "OK";
        }
    }
}
