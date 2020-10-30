﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    class Program
    {
        static Dictionary GameDictionary;

        static void Main()
        {
            LoadGame();
            for (int i = 0; i < 5; i++)
            {
                string special = GameDictionary.RandomWord();
                Console.WriteLine($"Random Word: {special}");

                foreach (var similarWord in GameDictionary.GetSimilarWords(special))
                {
                    Console.WriteLine(similarWord);
                }
            }
            

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
            Console.Clear();
        }
    }
}
