using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<char> _wrongLetters = new List<char>();

        //
        // List of the correct guess attempts
        //
        public List<char> _correctLetters = new List<char>();

        //
        // The word in it's previous masked form according to the correctly guessed letters
        // Example: -oo-
        //
        private string _wordMask;

        //
        // Object for generating random objects
        //
        private readonly Random random;

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
            Console.Title = "Guess the word";
            Console.ReadKey();

            //Initalize or clear list containing attempted letters not contained in the word
            _wrongLetters.Clear();

            //Initalize or clear list containing attempted letters found in the word being guessed
            _correctLetters.Clear();

            //Reset word possibility count
            posCount = Program.GameDictionary.WordList.Count;

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
        private void RunRules()
        {
            //The rules the AI uses to change values
            var newWordFamily = Program.GameDictionary.GetMinMaxFamily(_secretWord, _wordMask, _wrongLetters, _correctLetters, _wordFamily);

            if (newWordFamily == null) return;
            if(newWordFamily.Count() != 0)
            {
                _wordFamily = newWordFamily.ToList();
                _secretWord = Program.GameDictionary.MinMaxWord;
                ChangeSecretWord();
                posCount = _wordFamily.Count;
            }
            
            Console.Title = (_secretWord);
        }

        //
        // Change secret word to one of the possible alternatives
        //
        private void ChangeSecretWord()
        {
            if (_wordFamily == null)
                return;

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
                _secretWord = Program.GameDictionary.GetRandomWordOfLength(wordLength);
                _wordMask = GetMaskedForm();
                
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

            RunRules();
        }

        //
        // Get the number of guesses a user wishes to have
        //
        private void GetGuessNumber()
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
        private void PromptDisplayWordList()
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

        int posCount = 1;
        //
        // Print game screen
        //
        private void PrintGameScreen()
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
                Console.WriteLine($"There are {posCount} words that match your guess");
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
                //Cheat if character is guessed correctly
                RunRules();
            }
            

            if (_correctLetters.Contains(charInput) || _wrongLetters.Contains(charInput))
            {
                Console.WriteLine("You have already Guessed this");
            }
            else if (_secretWord.Contains(charInput))
            {
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
