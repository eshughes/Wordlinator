namespace Wordlinator 
{
    public class GuessInfo
    {
        public char? CorrectGuess { get; set; }
        public List<char> WrongPositionGuesses { get; set; }

        public GuessInfo() {
            WrongPositionGuesses = new List<char>();
        }
    }
}