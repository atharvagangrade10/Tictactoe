using System;
using System.Net.Sockets;
using System.Threading;
using Tictactoe;

namespace TIC_TAC_TOE
{
    using client;
    class Program
    {
        //making array and
        //by default I am providing 0-9 where no use of zero
        static char[] arr = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static int player = 1; //By default player 1 is set
        static int choice; //This holds the choice at which position user want to mark
        // The flag variable checks who has won if it's value is 1 then someone has won the match
        //if -1 then Match has Draw if 0 then match is still running
        static int flag = 0;
        static void Main(string[] args)
        {
            bool isHost = false;
            Console.WriteLine("Options:");
            Console.WriteLine("Enter 1 to host match");
            Console.WriteLine("Enter 2 to join match");
            Console.WriteLine("Enter 3 to exit");
            int choice =int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    isHost = true;
                    break;
                case 2:
                    isHost = false;
                    break;
                default:
                    return;
            }
            if(isHost)
            {
                
                HostPlay();
            }
            else
            {
                ClientPlay();
            }
        }
        public static void HostPlay()
        {
            Socket serverSocket = server.StartServer();
            bool play = true;
            do
            {
                arr = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                PlayServer(serverSocket);
                Console.Write("Do you want to play again[y/n]:");
                string serverChoice = Console.ReadLine();
                if (serverChoice == "y")
                {
                    server.SendMessage(serverSocket, serverChoice);
                    string clientChoice = server.RecieveMessage(serverSocket);
                    if (clientChoice == "y")
                    {
                        play = true;
                    }
                    else
                    {
                        play = false;
                    }
                }
                else
                {
                    server.SendMessage(serverSocket, serverChoice);
                    server.CloseConnection(serverSocket);
                    play = false;
                }
            } while (play);
        }
        public static void ClientPlay()
        {
            Console.Write("Server Ip:");
            string ipAdress = Console.ReadLine();
            Socket clientSocket = client.StartClient(ipAdress);
            bool play = true;
            do
            {
                arr = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                PlayClient(clientSocket);
                Console.Write("Do you want to play again[y/n]:");
                string clientChoice = Console.ReadLine();
                if (clientChoice == "y")
                {
                    client.SendMessage(clientSocket, clientChoice);
                    string serverChoice = client.RecieveMessage(clientSocket);
                    if (serverChoice == "y")
                    {
                        play = true;
                    }
                    else
                    {
                        play = false;
                    }
                }
                else
                {
                    client.SendMessage(clientSocket, clientChoice);
                    client.CloseConnection(clientSocket);
                    play = false;
                }
            } while (play);
        }
        public static void PlayServer(Socket serverSocket)
        {
            do
            {
                Console.Clear();// whenever loop will be again start then screen will be clear
                Console.WriteLine("Player1:X and Player2:O");
                Console.WriteLine("\n");
                if (player % 2 == 0)//checking the chance of the player
                {
                    Console.WriteLine("Player 2 Chance");
                }
                else
                {
                    Console.WriteLine("Player 1 Chance");
                }
                Console.WriteLine("\n");
                Board();// calling the board Function
                bool correctChoice = false;
                while (!correctChoice)
                {
                    if (player % 2 == 0)//checking the chance of the player
                    {
                        correctChoice = true;
                        Console.WriteLine("Recieveing from client.....");
                        choice = int.Parse(server.RecieveMessage(serverSocket));
                        Console.WriteLine($"choice:{choice}");
                    }
                    else
                    {
                        choice = int.Parse(Console.ReadLine());
                        if (choice > 0 && choice < 10)
                        {
                            correctChoice = true;
                            server.SendMessage(serverSocket, choice.ToString());
                        }
                        else
                        {
                            correctChoice = false;
                            Console.WriteLine("Wrong Choice Please Enter Again");
                        }
                    }
                }
                //Taking users choice
                // checking that position where user want to run is marked (with X or O) or not
                if (arr[choice] != 'X' && arr[choice] != 'O')
                {
                    if (player % 2 == 0) //if chance is of player 2 then mark O else mark X
                    {
                        arr[choice] = 'O';
                        player++;
                    }
                    else
                    {
                        arr[choice] = 'X';
                        player++;
                    }
                }
                else
                //If there is any possition where user want to run
                //and that is already marked then show message and load board again
                {
                    Console.WriteLine("Sorry the row {0} is already marked with {1}", choice, arr[choice]);
                    Console.WriteLine("\n");
                    Console.WriteLine("Please wait 2 second board is loading again.....");
                    Thread.Sleep(2000);
                }
                flag = CheckWin();// calling of check win
            }
            while (flag != 1 && flag != -1);
            // This loop will be run until all cell of the grid is not marked
            //with X and O or some player is not win
            Console.Clear();// clearing the console
            Console.WriteLine("Reloading Again");
            Board();// getting filled board again
            if (flag == 1)
            // if flag value is 1 then someone has win or
            //means who played marked last time which has win
            {
                Console.WriteLine("Player {0} has won", (player % 2) + 1);
            }
            else// if flag value is -1 the match will be draw and no one is winner
            {
                Console.WriteLine("Draw");
            }
            Console.ReadLine();
        }


