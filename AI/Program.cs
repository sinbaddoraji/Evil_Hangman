﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    class Program
    {
        //
        // Object handling interaction between game and the dictionary words
        //
        public static Dictionary GameDictionary;

        //
        // Game engine containing the AI and game platform
        //
        private static GameEngine GameEngine;

        //
        // Load and Start Game
        //(Initalize game objects)
        //
        static void LoadGame()
        {
            //Prompt the user with a game loading message
            //(This is a result of the large dictionary size)
           

            Console.WriteLine("Game loading...");
            GameDictionary = new Dictionary();
            GameEngine = new GameEngine();
            Console.Clear();

            Console.WriteLine("Mask Test: b---et");

            foreach (var item in GameDictionary.GetWordFamily("b---et", "basket"))
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();

            //Start AI game
            GameEngine.StartGame();
        }

        //
        //  Program Entry
        //
        static void Main()
        {
            LoadGame();
            GameEngine.Restart();
        }
    }
}
