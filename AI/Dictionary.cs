using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AI
{
    public class Dictionary
    {
        //
        // string array containing a list of words in the dictionary
        //
        public string[] WordList;

        //
        // object for creating random integer values
        //
        private Random _randomizer = null;

        public bool DoesNotHaveWrongLetter(List<char> wrongletters, string word)
        {
            bool output = true;
            foreach (var c in wrongletters)
            {
                output &= !word.Contains(c);
            }

            return output;
        }
        //
        // Returns words in the same word-family of that has
        // the letters guessed correctly by the user based on how similar they are to the word
        //
        public IEnumerable<string> GetSimilarWords(string maskedWord, string secretWord, List<char> wrongletters)
        {
            //Get all the words matching word mask
            return WordList.Where(word => word.Length == maskedWord.Length && word != secretWord && MatchesMask(word, maskedWord) && DoesNotHaveWrongLetter(wrongletters, word));
        }

        public IEnumerable<string> GetWordsWithoutChar(char c, string maskedWord, int wordLen, List<char> wrongletters)
        {
            //Get all the words matching word mask
            return WordList.Where(word => word.Length == wordLen && ! word.Contains(c) && MatchesMask(word, maskedWord) && DoesNotHaveWrongLetter(wrongletters, word));
        }

        private bool MatchesMask(string word, string mask)
        {
            //Double-tape turing machine
            if (word.Length != mask.Length) throw new Exception("Word and Mask are not the same length");

            bool matchMask = true;
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i] != '-') matchMask &= mask[i] == word[i];
            }

            return matchMask;
        }

        //
        // Return a random word of a certain length from the Dictionary
        //
        public string GetRandomWordOfLength(int wordLength)
        {
            if (_randomizer == null) _randomizer = new Random();
            var words = WordList.Where(x => x.Length == wordLength).ToList();
            return words[_randomizer.Next(0, words.Count())];
        }

        //
        // Dictionary object for word guessing game
        //
        public Dictionary()
        {
            //Get list of words from word dictionary
            WordList = File.ReadAllLines("dictionary.txt");
        }
    }

}
