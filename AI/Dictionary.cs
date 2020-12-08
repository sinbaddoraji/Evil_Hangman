using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AI
{
    public static class Dictionary
    {
        //
        // string array containing a list of words in the dictionary
        //
        public static List<string> WordList;

        //
        // object for creating random integer values
        //
        private static Random _randomizer = null;

        //
        // Last MinMax generatedWord
        //
        public static string MinMaxWord = "";


        //
        // Word does not contain characters asserted to not belong in the word being guessed
        // Also asserts if word has a higher count of correctly guessed letters
        //
        public static bool DoesNotHaveWrongLetter(string word)
        {
            bool output = true;

            //Assert that the current word does not contain any
            //character currently known to be a wrong guess
            foreach (var c in GameEngine._wrongLetters)
            {
                output &= !word.Contains(c);
            }

            //Assert that the current word has the same number of instances
            //of correct characters as the previously correct word
            foreach (var keyValuePair in GameEngine.correctLetterCount)
            {
                output &= word.Count(x => x == keyValuePair.Key) == keyValuePair.Value;
            }
            return output;
        }

        //
        // Get all possible word families
        //
        public static List<IEnumerable<string>> GetWordFamilies(string word, string mask)
        {
            //Use wordList as word family if word family is uninitalized
            var wordList = GameEngine._wordFamily;
            if (wordList == null)
                wordList = WordList;

            List<IEnumerable<string>> output = new List<IEnumerable<string>>();
            //Get letters not yet revealed
            List<char> unrevealed = new List<char>();
            for (int i = 0; i < word.Length; i++)
            {
                if(mask[i] == '-')
                     unrevealed.Add(word[i]);
            }
            
            //Get all word families
            Parallel.ForEach(unrevealed, unrevealedLetter => 
            {
                lock(output)
                {
                    string newMask = "";
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (word[i] == unrevealedLetter)
                            newMask += unrevealedLetter.ToString();
                        else
                            newMask += mask[i].ToString();
                    }
                    output.Add(GetSimilarWords(newMask, word));
                }
                
            });
            return output;
        }


        //
        // Get word family using min max algorithm
        //
        public static IEnumerable<string> GetMinMaxFamily(string word, string mask)
        {
            //Use wordList as word family if word family is uninitalized
            var wordList = GameEngine._wordFamily;
            if (wordList == null)
                wordList = WordList;

            //Return word family based on Knuth's minmax algorithm
            var wordFamilies = GetWordFamilies(word, mask);
            if (wordFamilies.Count() == 0) return null;

            //Get word family with the largest count
            var largestWordFamily = wordFamilies.OrderByDescending(x => x.Count()).First();
            if (largestWordFamily.Count() == 0)
                return null;

            //Get value from largestWordFamily that will reduce wordFamily size the most
            int lowest = -1;
            MinMaxWord = "";
            Parallel.ForEach(largestWordFamily, wrd => 
            {
                lock(MinMaxWord)
                {
                    //Get new word mask by replacing '-' with previously unrevealed character
                    var newWordMask = new string(wrd.Select(x => GameEngine._correctLetters.Contains(x) ? x : '-').ToArray());

                    //Get how many words would exist in the word family
                    int len = GetSimilarWords(mask, wrd).Count();

                    if (lowest == -1 || len < lowest)
                    {
                        lowest = len;
                        MinMaxWord = wrd;
                    }
                }
                
            });
            

            return largestWordFamily;
        }


        //
        // Returns words in the same word-family of that has
        // the letters guessed correctly by the user based on how similar they are to the word
        //
        public static IEnumerable<string> GetSimilarWords(string maskedWord, string secretWord)
        {
            //Use wordList as word family if word family is uninitalized
            var wordList = GameEngine._wordFamily;
            if (wordList == null)
                wordList = WordList;

            //Get all the words matching word mask
            return wordList.Where(word => word.Length == maskedWord.Length && word != secretWord && MatchesMask(word, maskedWord) && DoesNotHaveWrongLetter(word));
        }

        public static IEnumerable<string> GetWordsWithoutChar(char c, string maskedWord, int wordLen)
        {
            //Use wordList as word family if word family is uninitalized
            var wordList = GameEngine._wordFamily;
            if (wordList == null)
                wordList = WordList;

            //Get all the words matching word mask
            return wordList.Where(word => word.Length == wordLen && ! word.Contains(c) && MatchesMask(word, maskedWord) && DoesNotHaveWrongLetter(word));
        }

        private static bool MatchesMask(string word, string mask)
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
        public static string GetRandomWordOfLength(int wordLength)
        {
            if (_randomizer == null) _randomizer = new Random();
            var words = WordList.Where(x => x.Length == wordLength).ToList();
            return words[_randomizer.Next(0, words.Count())];
        }

        //
        // Dictionary object for word guessing game
        //
        public static void LoadDictionary()
        {
            //Get list of words from word dictionary
            WordList = File.ReadAllLines("dictionary.txt").ToList();
        }
    }

}
