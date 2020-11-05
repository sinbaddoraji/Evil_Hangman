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
        private bool _displayWordListLength;
        private string _secretWord;

        private int _guessesMade;
        private int _numberOfGuesses;
        private int GuessesLeft => _numberOfGuesses - _guessesMade;
        private int UnguessedLetterCount => wordMask.ToCharArray().Count(x => x == '-');

        public List<char> _enteredLetters;
        public List<char> _foundLetters;

        string wordMask;

        //List of words most similar to the secret word
        //(Arranged according to similarity to the secret word)
        private List<string> _wordFamily;
        private List<string> _wordFamilyWithoutDoubles;

        int previousWordFamilyCount;

        private string GetStringForm()
        {
            string output = "";
            for (int i = 0; i < _secretWord.Length; i++)
            {
                if (_foundLetters.Contains(_secretWord[i]))
                {
                    output += _secretWord[i].ToString();
                }
                else
                {
                    output += "-";
                }
            }
            return output;
        }

        public void StartGame()
        {
            Console.WriteLine("Let's play a game...\n");
            Console.WriteLine("I, the PC will select a word.");

            GetWordFromDictionary(false); //Get a random word of specified length from the dictionary 
            GetGuessNumber(); //Get the number of guesses allowed in the game
            PromptDisplayWordList(); //Ask user if they wish to know the number of possibilities the word can be

            Console.Clear();
            Console.WriteLine($"You have to guess a word of length {_secretWord.Length}");
            Console.WriteLine($"You have {_numberOfGuesses} guesses. Good Luck!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            //Initalize or clear list containing attempted letters not contained in the word
            if (_enteredLetters == null) _enteredLetters = new List<char>();
            else _enteredLetters.Clear();

            //Initalize or clear list containing attempted letters found in the word being guessed
            if (_foundLetters == null) _foundLetters = new List<char>();
            else _foundLetters.Clear();

            Console.Title = _secretWord;

            //Ask the user questions till he/she runs out of chances
            for (_guessesMade = 0; _guessesMade < _numberOfGuesses; _guessesMade++)
            {
                 wordMask = GetStringForm();
                _wordFamily = Program.GameDictionary.GetWordFamily(wordMask, _secretWord).ToList();
                _wordFamilyWithoutDoubles = Program.GameDictionary.GetWordFamily(wordMask, _secretWord, true).ToList();
                RunRules();
                PrintGameScreen();
            }

            Console.WriteLine($"\n\nThe word was {_secretWord}");
            Console.ReadKey();
        }

        public void Restart()
        {
            Console.WriteLine("\nDo you wish to restart? ");
            Console.Write("Y/N ?");

            char choice = Console.ReadKey().KeyChar;
            if (choice != 'y' && choice != 'n')
            {
                Console.WriteLine("\nPlease input either Y or N");
                Restart();
            }
            else
            {
                Console.Clear();
                StartGame();
            }
        }

        private void RunRules()
        {
            //The rules the AI uses to change values

            if(_wordFamily.Count < previousWordFamilyCount && _wordFamily.Count > 2)
            {
                //Change secret word to the least similar to it in the word family
               // _secretWord = _wordFamily.First();
            }

            if(Program.GameDictionary.HasDoubleLetters(_secretWord) && UnguessedLetterCount > _secretWord.Length /2)
            {
                if(_wordFamilyWithoutDoubles.Count > 0)
                {
                    _secretWord = _wordFamilyWithoutDoubles[0];
                }
            }

            if(UnguessedLetterCount <= 2 && _wordFamily.Count > 0)
            {
                _secretWord = _wordFamilyWithoutDoubles[0];
            }

            //Avoid repeating letters
            previousWordFamilyCount = _wordFamily.Count;
            Console.Title = _secretWord;
        }

        private void GetWordFromDictionary(bool doAsk)
        {
            bool properInput = false;
            int wordLength = -1;
            if(!doAsk)
            {
                //Prompt the user for the word length
                Console.Write("\nHow many characters should the word have? ");
                properInput = int.TryParse(Console.ReadLine(), out wordLength);
            }

            //Make sure the user's input is numerical
            while (!properInput || doAsk)
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
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("I do not know a word of that length");
                Console.WriteLine("Please input a smaller value\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                //Prompt User for word length once more
                GetWordFromDictionary(true);
            }
        }

        private void GetGuessNumber()
        {
            Console.Write("How many guesses do you wish to have? ");
            int.TryParse(Console.ReadLine(), out _numberOfGuesses);

            if(_numberOfGuesses <= 0)
            {
                Console.WriteLine("\nPlease input a valid integer input\n");
                GetGuessNumber();
            }
        }

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

        private void PrintGameScreen()
        {
            
            Console.Clear();
            Console.WriteLine("\n" + wordMask + "\n");
            
            //Prompt user with the letters guessed by the users
            Console.WriteLine($"Attempted Letters: {string.Join(" ",_enteredLetters)}");
            Console.WriteLine($"Correct Guesses: {string.Join(" , ", _foundLetters)}");
            Console.WriteLine($"{GuessesLeft} guesses left");

            if (_displayWordListLength)
            {
                int possibilities = _wordFamily.Count();
                Console.WriteLine($"There are {possibilities} words that match your guess");
            }

            char charInput = '/';

            while(!char.IsLetter(charInput))
            {
                Console.WriteLine("\nPlease input a letter of the alphabet");
                Console.Write("Guess a letter in the word: ");
                charInput = Console.ReadKey().KeyChar;
            }

            if (_foundLetters.Contains(charInput))
            {
                Console.WriteLine("You have already Guessed this");
            }
            else if(_secretWord.Contains(charInput))
            {
                _foundLetters.Add(charInput);
            }
            else
            {
                _enteredLetters.Add(charInput);
            }
        }
    }
}
