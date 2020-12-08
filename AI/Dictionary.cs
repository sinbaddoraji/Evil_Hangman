using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AI
{
    public class Dictionary
    {
        //
        // string array containing a list of words in the dictionary
        //
        public List<string> WordList;

        //
        // object for creating random integer values
        //
        private Random _randomizer = null;

        //
        // Last MinMax generatedWord
        //
        public string MinMaxWord = "";

        //
        // Word does not contain characters asserted to not belong in the word being guessed
        //
        public bool DoesNotHaveWrongLetter(List<char> wrongletters, string word)
        {
            bool output = true;
            foreach (var c in wrongletters)
            {
                output &= !word.Contains(c);
            }

            return output;
        }


        public List<IEnumerable<string>> GetWordFamilies(string word, string mask, List<char> wrongletters, List<string> wrdList)
        {
            var wordList = wrdList;
            if (wrdList == null)
                wordList = WordList;

            if (mask.Trim() == "") return null;
            List<IEnumerable<string>> output = new List<IEnumerable<string>>();

            //Get letters not yet revealed
            List<char> unrevealed = new List<char>();
            for (int i = 0; i < word.Length; i++)
            {
                if(mask[i] == '-')
                     unrevealed.Add(word[i]);
            }
            
            //Generate all possible word masks
            List<string> possibleWordMasks = new List<string>();
            Parallel.ForEach(unrevealed, unrevealedLetter => 
            {
                lock(possibleWordMasks)
                {
                    string newMask = "";
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (word[i] == unrevealedLetter)
                            newMask += unrevealedLetter.ToString();
                        else
                            newMask += mask[i].ToString();
                    }
                    possibleWordMasks.Add(newMask);
                }
                
            });

            Parallel.ForEach(possibleWordMasks, wordMask => 
            {
                lock(output)
                {
                    output.Add(GetSimilarWords(wordMask, word, wrongletters, wordList));
                }
            });
            return output;
        }

        

        public IEnumerable<string> GetMinMaxFamily(string word, string mask, List<char> wrongletters, List<char> correctLetters, List<string> wrdList)
        {
            var wordList = wrdList;
            if (wrdList == null)
                wordList = WordList;

            //Return word family based on Knuth's minmax algorithm
            var wordFamilies = GetWordFamilies(word, mask, wrongletters, wordList);
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
                    var newWordMask = new string(wrd.Select(x => correctLetters.Contains(x) ? x : '-').ToArray());
                    int len = GetSimilarWords(mask, wrd, wrongletters, wordList).Count();

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
        public IEnumerable<string> GetSimilarWords(string maskedWord, string secretWord, List<char> wrongletters, List<string> wrdList)
        {
            var wordList = wrdList;
            if (wrdList == null)
                wordList = WordList;

            //Get all the words matching word mask
            return wordList.Where(word => word.Length == maskedWord.Length && word != secretWord && MatchesMask(word, maskedWord) && DoesNotHaveWrongLetter(wrongletters, word));
        }

        public IEnumerable<string> GetWordsWithoutChar(char c, string maskedWord, int wordLen, List<char> wrongletters, List<string> wrdList)
        {
            var wordList = wrdList;
            if (wrdList == null)
                wordList = WordList;

            //Get all the words matching word mask
            return wordList.Where(word => word.Length == wordLen && ! word.Contains(c) && MatchesMask(word, maskedWord) && DoesNotHaveWrongLetter(wrongletters, word));
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
            WordList = File.ReadAllLines("dictionary.txt").ToList();
        }
    }

}
