using System;

namespace AI
{
    class Program
    {
        //
        //  Program Entry
        //
        static void Main()
        {
            //Prompt the user with a game loading message
            //(This is a result of the large dictionary size)
            Console.WriteLine("Game loading...");
            Dictionary.LoadDictionary();
            Console.Clear();

            //Start AI game
            GameEngine.StartGame();
            GameEngine.Restart();
        }
    }
}
