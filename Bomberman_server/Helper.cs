using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong_server {
    class Helper {
        public static void DeleteSpaces(ref string sstr) {
            int i = sstr.Length - 1;
            for (; !char.IsLetterOrDigit(sstr[i]) && !char.IsSymbol(sstr[i]); i--) { }

            i++;

            string new_str = string.Empty;
            for (int j = 0; j < i; j++) {
                new_str += sstr[j];
            }

            sstr = new_str;
        }

        public static int getRandomY() {
            Random rand = new Random();

            int y = rand.Next() % 3;

            if (y == 0) {
                y = -1;
            } else if (y == 1) {
                y = 0;
            } else if (y == 2) {
                y = 1;
            }

            return y;
        }
    }
}
