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
        int velocityY = 0;
        int leftPlaceGoal = 0;
        int rightPlaceGoal = 59;
        int topLine = 0;
        int bottomLine = 22;

        public string Turn(int posLeft, int posRight, int[] posBall) {
            int x = posBall[0];
            int y = posBall[1];

            if (x == leftPlaceGoal) {
                velocityX = Helper.getRandomX();
                velocityY = Helper.getRandomY();
                return "LEFTLOSE";
            } else if (x == rightPlaceGoal) {
                velocityX = Helper.getRandomX();
                velocityY = Helper.getRandomY();
                return "RIGHTLOSE";
            } else if ((x == leftPlaceGoal + 1) && (y == posLeft)) {
                velocityX = 1;
                velocityY = Helper.getRandomY();
            } else if ((x == rightPlaceGoal - 1) && (y == posRight)) {
                velocityX = -1;
                velocityY = Helper.getRandomY();
            } else if (y == topLine) {
                velocityY = 1;
            } else if (y == bottomLine) {
                velocityY = -1;
            } else if ((x == rightPlaceGoal - 1) && (y == posRight - 1)) {
                velocityX = -1;
                velocityY = Helper.getRandomY();
            } else if ((x == rightPlaceGoal - 1) && (y == posRight + 1)) {
                velocityX = -1;
                velocityY = Helper.getRandomY();
            } else if ((x == leftPlaceGoal + 1) && (y == posLeft - 1)) {
                velocityX = 1;
                velocityY = Helper.getRandomY();
            } else if ((x == leftPlaceGoal + 1) && (y == posLeft + 1)) {
                velocityX = 1;
                velocityY = Helper.getRandomY();
            }
            //else if ((velocityX == -1) && (velocityY == -1) && (posLeft == y - 1) && (x == leftPlaceGoal + 1)) {
            //    velocityX = 1;
            //    velocityY = Helper.getRandomY();
            //} else if ((velocityX == -1) && (velocityY == 1) && (posLeft == y + 1) && (x == leftPlaceGoal + 1)) {
            //    velocityX = 1;
            //    velocityY = Helper.getRandomY();
            //} else if ((velocityX == 1) && (velocityY == -1) && (posRight == y - 1) && (x == rightPlaceGoal - 1)) {
            //    velocityX = -1;
            //    velocityY = Helper.getRandomY();
            //} else if ((velocityX == 1) && (velocityY == 1) && (posRight == y + 1) && (x == rightPlaceGoal - 1)) {
            //    velocityX = -1;
            //    velocityY = Helper.getRandomY();
            //}

            x += velocityX;
            y += velocityY;

            posBall[0] = x;
            posBall[1] = y;

            return "OK";
        }
    }
}
