using System;
using System.Collections.Generic;
using System.Linq;

namespace AI
{
    public static class GameEngine
    {
        //
        // If true the number of words left in the word list will be displayed
        //
        private static bool _displayWordListLength;

        //
        // The secret word the user is trying to guess
        //
        private static string _secretWord;

        //
        // The number of guesses made in the game
        //
        private static int _guessesMade;

        //
        // The maximum number of guesses allowed in the game
        //
        private static int _numberOfGuesses;

        //
        // The number of guesses left in the game play
        //
        private static int GuessesLeft => _numberOfGuesses - _guessesMade;

        //
        // Count of possibilities left for secret word
        //
        static int posCount = 1;

        //
        // List of the failed guess attempts
        //
        public static List<char> _wrongLetters = new List<char>();

        //
        // List of the correct guess attempts
        //
        public static List<char> _correctLetters = new List<char>();

        //
        // Dictionary holding the count of every correctly guessed character
        //
        public static Dictionary<char, int> correctLetterCount = new Dictionary<char, int>();

        //
        // The word in it's previous masked form according to the correctly guessed letters
        // Example: -oo-
        //
        private static string _wordMask;

        //
        //List of words most similar to the secret word
        //(Arranged according to similarity to the secret word)
        //
        public static List<string> _wordFamily;

        //
        // Masked Form of the secret Word
        // Example -oo-
        //
        private static string GetMaskedForm()
            => new string(_secretWord.Select(x => _correctLetters.Contains(x) ? x : '-').ToArray());

        //
        // Masked Form of the secret Word
        // Example -oo-
        //
        public static void StartGame()
        {
            //Clear Console
            Console.Clear();
            //Greet the player with an introduction message
            Console.WriteLine("Let's play a game...\n");
            Console.WriteLine("I, the PC will select a word.");

            GetWordFromDictionary(false); //Get a random word of specified length from the dictionary 
            GetGuessNumber(); //Get the number of guesses allowed in the game
            PromptDisplayWordList(); //Ask user if they wish to know the number of possibilities the word can be

            //Give user a recap of the choices he/she made
            Console.Clear();
            Console.WriteLine($"You have to guess a word of length {_secretWord.Length}");
            Console.WriteLine($"You have {_numberOfGuesses} guesses. Good Luck!");
            Console.WriteLine("Press any key to continue...");
            Console.Title = "Guess the word";
            Console.ReadKey();

            //Initalize or clear list containing attempted letters not contained in the word
            _wrongLetters.Clear();

            //Initalize or clear list containing attempted letters found in the word being guessed
            _correctLetters.Clear();
            correctLetterCount.Clear();

            //Ask the user questions till he/she runs out of chances
            for (_guessesMade = 0; _guessesMade < _numberOfGuesses && _wordMask.Contains("-"); _guessesMade++)
            {
                _wordMask = GetMaskedForm();
                PrintGameScreen();
            }

            if (!_wordMask.Contains("-"))
            {
                Console.WriteLine("\n\nYou are correct!!");
            }
            Console.WriteLine($"\n\nThe word was {_secretWord}");
        }

