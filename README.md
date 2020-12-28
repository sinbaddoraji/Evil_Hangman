## Evil Hangman
This respository holds code for assignment for a University module called "Artificial Intelligence". It is a word guessing game (a console application) where the computer attempts to cheat as the player/user attempts to guess the random word selected by player 1 (PC).

The game cheats by creating word families based on correct guesses already made and guesses not yet made. It selects the largest word family based on the guess (The word family containing the most likely guesses that can be made by the human player. The secret word is then replaced by one of the least likely words from this word family. This algorithm is a modification of the Minimax algorithm 
