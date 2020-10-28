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
        private readonly string[] _wordFamilies = new string[]
        {
            "ack", "ad","ail","ain","ake","ale","all","am","ame","an","ank", "ap", "ar", "ash", "at", "ate", "aw", "ay",

            "eat", "eel","eep","eet","ell","en","ent","est",
            
            "ice","ick","ide", "ife", "ight", "ile", "ill", "in", "ine", "ing", "ink", "ip", "it",

             "oat","ock","og", "oil", "oke", "oo", "ood", "oof", "ook", "oom", "ool", "oon", "oop", "oot", "ore", "orn", "ot", "ought", "ould", "ouse", "out", "ow", "own",

             "uck", "ug", "ump"
        };

        public Dictionary<string, WordFamily> WordFamilies = new Dictionary<string, WordFamily>();

        public string[] WordList;

        public Dictionary()
        {
            //Get list of words from word dictionary
            WordList = File.ReadAllLines("dictionary.txt");

            //Initalize Word families using the array of common word endings
            foreach (var wordFamily in _wordFamilies)
            {
                WordFamily newWordFamily = new WordFamily();
                WordFamilies.Add(wordFamily, newWordFamily);
            }

            Parallel.ForEach(WordList.AsParallel(), word => 
            {
                lock (WordList)
                {
                    foreach (var wordFamily in _wordFamilies)
                    {
                        if (word.EndsWith(wordFamily))
                        {
                            WordFamilies[wordFamily].AddWord(word);
                            break; //End loop beacause word can only belong to one word family.
                        }
                    }
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
        public void AddWord(string word)
        {
            Words.Add(word);
        }

        //
        // Get words of a certain length from Word-Family
        //
        public IEnumerable<string> GetWords(int length)
        {
            return Words.Where(x => x.Length == length);
        }
    }

}