        //
        // Restart Game
        //
        public static void Restart()
        {
            Console.WriteLine("\nDo you wish to restart? ");
            Console.Write("Y/N ?");

            char choice = Console.ReadKey().KeyChar;
            if (choice != 'y' && choice != 'n')
            {
                Console.WriteLine("\nPlease input either Y or N");
                Restart(); //Ask the user if they wish to restart once more
            }
            else if (choice == 'Y' || choice == 'y')
            {
                StartGame(); // Restart Game
                Restart();
            }
            else if (choice == 'N' || choice == 'n')
            {
                Console.Clear();
                Console.WriteLine("The game will now close. Thank you for playing!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }

        //
        // Run AI rules
        //
        private static void RunRules()
        {
            //The rules the AI uses to change values
            var newWordFamily = Dictionary.GetMinMaxFamily(_secretWord, _wordMask);

            if (newWordFamily == null) return;
            if(newWordFamily.Count() != 0)
            {
                _secretWord = Dictionary.MinMaxWord;
                _wordFamily = newWordFamily.ToList();
                posCount = _wordFamily.Count;
            }

            Console.Title = _secretWord;
        }

        //
        // Get Secret word from Dictionary
        // (If askForWordLength then ask user for the length of the word)
        // 
        // A recurcive method that ensures that the user input is valid and 
        // the criteria given is met
        //
        private static void GetWordFromDictionary(bool askForWordLength)
        {
            bool properInput = false;
            int wordLength = -1;
            if (!askForWordLength)
            {
                //Prompt the user for the word length
                Console.WriteLine("\nTThere are no words of length 26 and 27 in the dictionary");
                Console.Write("\nHow many characters should the word have? ");
                properInput = int.TryParse(Console.ReadLine(), out wordLength);
            }

            //Make sure the user's input is numerical
            while (!properInput || askForWordLength)
            {
                Console.WriteLine("\n Please input a numerical value");
                Console.Write("How many characters should the word have? ");

                properInput = int.TryParse(Console.ReadLine(), out wordLength);
                if (properInput) break;
            }

            //Select a word of that specified length
            try
            {
                //Select random secret word and generate wordMask
                _secretWord = Dictionary.GetRandomWordOfLength(wordLength);
                _wordMask = GetMaskedForm();
                
                //Initalize initial word family so min max algorithm has fewer words to deal with
                _wordFamily = Dictionary.GetSimilarWords(_wordMask, _secretWord).ToList();
                posCount = _wordFamily.Count;

                //Word of specified length has been found
            }
            catch (Exception)
            {
                //If no word of specified length exists in the dictionary
                Console.WriteLine("\nI do not know a word of that length");
                Console.WriteLine("Please input a smaller value\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                //Prompt User for word length once more
                GetWordFromDictionary(true);
            }
        }

        //
        // Get the number of guesses a user wishes to have
        //
        private static void GetGuessNumber()
        {
            Console.WriteLine();
            Console.Write("How many guesses do you wish to have? ");
            int.TryParse(Console.ReadLine(), out _numberOfGuesses);


            if (_numberOfGuesses <= 0)
            {
                Console.WriteLine("\nPlease input a valid integer input\n");
                GetGuessNumber();
            }
        }

        //
        // Ask user if he/she wishes to know the number of possibilities the word can be
        //
        private static void PromptDisplayWordList()
        {
            Console.Title = "Y or N?";
            Console.WriteLine("Do you wish to know the number of possibilites of the guessed word?");
            Console.Write("Y/N ? ");

            char choice = Console.ReadKey().KeyChar;

            if (choice != 'y' && choice != 'n')
            {
                Console.WriteLine("\nPlease input either Y or N");
                PromptDisplayWordList();
            }
            else
            {
                _displayWordListLength = choice == 'y';
            }
        }

        //
        // Print game screen
        //
        private static void PrintGameScreen()
        {
            if (!_wordMask.Contains("-")) return;
            Console.Clear();
            Console.WriteLine("\n" + _wordMask + "\n");

            //Prompt user with the letters guessed by the users
            Console.WriteLine($"Attempted Letters: {string.Join(" ", _wrongLetters)}");
            Console.WriteLine($"Correct Guesses: {string.Join(" , ", _correctLetters)}");
            Console.WriteLine($"{GuessesLeft} guesses left");

            if (_displayWordListLength)
            {
                Console.WriteLine($"There are {posCount} words in the word family");
            }

            char charInput = '/';

            while (!char.IsLetter(charInput))
            {
                Console.WriteLine("\nPlease input a letter of the alphabet");
                Console.Write("Guess a letter in the word: ");
                charInput = Console.ReadKey().KeyChar;
            }


            if(_secretWord.Contains(charInput))
            {
                if (_wordFamily != null && GuessesLeft == 1 && _wordFamily.Count == 2)
                {
                    _secretWord = _wordFamily.Where(x => x != _secretWord).First();
                }
                else
                {
                    //Cheat if character is guessed correctly
                    RunRules();
                }
                
            }
            

            if (_correctLetters.Contains(charInput) || _wrongLetters.Contains(charInput))
            {
                Console.WriteLine("You have already Guessed this");
            }
            else if (_secretWord.Contains(charInput))
            {
                correctLetterCount.Add(charInput, _secretWord.Count(x => x == charInput));
                //Correct guess
                _correctLetters.Add(charInput);
                _guessesMade--;

                //Update number of possible alternatives
                posCount = _wordFamily.Count() + 1;
            }
            else
            {
                _wrongLetters.Add(charInput);
            }
        }
    }
}
