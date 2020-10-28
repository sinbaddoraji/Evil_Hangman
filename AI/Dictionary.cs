using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    public class Dictionary
    {
        //
        // The names of wordFamilies
        //
        private readonly string[] _wordFamilies = new string[]
        {
            "ack", "ad","ail","ain","ake","ale","all","am","ame","an","ank", "ap", "ar", "ash", "at", "ate", "aw", "ay",

            "eat", "eel","eep","eet","ell","en","ent","est",
            
            "ice","ick","ide", "ife", "ight", "ile", "ill", "in", "ine", "ing", "ink", "ip", "it",

             "oat","ock","og", "oil", "oke", "oo", "ood", "oof", "ook", "oom", "ool", "oon", "oop", "oot", "ore", "orn", "ot", "ought", "ould", "ouse", "out", "ow", "own",

             "uck", "ug", "ump", ""
        };

        //
        // A dictionary object holding a list of all word families and the words they contain
        //
        public Dictionary<string, WordFamily> WordFamilies = new Dictionary<string, WordFamily>();

        //
        // string array containing a list of words in the dictionary
        //
        public string[] WordList;

        //
        // object for creating random integer values
        //
        private Random _randomizer = null;

        //
        // Return a random word from the Dictionary
        //
        public string RandomWord()
        {
            if (_randomizer == null) _randomizer = new Random();
            return WordList[_randomizer.Next(0, WordList.Length)];
        }

        //
        // Get Word-Family of a specified word
        //
        public string GetWordFamily(string word)
        {
            foreach (string wordFamily in _wordFamilies)
            {
                if (word.EndsWith(wordFamily)) return wordFamily;
            }

            return "";
        }

        //
        // Dictionary object for word guessing game
        //
        public Dictionary()
        {
            //Get list of words from word dictionary
            WordList = File.ReadAllLines("dictionary.txt");

            //Initalize Word families using the array of common word endings
            for (int i = 0; i < _wordFamilies.Length; i++)
            {
                WordFamilies.Add(_wordFamilies[i], new WordFamily());
            }

            Parallel.ForEach(WordList, word => 
            {
                lock (WordList)
                {
                    WordFamilies[GetWordFamily(word)].AddWord(word);
                }
            });
        }
    }

    public class WordFamily
    {
        //
        // Words in Word-Family
        //
        public List<string> Words { get; }

        //
        // A group of similar words
        //
        public WordFamily() => Words = new List<string>();

        //
        // Add word to Word-Family
        //
        public void AddWord(string word) => Words.Add(word);

        //
        // Get words of a certain length from Word-Family
        //
        public IEnumerable<string> GetWords(int length) => Words.Where(x => x.Length == length);
    }

}
