using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    public class GameEngine
    {
        //
        // If true the number of words left in the word list will be displayed
        //
        private bool _displayWordListLength;

        //
        // The secret word the user is trying to guess
        //
        private string _secretWord;

        //
        // The number of guesses made in the game
        //
        private int _guessesMade;

        //
        // The maximum number of guesses allowed in the game
        //
        private int _numberOfGuesses;

        //
        // The number of guesses left in the game play
        //
        private int GuessesLeft => _numberOfGuesses - _guessesMade;

        //
        // List of the failed guess attempts
        //
        public List<char> _wrongLetters;

        //
        // List of the correct guess attempts
        //
        public List<char> _correctLetters;

        //
        // The word in it's previous masked form according to the correctly guessed letters
        // Example: -oo-
        //
        private string wordMask;

        //
        // The last guess made by the user
        //
        private char lastGuess;

        //
        // Object for generating random objects
        //
        private readonly Random random;

        //
        // Most common letter in the word being searched for
        //
        char MostCommonLetter()
        {
            char output = '\0';
            int maxOccurance = 0;

            for (int i = 0; i < _secretWord.Length; i++)
            {
                int num = _secretWord.Count(x => x == _secretWord[i]);
                if (num > maxOccurance)
                {
                    maxOccurance = num;
                    output = _secretWord[i];
                }
            }

            return output;
        }

        //
        //List of words most similar to the secret word
        //(Arranged according to similarity to the secret word)
        //
        private List<string> _wordFamily;

        //
        // The object holding the AI
        // Also the object that handles game play
        //
        public GameEngine()
        {
            random = new Random();
        }

        //
        // Masked Form of the secret Word
        // Example -oo-
        //
        private string GetMaskedForm() 
            => new string(_secretWord.Select(x => _correctLetters.Contains(x) ? x : '-').ToArray());

        //
        // Masked Form of the secret Word
        // Example -oo-
        //
        public void StartGame()
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
            Console.ReadKey();

            //Initalize or clear list containing attempted letters not contained in the word
            if (_wrongLetters == null) _wrongLetters = new List<char>();
            else _wrongLetters.Clear();

            //Initalize or clear list containing attempted letters found in the word being guessed
            if (_correctLetters == null) _correctLetters = new List<char>();
            else _correctLetters.Clear();

            //Ask the user questions till he/she runs out of chances
            for (_guessesMade = 0; _guessesMade < _numberOfGuesses && wordMask.Contains("-"); _guessesMade++)
            {
                 wordMask = GetMaskedForm();
                _wordFamily = Program.GameDictionary.GetWordFamily(wordMask, _secretWord).ToList();
                RunRules();
                PrintGameScreen();
            }

            if(!wordMask.Contains("-"))
            {
                Console.WriteLine("You are correct!!");
            }
            Console.WriteLine($"\n\nThe word was {_secretWord}");
            Console.ReadKey();
        }

        //
        // Restart Game
        //
        public void Restart()
        {
            Console.WriteLine("\nDo you wish to restart? ");
            Console.Write("Y/N ?");

            char choice = Console.ReadKey().KeyChar;
            if (choice != 'y' && choice != 'n')
            {
                Console.WriteLine("\nPlease input either Y or N");
                Restart(); //Ask the user if they wish to restart once more
            }
            else if(choice == 'Y' || choice == 'y')
            {
                StartGame(); // Restart Game
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
        private void RunRules()
        {
            //The rules the AI uses to change values

            //Note: _wordFamily is arranged from the most probable word to the least probable word

            //Do nothing till 20% of guesses have been made

            double percentageOfGuesses = (double)_guessesMade / (double)_numberOfGuesses * 100;
            double percentageOfCorrectLetters = (double)_correctLetters.Count / (double)_secretWord.Distinct().Count() * 100;

            if (percentageOfGuesses >= 20 && percentageOfGuesses <= 25)
            {
                if(percentageOfCorrectLetters >= 26 || _wordFamily.Count < 3)
                {
                    ChangeSecretWord();
                }
            }
            else if(lastGuess == MostCommonLetter() && _wordFamily.Count > 0)
            {
                //Change secret word if the user guesses a letter that occurs frequently
                ChangeSecretWord();
            }
            else if(percentageOfGuesses >= 25 && _secretWord[0] == lastGuess)
            {
                ChangeSecretWord();
            }

            //previousWordFamilyCount = _wordFamily.Count;
            //Console.Title = _secretWord;
        }

        //
        // Change secret word to one of the possible alternatives
        //
        private void ChangeSecretWord()
        {
            int newIndex = random.Next(0, _wordFamily.Count);
            if (newIndex == _wordFamily.Count) newIndex--;

            if (_wordFamily.Count > 0)
                _secretWord = _wordFamily[newIndex];
        }


        //
        // Get Secret word from Dictionary
        // (If askForWordLength then ask user for the length of the word)
        // 
        // A recurcive method that ensures that the user input is valid and 
        // the criteria given is met
        //
        private void GetWordFromDictionary(bool askForWordLength)
        {
            bool properInput = false;
            int wordLength = -1;
            if(!askForWordLength)
            {
                //Prompt the user for the word length
                Console.WriteLine("\nThe longest word in this programs's dictionary has 24 characters");
                Console.Write("\nHow many characters should the word have? ");
                properInput = int.TryParse(Console.ReadLine(), out wordLength);
            }

            //Make sure the user's input is numerical
            while (!properInput || askForWordLength)
            {
                Console.Clear();
                Console.WriteLine("\n Please input a numerical value");
                Console.Write("How many characters should the word have? ");

                properInput = int.TryParse(Console.ReadLine(), out wordLength);
                if (properInput) break;
            }

            //Select a word of that specified length
            try
            {
                _secretWord = Program.GameDictionary.GetRandomWordOfLength(wordLength);
                //Word of specified length has been found
            }
            catch (Exception)
            {
                //If no word of specified length exists in the dictionary
                Console.Clear();
                Console.WriteLine("I do not know a word of that length");
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
        private void GetGuessNumber()
        {
            Console.WriteLine($"\nThe minimum number of guesses you can have is {_secretWord.Length}");
            Console.Write("How many guesses do you wish to have? ");
            int.TryParse(Console.ReadLine(), out _numberOfGuesses);

            if(_numberOfGuesses <= 0 || _numberOfGuesses < _secretWord.Length)
            {
                Console.WriteLine("\nPlease input a valid integer input\n");
                GetGuessNumber();
            }
        }

        //
        // Ask user if he/she wishes to know the number of possibilities the word can be
        //
        private void PromptDisplayWordList()
        {
            Console.Title = "Y or N?";
            Console.WriteLine("Do you wish to know the number of possibilites of the guessed word?");
            Console.Write("Y/N ? ");

            char choice = Console.ReadKey().KeyChar;

            if(choice != 'y' && choice != 'n')
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
        private void PrintGameScreen()
        {
            
            Console.Clear();
            Console.WriteLine("\n" + wordMask + "\n");
            
            //Prompt user with the letters guessed by the users
            Console.WriteLine($"Attempted Letters: {string.Join(" ",_wrongLetters)}");
            Console.WriteLine($"Correct Guesses: {string.Join(" , ", _correctLetters)}");
            Console.WriteLine($"{GuessesLeft} guesses left");

            if (_displayWordListLength)
            {
                int possibilities = _wordFamily.Count() + 1;
                Console.WriteLine($"There are {possibilities} words that match your guess");
            }

            char charInput = '/';

            while(!char.IsLetter(charInput))
            {
                Console.WriteLine("\nPlease input a letter of the alphabet");
                Console.Write("Guess a letter in the word: ");
                charInput = Console.ReadKey().KeyChar;
            }

            lastGuess = charInput;

            if (_correctLetters.Contains(charInput))
            {
                Console.WriteLine("You have already Guessed this");
            }
            else if(_secretWord.Contains(charInput))
            {
                _correctLetters.Add(charInput);
            }
            else
            {
                _wrongLetters.Add(charInput);
            }
        }
    }
}
