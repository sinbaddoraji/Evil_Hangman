﻿using System;
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

        //
        // Returns words in the same word-family of that has
        // the letters guessed correctly by the user based on how similar they are to the word
        // (The lower the index, the less similar it is)
        //
        public IEnumerable<string> GetWordFamily(string maskedWord, string secretWord)
        {
            //Get all the words matching word mask
            return WordList.Where(x => x.Length == maskedWord.Length && x != secretWord  && MatchesMask(x, maskedWord))
                           .OrderBy(x => CalculateSimilarity(x, secretWord));
        }

        private bool MatchesMask(string word, string mask)
        {
            //Multi-tape turing machine
            if (word.Length != mask.Length) throw new Exception("Word and Mask are not the same length");

            bool matchMask = true;
            for (int i = 0; i < mask.Length; i++)
            {
                if(mask[i] != '-') matchMask &= mask[i] == word[i];
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

        /// <summary>
        /// Calculate percentage similarity of two strings
        /// <param name="source">Source String to Compare with</param>
        /// <param name="target">Targeted String to Compare</param>
        /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        public double CalculateSimilarity(string source, string target)
        {
            //From https://www.techdoubts.net/2018/10/measure-similarity-of-2-strings-levenshtein.html
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }

        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary>
        int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
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
