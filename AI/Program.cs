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
           
            /*
            Console.WriteLine($"Random Word: {GameDictionary.RandomWord()}");
            Console.WriteLine($"Random Word: {GameDictionary.RandomWord()}");
            Console.WriteLine($"Random Word: {GameDictionary.RandomWord()}");
            Console.WriteLine($"Random Word: {GameDictionary.RandomWord()}\n");

            foreach (var item in GameDictionary.WordFamilies["oom"].GetWords(4))
            {
                Console.WriteLine(item);
            } 
            */
            Console.ReadKey();
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
