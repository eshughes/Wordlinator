using System.Text;
using System.Text.RegularExpressions;

namespace Wordlinator
{
    class Program
    {
        private static readonly int wordLength = 5;
        private static readonly List<char> allLetters = new List<char>() {
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Greetings programs. I am the Wordlinator! Tremble at my powers of word suggestion!");
            Console.WriteLine("Let's talk about the letters you have already.");

            var guesses = new List<GuessInfo>();
            var badLetters = new List<char>();

            for (int i = 1; i < wordLength + 1; i++) 
            {
                var guess = new GuessInfo();

                Console.WriteLine($"________________ Position {i} ________________");
                Console.WriteLine($"Do you have a green letter for position {i}? (enter for no)");

                var inputString = Console.ReadLine();
                if (inputString != null && inputString.Any(x => Char.IsLetter(x)))
                {
                    guess.CorrectGuess = Char.ToLower(inputString.ToCharArray().Last(x => Char.IsLetter(x)));
                }

                Console.WriteLine($"Do you have any yellow letters for position {i}? (enter all on one line)");

                inputString = Console.ReadLine();
                if (inputString != null)
                {
                    guess.WrongPositionGuesses = inputString.ToCharArray().Where(x => Char.IsLetter(x)).Select(x => Char.ToLower(x)).ToList();
                }
                
                Console.WriteLine($"Do you have any gray letters for position {i}? (enter all on one line)");
                
                inputString = Console.ReadLine();
                if (inputString != null)
                {
                    badLetters.AddRange(inputString.ToCharArray().Where(x => Char.IsLetter(x)).Select(x => Char.ToLower(x)).ToList());
                }

                guesses.Add(guess);
            }


            var correctGuesses = guesses.Where(x => x.CorrectGuess != null).Select(x => x.CorrectGuess.Value).ToList();
            var misplacedLetters = guesses.SelectMany(x => x.WrongPositionGuesses).ToList();

            var confirmedLetters = correctGuesses.Concat(misplacedLetters).Distinct();

            Console.WriteLine($"______________________________________________");

            StringBuilder sb = new StringBuilder();
            sb.Append("\\b");
            sb.Append("(?=");

            foreach (var guess in guesses)
            {
                if (guess.CorrectGuess != null)
                {
                    sb.Append($"{guess.CorrectGuess}");
                }
                else if (guess.WrongPositionGuesses.Any() || badLetters.Any()) 
                {
                    var possibleLetters = allLetters.Except(badLetters).Except(guess.WrongPositionGuesses);
                    var possibleLetterString = string.Join("", possibleLetters);
                    sb.Append($"[{possibleLetterString}]");
                }
                else
                {
                    sb.Append($"[a-z]");
                }
            }
            sb.Append(")");

            foreach (var misplacedLetter in misplacedLetters) {
                sb.Append($"(?=.*{misplacedLetter}.*)");
            }

            sb.Append("\\b");

            var dictionary = System.IO.File.ReadAllText(@"..\\..\\..\\dictionary.csv");
            var pattern = sb.ToString();
            Console.WriteLine($"{pattern}");

            var regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            var regex = new Regex(pattern, regexOptions);
            
            Console.WriteLine($"Consider the following:");

            foreach (Match match in regex.Matches(dictionary)) {
                Console.WriteLine(dictionary.Substring(match.Index, wordLength));
            }
        }
    }
}