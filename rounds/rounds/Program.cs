using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace rounds
{
    internal class Program
    {

        static bool Warning = false;
        
        // read these two from environment.txt
        static int Rounds = 0;
        static int Courts = 0;

        static int MaxPlayers = 100;
        static int MaxRules = 20;
        static int PlayerParameters = 100;
        static int RulesParameters = 20;
        static int NumberofAvailable;
        static int ResultNumber = 0;

        // Define a 2D array of strings with 3 rows and 4 columns, but do not initialize
        static string[,] PlayerMatrix = new string[MaxPlayers, PlayerParameters];
        static string[,] Rules = new string[MaxRules, RulesParameters];
        static string[] Types = { "MS", "LS", "MD", "LD", "MXD" };
        static string[] Available = new string[100];
        
        // hold the results of the matches in  a string array
        // we need this to see who is available for choice in the same round
        // round, court, type, player number, partner1
        static string[,] Results = new string[100, 100];


        static int NumberOfPlayers = 0;

        static void Main(string[] args)
        {
            // Input files
            string environment = "environment.txt";
            string players = "players.txt";
            string rules = "rules.txt";


            Console.WriteLine("Rounds Generator, Martin Dew-Hattens");

            string path = Directory.GetCurrentDirectory();
            Console.WriteLine("Current working directory is" + " " + path);

            InitialiseMaxtrix();

            // Read from Environment.txt
            
            if (File.Exists(environment))
            {
                // This path is a file
                ProcessEnvironment(environment);
            }
            else
            {
                Console.WriteLine("ERROR: environment.txt not found");
                Console.ReadKey();
                Environment.Exit(-1);
            }
            

            if (Courts == 0 || Rounds == 0)
            {
                Console.WriteLine("ERROR: courts or rounds not defined in environment.txt");
                Console.ReadKey();
                Environment.Exit(-4);
            }

            // From from players.txt
            if (File.Exists(players))
            {
                // This path is a file
                ProcessPlayers(players);
            }
            else
            {
                Console.WriteLine("ERROR: players.txt not found");
                Console.ReadKey();
                Environment.Exit(-2);
            }


            // Read from rules.txt
            if (File.Exists(rules))
            {
                // This path is a file
                ProcessRules(rules);
            }
            else
            {
                Console.WriteLine("ERROR: rules.txt not found");
                Console.ReadKey();
                Environment.Exit(-3);
            }

            // and finally do the draw of the rounds
            GenerateRounds();


            Console.ReadKey();
            Environment.Exit(0);

        }

        public static void WriteResults()
        {

            // write to file rounds.txt
            string filePath = "rounds.txt";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                //Console.WriteLine("File deleted successfully.");
            }

            // Using StreamWriter in a using statement to automatically close the file
            using (StreamWriter writer = new StreamWriter(filePath))
            {

                int i = 0;
                while (Results[i, 0] != "")
                {
                    string player = Results[i, 3];
                    int val = int.Parse(player);

                    string surname = PlayerMatrix[val, 1];
                    string cn = PlayerMatrix[val, 2];
                    string gender = PlayerMatrix[val, 3];

                    writer.WriteLine("Round" + " " + Results[i, 0] + " " + "Court" + "  " + Results[i, 1] + " " + "Event" + "  " + Results[i, 2] + " " + "Player" + "  " + Results[i, 3] + " " + "Surname:" + surname + " " + "Name:" + cn + " " + "Gender:" + gender);
                    i = i + 1;
                }
            }




        }
        public static void ShowResults()
        {
            Console.WriteLine(" ");
            Console.WriteLine("Show Results");

            // round, court, type, player number, partner1
            //static string[,] Results = new string[100, 100];
            int i = 0;
            while (Results[i,0] != "")
            {
                //Console.WriteLine("debug1" + Results[i, 0]);
                //Console.WriteLine("debug1" + Results[i, 1]);
                //Console.WriteLine("debug1" + Results[i, 2]);
                //Console.WriteLine("debug1" + Results[i, 3]);

                string player = Results[i, 3];
                int val = int.Parse(player);

                string surname = PlayerMatrix[val-1, 1];
                string cn = PlayerMatrix[val-1, 2];
                string gender = PlayerMatrix[val-1, 3];

                Console.WriteLine("Round" + " " + Results[i, 0] + " " + "Court" + "  " + Results[i, 1] + " " + "Event" + "  " + Results[i, 2] + " " + "Player"  + "  " + Results[i, 3] + " " + "Surname:"  +  surname + " " + "Name:"  + cn + " " + "Gender:" + gender);
                i = i + 1;
            }
        } 

        public static int CreateMS(int i,int j)
        {

            int success;

            success = FindPersonForEvent(i, j, "male", "MS");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "male", "MS");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }
            return (0);

        }



        public static int CreateLS(int i, int j)
        {

            int success;

            success = FindPersonForEvent(i, j, "female", "LS");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "female", "LS");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }
            return (0);

        }

        public static int CreateLD(int i, int j)
        {

            int success;

            success = FindPersonForEvent(i, j, "female", "LD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "female", "LD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "female", "LD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "female", "LD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }
            return (0);

        }


        public static int CreateMD(int i, int j)
        {

            int success;

            success = FindPersonForEvent(i, j, "male", "MD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "male", "MD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "male", "MD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "male", "MD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }
            return (0);

        }


        public static int CreateMXD(int i, int j)
        {

            int success;

            success = FindPersonForEvent(i, j, "male", "MXD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "male", "MXD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "female", "MXD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }

            success = FindPersonForEvent(i, j, "female", "MXD");
            if (success != 0)
            {
                ClearCourt(i, j);
                return (-1);
            }
            return (0);

        }

        public static void ClearCourt(int court, int round)
        {
            // clear the results given for the round and court
            for (int k = 0; k < 100; k++)
            {
                //Console.WriteLine("debug" + Results[k, 0] + Results[k, 1]);

                if (!string.IsNullOrEmpty(Results[k, 0]) && !string.IsNullOrEmpty(Results[k, 1]))
                {
                    if (Convert.ToInt32(Results[k, 0]) == round)
                    {
                        if (Convert.ToInt32(Results[k, 1]) == court)
                        {
                            //Console.WriteLine("ShowUnusedPlayersForRound:player " + i + " is busy");
                            Results[k, 0] = "";
                            Results[k, 1] = "";
                            Results[k, 2] = "";
                            Results[k, 3] = "";
                           
                        }
                    }
                }
            }


        }

        public static int EmergencyMatch(int i, int j)
        {
            int success;

            // use a fixed list

            Console.WriteLine("Trying to ceate LS");
            success = CreateLS(i, j);
            if (success != 0)
            {
                Console.WriteLine("LS Match creation failed");
                Console.WriteLine("Trying to create MD");
                success = CreateMD(i, j);
                if (success != 0)
                {
                   
                    Console.WriteLine("MD Match creation failed");
                    Console.WriteLine("Trying to create LD");
                    success = CreateLD(i, j);
                    if (success != 0)
                    {
                        Console.WriteLine("LD Match creation failed");
                        Console.WriteLine("Trying to create MXD");
                        success = CreateMXD(i, j);
                        if (success != 0)
                        {
                            Console.WriteLine("MXD Match creation failed");
                            Console.WriteLine("No match could be created");
                            return (-1);
                        }
                    }
                }
            }
            return (0);

        }

        public static void GenerateRounds()
        {
            Console.WriteLine(" ");
            Console.WriteLine("Generating rounds");
            
            int i,j;
            int r = 0;
            int success = 0;

            int NumberofTypes = Types.Length;

            // generate over all rounds and all courts
            for (i=1; i <= Rounds; i++)
            {
                Console.WriteLine(" ");
                Console.WriteLine("Round" + " " + i );


                for (j = 1; j <= Courts; j++)
                {
                    Console.WriteLine("Court" + " " + j);
                    // generate a match type
                    r = GenerateRandom(NumberofTypes);
                    //Console.WriteLine("Random type of match is " + " " + r);
                    Console.WriteLine("Match type is " + Types[r - 1]);

                    if (Types[r - 1] == "MS")
                    {
                            success = CreateMS(i, j);
                            if (success != 0)
                            {
                                Console.WriteLine("MS Match creation  failed");
                                // so try and create another type
                                success=EmergencyMatch(i, j);
                                if (success != 0)
                                {
                                    Console.WriteLine("No match could be created");
                                }
                            }
                    }
                    



                    if (Types[r - 1] == "LS")
                    {
                        success = CreateLS(i, j);
                        if (success != 0)
                        {
                            Console.WriteLine("LS Match creation  failed");
                            // so try and create another type
                            success = EmergencyMatch(i, j);
                            if (success != 0)
                            {
                                Console.WriteLine("No match could be created");
                            }
                        }

                    }

                    if (Types[r - 1] == "LD")
                    {
                        success = CreateLD(i, j);
                        if (success != 0)
                        {
                            Console.WriteLine("LD Match creation  failed");
                            // so try and create another type
                            success = EmergencyMatch(i, j);
                            if (success != 0)
                            {
                                Console.WriteLine("No match could be created");
                            }
                        }


                    }
                    if (Types[r - 1] == "MD")
                    {
                        success = CreateMD(i, j);
                        if (success != 0)
                        {
                            Console.WriteLine("MD Match creation  failed");
                            // so try and create another type
                            success = EmergencyMatch(i, j);
                            if (success != 0)
                            {
                                Console.WriteLine("No match could be created");
                            }
                        }


                    }
                    if (Types[r - 1] == "MXD")
                    {
                        success = CreateMXD(i, j);
                        if (success != 0)
                        {
                            Console.WriteLine("MXD Match creation  failed");
                            // so try and create another type
                            success = EmergencyMatch(i, j);
                            if (success != 0)
                            {
                                Console.WriteLine("No match could be created");
                            }
                        }

                    }   


                }
                ShowUnusedPlayersForRound(i);

            }
            ShowResults();
            WriteResults();
            Console.WriteLine("Generation finished");    
            
            
        }



        public static void ShowUnusedPlayersForRound(int round)
        {
            // for each round show any players which do not have a match
            int Used = 0;
            Console.WriteLine(" ");
            Console.WriteLine("Unused players this round");

            for (int i = 1; i <= NumberOfPlayers; i++)
            {
                // is the player in the Result list for this round?

                    // is he/she available?
                    // loop through Results list
                    bool busy = false;

                    for (int k = 0; k < 100; k++)
                    {
                        //Console.WriteLine("debug" + Results[k, 0] + Results[k, 1]);

                        if (!string.IsNullOrEmpty(Results[k, 0]) && !string.IsNullOrEmpty(Results[k, 1]))
                        {
                            if (Convert.ToInt32(Results[k, 0]) == round)
                            {
                                if (Convert.ToInt32(Results[k, 3]) == i)
                                {
                                    //Console.WriteLine("ShowUnusedPlayersForRound:player " + i + " is busy");
                                    busy = true;
                                    Used = Used + 1;
                                    break;
                                }
                            }
                        }
                    }

                    if (busy == false)
                    {
                        Console.WriteLine("Player " + i + " " + PlayerMatrix[i - 1, 1 ] + "," + PlayerMatrix[i - 1, 2] + " Gender:" + PlayerMatrix[i - 1, 3] + " is unused this round");
                    }

                
            }

            if (Used < NumberOfPlayers)
                Console.WriteLine("Number of players used is " + Used + " out of " + NumberOfPlayers);

            Console.WriteLine("End of unused players listing");


        }
            public static void ClearAvailable()
        {
            for (int i=0; i < 100; i++)
                Available[i] = "";

        }
        public static void ShowAvailable()
        {
            int i = 0;
            Console.WriteLine("Available List");
            while (Available[i] != "")
            { 
                Console.WriteLine("Available" + " " + Available[i]);
                i = i + 1;
            }
        }

        public static int FindPersonForEvent(int round, int court, string gender, string Event)
        {


            ClearAvailable();
            NumberofAvailable = 0;
            int Choice = 0;

            // look in player matrix for all ladies and check if they are available
            // form a list and choose one by random

            //Console.WriteLine("Round" + " " + round + " " + "court" + " " + court);
            for (int i = 1; i <= NumberOfPlayers; i++)
            {
                //Console.WriteLine("Testing player" + " " + i + " " + PlayerMatrix[i - 1, 3]);
                if (PlayerMatrix[i - 1, 3] == gender)
                {

                    // is he/she available?
                    // loop through Results list
                    bool busy = false;

                    for (int k = 0; k < 100; k++)
                    {
                        //Console.WriteLine("debug" + Results[k, 0] + Results[k, 1]);

                        if (!string.IsNullOrEmpty(Results[k, 0]) && !string.IsNullOrEmpty(Results[k, 1]))
                        {
                            if (Convert.ToInt32(Results[k, 0]) == round)
                            {
                                if (Convert.ToInt32(Results[k, 3]) == i)
                                {
                                    //Console.WriteLine("player is already busy");
                                    busy = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (busy == false)
                    {
                        // ok so no busy but do they play this event?
                        if (Rules[0,1] == "true")
                        {
                            if (Event == "MS" && PlayerMatrix[i - 1, 4] == "false")
                            {
                                Console.WriteLine("Player " + i + " will not play singles");
                                continue;
                            }
                            if (Event == "LS" && PlayerMatrix[i - 1, 4] == "false")
                            {
                                Console.WriteLine("Player " + i + " will not play singles");
                                continue;
                            }
                            if (Event == "LD" && PlayerMatrix[i - 1, 5] == "false")
                            {
                                Console.WriteLine("Player " + i + " will not play doubles");
                                continue;
                            }
                            if (Event == "MD" && PlayerMatrix[i - 1, 5] == "false")
                            {
                                Console.WriteLine("Player " + i + " will not play doubles");
                                continue;
                            }
                            if (Event == "MXD" && PlayerMatrix[i - 1, 6] == "false")
                            {
                                Console.WriteLine("Player " + i + " will not play mixed");
                                continue;
                            }
                        }

                        // add her to available list
                        //Console.WriteLine("adding to Available list");
                        Available[NumberofAvailable] = PlayerMatrix[i - 1, 0];
                        NumberofAvailable = NumberofAvailable + 1;

                    }

                }
            }
            if (NumberofAvailable == 0)
            {
                Console.WriteLine("Lack of players available for this choice");
                
                return (-1);
            }
            else
            {
                //ShowAvailable();

                // then find an available opponent by random choice
                Choice = GenerateRandom(NumberofAvailable);
                //Console.WriteLine("Random player from available list " + " " + Choice);
                //Console.WriteLine("Playernumber is " + Available[Choice - 1]);

                // enter into results

                Results[ResultNumber, 0] = round.ToString();
                Results[ResultNumber, 1] = court.ToString();
                Results[ResultNumber, 2] = Event;
                Results[ResultNumber, 3] = Available[Choice - 1];

                //Console.WriteLine("debug2 round " + Results[ResultNumber, 0]);
                //Console.WriteLine("debig2 court " + Results[ResultNumber, 1]);
                //Console.WriteLine("debug2 Event " + Results[ResultNumber, 2]);
                //Console.WriteLine("debug2 player " + Results[ResultNumber, 3]);

                ResultNumber = ResultNumber + 1;

            }

            return (0);

        }
        public static int GenerateRandom(int maxvalue)
        {
            // Create an instance of the Random class
            Random random = new Random();

            // Generate a random number between 1 (inclusive) and 100 (exclusive)
            int randomNumber = random.Next(1, maxvalue+1);

            // Print the random number
            //Console.WriteLine("Random number between 1 and" + " " + maxvalue + " " + "is"  + " " + randomNumber);


            return randomNumber;
        }

        public static void InitialiseMaxtrix()
        {

            int i,j;

            for (i = 0; i < MaxPlayers; i++)
                {
                for (j=0; j< PlayerParameters; j++)
                    PlayerMatrix[i,j] = "" ;
            }
            for (i = 0; i < MaxRules; i++)
            {
                for (j = 0; j < RulesParameters; j++)
                    Rules[i, j] = "";
            }
            for (i = 0; i < 100; i++)
            {
                for (j = 0; j < 100; j++)
                    Results[i, j] = "";

            }
        }
        public static void ProcessRules(string rules)
        {
            Console.WriteLine(" ");
            Console.WriteLine("Processing Rules");

            using (StreamReader reader = new StreamReader(rules))
            {
                string line;
                // Read the file line by line
                while ((line = reader.ReadLine()) != null)
                {


                    // skip over empty lines
                    if (string.IsNullOrEmpty(line))
                    {
                        //Console.WriteLine("empty line");
                        continue;
                    }


                    // skip over lines with a hash 
                    if (!string.IsNullOrEmpty(line) && line[0] == '#')
                    {
                        continue;
                    }

                    // parse by spaces or tabs
                    char[] delimiters = new char[] { ' ', '\t' };

                    // Split the line using the delimiters, removing empty entries
                    string[] words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    
                    
                    if (words[0] == "1")
                    {
                        // player preferences  - true or false for singles or doubles
                        Rules[0,0] = words[0];
                        Rules[0,1] = words[1];

                        Console.WriteLine("Found rule 1" + " " + words[1]);
                    }
                  


                }



            }
            Console.WriteLine("Finished Processing rules");

        } 

        public static void ProcessEnvironment(string environment)
        {
            Console.WriteLine(" ");
            Console.WriteLine("Processing Environment");

            using (StreamReader reader = new StreamReader(environment))
            {
                string line;
                // Read the file line by line
                while ((line = reader.ReadLine()) != null)
                {
                    // Process the line (e.g., print to console)
                    //Console.WriteLine(line);

                    // skip over empty lines
                    if (string.IsNullOrEmpty(line))
                    {
                        //Console.WriteLine("empty line");
                        continue;
                    }


                    // skip over lines with a hash 
                    if (!string.IsNullOrEmpty(line) && line[0] == '#')
                    {
                        //Console.WriteLine("The line starts with a hash (#): " + line);
                        continue;
                    }

                    // parse by spaces or tabs
                    char[] delimiters = new char[] { ' ', '\t' };

                    // Split the line using the delimiters, removing empty entries
                    string[] words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    if (words[0] == "courts")
                    {
                        Courts = int.Parse(words[1]);
                        Console.WriteLine("Found courts " + " " + Courts);
                    }
                    if (words[0] == "rounds")
                    {
                        Rounds = int.Parse(words[1]);
                        Console.WriteLine("Found rounds " + " " + Rounds);
                    }


                }
            }

            Console.WriteLine("Finished Processing environment");

        }
        public static void ProcessPlayers(string players)
        {

            Console.WriteLine(" ");
            Console.WriteLine("Processing Players");

            int PlayerIndex = -1;

            using (StreamReader reader = new StreamReader(players))
            {
                string line;
                // Read the file line by line
                
                while ((line = reader.ReadLine()) != null)
                {
                    //Console.WriteLine("testing" + " " +  line);

                    // skip over empty lines
                    if (string.IsNullOrEmpty(line) )
                    {
                        //Console.WriteLine("empty line");
                        continue;
                    }


                    // skip over lines with a hash 
                    if (!string.IsNullOrEmpty(line) && line[0] == '#')
                    { 
                        //Console.WriteLine("The line starts with a hash (#): " + line);
                        continue;
                    }

                    Console.WriteLine(line);

                    // so we have a valid line  - update the player indexx
                    PlayerIndex = PlayerIndex + 1;

                    // parse by spaces or tabs
                    char[] delimiters = new char[] { ' ', '\t' };

                    // Split the line using the delimiters, removing empty entries
                    string[] words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    /*
                        # Number
                        # Surname
                        # FirstName
                        # Gender (male, female)
                        # plays singles
                        # plays doubles
                        # Players mixed
                        # Preferred doubles player number (0 for none)
                        # Preferred Mixed player number (0 for none)
                        # Singles ranking
                        # Doubles ranking
                        # Mixed ranking
                    */

                    // first value is player number
                   

                    PlayerMatrix[PlayerIndex,0] = words[0];
                    PlayerMatrix[PlayerIndex,1] = words[1];
                    PlayerMatrix[PlayerIndex, 2] = words[2];
                    PlayerMatrix[PlayerIndex, 3] = words[3];
                    PlayerMatrix[PlayerIndex, 4] = words[4];
                    PlayerMatrix[PlayerIndex, 5] = words[5];
                    PlayerMatrix[PlayerIndex, 6] = words[6];
                    PlayerMatrix[PlayerIndex, 7] = words[7];
                    PlayerMatrix[PlayerIndex, 8] = words[8];
                    PlayerMatrix[PlayerIndex, 9] = words[9];
                    PlayerMatrix[PlayerIndex, 10] = words[10];
                    PlayerMatrix[PlayerIndex, 11] = words[11];

                    //Console.WriteLine(PlayerIndex + " " + words[0] + " " + words[1] + " " + words[2] + " " + words[3]) ;
                }

            }
            NumberOfPlayers = PlayerIndex + 1;
            Console.WriteLine("Number of players found is" + " " + NumberOfPlayers);
            Console.WriteLine("Finished Processing players");

        }
    }
}
