using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    class Program
    {
        public static Dictionary GameDictionary;
        private static GameEngine GameEngine;

        static void Main()
        {
            LoadGame();
            GameEngine.Restart();
        }

        static void LoadGame()
        {
            //Prompt the user with a game loading message
            //(This is a result of the large dictionary size)
            
            Console.WriteLine("Game loading...");
            GameDictionary = new Dictionary();
            GameEngine = new GameEngine();
            Console.Clear();

            //Start AI game
            GameEngine.StartGame();
        }
    }
}
