﻿using System;
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
                    Helper.DeleteSpaces(ref command);
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
                                    byte[] message = Encoding.Default.GetBytes(session.Right.nickName);
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

            Console.WriteLine("WOW {0} and {1} connected to game [{2}]", session.Left.nickName, session.Right.nickName, session.GID);

            session.Right.socket.Send(Encoding.Default.GetBytes(session.Left.nickName));
        }
    }
}