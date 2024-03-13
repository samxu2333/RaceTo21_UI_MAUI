using System;

namespace RaceTo21
{
    class Program
    {
        /// <summary>
        /// Instantiate game elements. Repeatedly prompt game to update until end condition detected.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            CardTable cardTable = new CardTable();
            Game game = new Game(cardTable);
            string response = Console.ReadLine();
            do
            {
                while (game.nextTask != Task.GameOver)
                {
                    game.DoNextTask();
                    Console.Write("Do you want to play again? (Y/N)");   
                }
            }
            while (response.ToUpper().StartsWith("Y"));       
        }
    }
}

