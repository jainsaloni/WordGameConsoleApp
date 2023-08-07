using ConsoleTables;
using System;
using System.Data.SqlClient;

namespace WordGameConsoleApp
{
    public class PlayGame
    {
        private string word = "";
        private string displayWord = "";
        private string playerName = "";
        private int numberOfGuesses = 10;
        private bool gameOver = false;

        public void GetPlayer(SqlConnection sqlConnection)
        {
            //Get the player name
            Console.WriteLine("Player Name:");
            playerName = Console.ReadLine();

            //Add player to the database with initial score 0
            string insertPlayerQuery = "INSERT INTO PLAYERS (PlayerName, Score) VALUES ('" + playerName + "','" + 0 + "')";
            SqlCommand insertCommand = new SqlCommand(insertPlayerQuery, sqlConnection);
            insertCommand.ExecuteNonQuery();

            Start:
                StartGame(sqlConnection);

                GetScoreBoard(sqlConnection);

                Console.WriteLine("Would you like to continue? y/n");
                string answer = Console.ReadLine();

                if (answer == "y")
                {
                    gameOver = false;
                    numberOfGuesses = 10;
                    goto Start;
                }
                else if (answer != "n")
                    Console.WriteLine("Sorry wrong input");
        }

        private void StartGame(SqlConnection sqlConnection)
        {
            //Get a random word from the table of word list
            string selectWordQuery = "SELECT TOP 1 * FROM WORDS ORDER BY NEWID()";
            SqlCommand getWord = new SqlCommand(selectWordQuery, sqlConnection);
            SqlDataReader wordReader = getWord.ExecuteReader();
            while (wordReader.Read())
            {
                word = wordReader.GetValue(1).ToString();
            }
            wordReader.Close();

            //storing the word into char array and replacing some letters with _ for the player to guess
            char[] charArray = word.ToCharArray();
            char[] originalArray = word.ToCharArray();
            for (int i = 1; i < word.Length; i += 2)
            {
                charArray[i] = '_';
            }
            displayWord = string.Join("", charArray);

            while (gameOver == false)
            {
                Console.Clear();
                Console.WriteLine(displayWord);
                Console.WriteLine("You have " + numberOfGuesses + " attempts!");
                Console.WriteLine("Guess the letter:");
                string letter = Console.ReadLine();
                string letterUpper = letter.ToUpper();

                if (String.IsNullOrEmpty(letterUpper) || letterUpper.Length == 0)
                {
                    Console.WriteLine("Please enter a valid letter.");
                }

                //if the letter matches the words get updated with the letter and no guess is deducted
                if (word.Contains(letterUpper) && !String.IsNullOrEmpty(letterUpper))
                {
                    for (int i = 0; i < originalArray.Length; i++)
                    {
                        if (originalArray[i].Equals(char.Parse(letterUpper)))
                        {
                            charArray[i] = originalArray[i];
                        }
                    }
                    displayWord = string.Join("", charArray);
                    Console.WriteLine(displayWord);
                }
                //wrong guess deducts the number of guesses
                else
                {
                    numberOfGuesses--;
                }
                //when all letter are guessed player wins and score is updated
                if (word.Equals(displayWord))
                {
                    gameOver = true;
                    Console.WriteLine("YOU WON!");
                    UpdatePlayer(sqlConnection);
                }
                //when all guesses are used player looses and score is updated as 0
                if (numberOfGuesses == 0)
                {
                    gameOver = true;
                    Console.WriteLine("YOU LOOSE!");
                    UpdatePlayer(sqlConnection);
                }
            }
        }

        private void UpdatePlayer(SqlConnection sqlConnection)
        {
            //update the player with new score based on number of guesses remaining to guess the word. Highest being 10 and lowest being 0
            string updateQuery = "UPDATE PLAYERS SET SCORE =" + numberOfGuesses + " WHERE PLAYERNAME =" + "'" + playerName + "'";
            SqlCommand update = new SqlCommand(updateQuery, sqlConnection);
            update.ExecuteNonQuery();
        }

        private void GetScoreBoard(SqlConnection sqlConnection)
        {
            //Get all players data and display in a table format ordered by the highest score
            Console.WriteLine("\nScoreboard:");
            string selectQuery = "SELECT * FROM PLAYERS ORDER BY SCORE DESC";
            SqlCommand display = new SqlCommand(selectQuery, sqlConnection);
            SqlDataReader dataReader = display.ExecuteReader();
            var table = new ConsoleTable("Player Name", "Score");
            while (dataReader.Read())
            {
                table.AddRow(dataReader.GetValue(1).ToString(), dataReader.GetValue(2).ToString());
            }
            Console.WriteLine(table);
            dataReader.Close();
        }
    }
}
