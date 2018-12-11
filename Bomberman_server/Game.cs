using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PingPong_server {
    class GameZone {
        List<Sessions> game_zone = new List<Sessions>();

        public void SmokeRoom(object _player) {
            Player player = (Player)_player;
            byte[] buffer = new byte[30];
            string[] answer = new string[2];
            string command = null;
            NetworkStream stream = new NetworkStream(player.socket);
            while (true) {
                Array.Clear(buffer, 0, buffer.Length);
                try {
                    stream.Read(buffer, 0, 30);
                } catch {
                    return;
                }

                answer = Encoding.Default.GetString(buffer).Split(';');
                command = answer[0];
                try {
                    command = Helper.DeleteSpaces(command);
                } catch {
                    return;
                }

                Console.WriteLine(command + ".");

                if (command == "GETLIST") {
                    Console.WriteLine("Send list");
                    if (game_zone.Capacity == 0) {
                        byte[] badAnswer = Encoding.Default.GetBytes("EMPTY");
                        stream.Write(badAnswer, 0, badAnswer.Length);
                    } else {
                        StringBuilder sb = new StringBuilder();
                        foreach (Sessions session in game_zone) {
                            sb.Append(session.toString());
                            if (session != game_zone.Last<Sessions>()) {
                                sb.Append(";");
                            }
                        }
                        string finishList = sb.ToString();
                        Console.WriteLine("Finish list: " + finishList);
                        byte[] buf = Encoding.Default.GetBytes(finishList);
                        stream.Write(buf, 0, buf.Length);
                    }
                } else if (command == "GOGAME") {
                    int GID = Int32.Parse(answer[1]);
                    if (GID <= 0) {
                        byte[] badAnswer = Encoding.Default.GetBytes("BAD");
                        stream.Write(badAnswer, 0, badAnswer.Length);
                    } else {
                        if (SessionFree(GID)) {
                            EnterToSession(player, GID);
                            foreach (Sessions session in game_zone) {
                                if (session.GID == GID) {
                                    byte[] message = Encoding.Default.GetBytes(session.Left.nickName);
                                    stream.Write(message, 0, message.Length);
                                }
                            }
                        } else {
                            byte[] badAnswer = Encoding.Default.GetBytes("BAD");
                            stream.Write(badAnswer, 0, badAnswer.Length);
                        }
                    }
                    break;
                } else if (command == "CREATEGAME") {
                    Console.WriteLine("creating the game");
                    int GID = AddSession(player, null);
                    PrepareToGame(player, GID);
                    break;
                }
            }

            stream.Close();
        }
        public int AddSession(Player left, Player right) {
            if (game_zone.Count > 20) {
                return 0;
            }

            int GID = 0;

            try {
                Random rand = new Random();
                bool unique = false;
                while (!unique) {
                    GID = rand.Next();
                    foreach (Sessions session in game_zone) {
                        if (session.GID == GID) {
                            continue;
                        }
                    }
                    unique = true;
                }

                game_zone.Add(new Sessions(left, right, GID));
            } catch (Exception exc) {
                Console.WriteLine("Error AddSession: " + exc.Message);
                return 0;
            }

            return GID;
        }
        public bool DeleteSession(int GID) {
            try {
                foreach (Sessions session in game_zone) {
                    if (session.GID == GID) {
                        game_zone.Remove(session);
                        break;
                    }
                }
            } catch (Exception exc) {
                Console.WriteLine("Error AddSession: " + exc.Message);
                return false;
            }
            return true;
        }
        private bool SessionFree(int GID) {
            foreach (Sessions session in game_zone) {
                if (session.GID == GID && session.status == SessionStatus.Free) {
                    return true;
                }
            }

            return false;
        }
        private void EnterToSession(Player new_player, int GID) {
            foreach (Sessions session in game_zone) {
                if (session.GID == GID) {
                    session.status = SessionStatus.Busy;
                    session.AddPlayer(new_player);
                }
            }
        }
        private Sessions GetSession(int GID) {
            foreach (Sessions session in game_zone) {
                if (session.GID == GID) {
                    return session;
                }
            }

            return null;
        }
        public void PrepareToGame(Player player, int GID) {
            Sessions session = GetSession(GID);
            while (session.status == SessionStatus.Free) {
                Thread.Sleep(1000);
            }

            Console.WriteLine("WOW {0} and {1} started the game [{2}]", session.Left.nickName, session.Right.nickName, session.GID);

            try {
                session.Left.socket.Send(Encoding.Default.GetBytes(session.Right.nickName));
            } catch {
                Console.WriteLine(session.Left.nickName + "severed connection");
            }

            StartGame(session);
        }
        public void StartGame(Sessions session) {
            new Thread(ChatUp).Start(session);
            NetworkStream streamLeft;
            NetworkStream streamRight;

            try {
                streamLeft = new NetworkStream(session.Left.socket);
                streamRight = new NetworkStream(session.Right.socket);
            } catch {
                return;
            }

            string readyLeft;
            string readyRight;
            byte[] rLeft = new byte[5];
            byte[] rRight = new byte[5];

            try {
                streamLeft.Read(rLeft, 0, 5);
                streamRight.Read(rRight, 0, 5);
            } catch { }

            readyLeft = Encoding.Default.GetString(rLeft);
            readyRight = Encoding.Default.GetString(rRight);

            if (readyLeft != "READY" || readyRight != "READY") {
                return;
            } else {
                streamLeft.Write(Encoding.Default.GetBytes("START"), 0, 5);
                streamRight.Write(Encoding.Default.GetBytes("START"), 0, 5);
            }

            int sizeClientBuffer = 2;
            int sizeServerBuffer = 24;

            var bufferLeft = new byte[sizeClientBuffer];
            var bufferRight = new byte[sizeClientBuffer];
            var sendBuffer = new byte[sizeServerBuffer];

            int startPosBallX = 28;
            int startPosBallY = 11;
            int[] PosBall = { startPosBallX, startPosBallY };

            int scoreLeft = 0;
            int scoreRight = 0;

            string sendString = null;
            var GR = new GameRules();
            int count = 0;
            while (scoreLeft < 5 && scoreRight < 5) {
                Array.Clear(bufferLeft, 0, bufferLeft.Length);
                Array.Clear(bufferRight, 0, bufferRight.Length);
                Array.Clear(sendBuffer, 0, sendBuffer.Length);

                //Thread.Sleep(510);

                try {
                    streamLeft.Read(bufferLeft, 0, sizeClientBuffer);
                    streamRight.Read(bufferRight, 0, sizeClientBuffer);
                } catch {
                    break;
                }


                string raw = Encoding.Default.GetString(bufferLeft);
                raw = Helper.DeleteSpaces(raw);
                int posLeft = int.Parse(raw);
                raw = Encoding.Default.GetString(bufferRight);
                int posRight = int.Parse(raw);

                string turnAction = GR.Turn(posLeft, posRight, PosBall);

                if (turnAction == "LEFTLOSE") {
                    scoreRight++;
                    PosBall[0] = startPosBallX;
                    PosBall[1] = startPosBallY;
                } else if (turnAction == "RIGHTLOSE") {
                    scoreLeft++;
                    PosBall[0] = startPosBallX;
                    PosBall[1] = startPosBallY;
                } else if (turnAction == "OK") {

                }

                sendString = "MOTION;" + 
                            posLeft + ";" + posRight + ";" + 
                            PosBall[0] + "," + PosBall[1] + ";" + 
                            scoreLeft + "," + scoreRight;

                //Console.WriteLine(count + " " + sendString);
                count++;

                sendBuffer = Encoding.Default.GetBytes(sendString);
                try {
                    streamLeft.Write(sendBuffer, 0, sendBuffer.Length);
                    streamRight.Write(sendBuffer, 0, sendBuffer.Length);
                } catch { }
            }

            if (scoreLeft == 5) {
                sendBuffer = Encoding.Default.GetBytes("ACTION;LOSE");
                streamRight.Write(sendBuffer, 0, sendBuffer.Length);

                sendBuffer = Encoding.Default.GetBytes("ACTION;WIN");
                streamLeft.Write(sendBuffer, 0, sendBuffer.Length);
            } else if (scoreRight == 5) {
                sendBuffer = Encoding.Default.GetBytes("ACTION;LOSE");
                streamLeft.Write(sendBuffer, 0, sendBuffer.Length);

                sendBuffer = Encoding.Default.GetBytes("ACTION;WIN");
                streamRight.Write(sendBuffer, 0, sendBuffer.Length);
            }

            streamLeft.Close();
            streamRight.Close();
        }

        private void ChatUp(object _session) {
            new Thread(ChatHandler).Start(_session);

            Sessions session = (Sessions)_session;

            int sizeChatMsg = 100;
            byte[] chatBuffer = new byte[sizeChatMsg];

            try {
                while (true) {
                    Array.Clear(chatBuffer, 0, sizeChatMsg);
                    session.Left.chatSocket.Receive(chatBuffer);
                    session.Right.chatSocket.Send(chatBuffer);
                }
            } catch {

            }
        }
        private void ChatHandler(object _session) {
            Sessions session = (Sessions)_session;

            int sizeChatMsg = 100;
            byte[] chatBuffer = new byte[sizeChatMsg];

            try {
                while (true) {
                    Array.Clear(chatBuffer, 0, sizeChatMsg);
                    session.Right.chatSocket.Receive(chatBuffer);
                    session.Left.chatSocket.Send(chatBuffer);
                }
            } catch {

            }
        }
    }
}