        public static void PlayClient(Socket clientSocket)
        {
            do
            {
                Console.Clear();// whenever loop will be again start then screen will be clear
                Console.WriteLine("Player1:X and Player2:O");
                Console.WriteLine("\n");
                if (player % 2 == 0)//checking the chance of the player
                {
                    Console.WriteLine("Player 2 Chance");
                }
                else
                {
                    Console.WriteLine("Player 1 Chance");
                }
                Console.WriteLine("\n");
                Board();// calling the board Function
                bool correctChoice = false;
                while (!correctChoice)
                {
                    if (player % 2 == 0)//checking the chance of the player
                    {
                        choice = int.Parse(Console.ReadLine());
                        if (choice > 0 && choice < 10)
                        {
                            correctChoice = true;
                            client.SendMessage(clientSocket, choice.ToString());
                        }
                        else
                        {
                            correctChoice = false;
                            Console.WriteLine("Wrong Choice Please Enter Again");
                        }

                    }
                    else
                    {
                        correctChoice = true;
                        Console.WriteLine("Recieveing from server.....");
                        choice = int.Parse(client.RecieveMessage(clientSocket));
                        Console.WriteLine($"choice:{choice}");
                    }

                }
                //Taking users choice
                // checking that position where user want to run is marked (with X or O) or not
                if (arr[choice] != 'X' && arr[choice] != 'O')
                {
                    if (player % 2 == 0) //if chance is of player 2 then mark O else mark X
                    {
                        arr[choice] = 'O';
                        player++;
                    }
                    else
                    {
                        arr[choice] = 'X';
                        player++;
                    }
                }
                else
                //If there is any possition where user want to run
                //and that is already marked then show message and load board again
                {
                    Console.WriteLine("Sorry the row {0} is already marked with {1}", choice, arr[choice]);
                    Console.WriteLine("\n");
                    Console.WriteLine("Please wait 2 second board is loading again.....");
                    Thread.Sleep(2000);
                }
                flag = CheckWin();// calling of check win
            }
            while (flag != 1 && flag != -1);
            // This loop will be run until all cell of the grid is not marked
            //with X and O or some player is not win
            Console.Clear();// clearing the console
            Board();// getting filled board again
            if (flag == 1)
            // if flag value is 1 then someone has win or
            //means who played marked last time which has win
            {
                Console.WriteLine("Player {0} has won", (player % 2) + 1);
            }
            else// if flag value is -1 the match will be draw and no one is winner
            {
                Console.WriteLine("Draw");
            }
            Console.ReadLine();
        }



        // Board method which creats board
        private static void Board()
        {
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", arr[1], arr[2], arr[3]);
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", arr[4], arr[5], arr[6]);
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", arr[7], arr[8], arr[9]);
            Console.WriteLine("     |     |      ");
        }




        //Checking that any player has won or not
        private static int CheckWin()
        {
            #region Horzontal Winning Condtion
            //Winning Condition For First Row
            if (arr[1] == arr[2] && arr[2] == arr[3])
            {
                return 1;
            }
            //Winning Condition For Second Row
            else if (arr[4] == arr[5] && arr[5] == arr[6])
            {
                return 1;
            }
            //Winning Condition For Third Row
            else if (arr[6] == arr[7] && arr[7] == arr[8])
            {
                return 1;
            }
            #endregion
            #region vertical Winning Condtion
            //Winning Condition For First Column
            else if (arr[1] == arr[4] && arr[4] == arr[7])
            {
                return 1;
            }
            //Winning Condition For Second Column
            else if (arr[2] == arr[5] && arr[5] == arr[8])
            {
                return 1;
            }
            //Winning Condition For Third Column
            else if (arr[3] == arr[6] && arr[6] == arr[9])
            {
                return 1;
            }
            #endregion
            #region Diagonal Winning Condition
            else if (arr[1] == arr[5] && arr[5] == arr[9])
            {
                return 1;
            }
            else if (arr[3] == arr[5] && arr[5] == arr[7])
            {
                return 1;
            }
            #endregion
            #region Checking For Draw
            // If all the cells or values filled with X or O then any player has won the match
            else if (arr[1] != '1' && arr[2] != '2' && arr[3] != '3' && arr[4] != '4' && arr[5] != '5' && arr[6] != '6' && arr[7] != '7' && arr[8] != '8' && arr[9] != '9')
            {
                return -1;
            }
            #endregion
            else
            {
                return 0;
            }
        }
    }
}