using System.Diagnostics.CodeAnalysis;

namespace Nabla.TypeScript.Tool.Interaction
{
    public class ConsoleUI : IUserInterface
    {
        readonly static Dictionary<Severity, ConsoleColorSet> _palette = new()
        {
            [Severity.Comment] = new(ConsoleColor.DarkGray, ConsoleColor.Black),
            [Severity.Information] = new(ConsoleColor.Blue, ConsoleColor.Black),
            [Severity.Confirmation] = new(ConsoleColor.White, ConsoleColor.Black),
            [Severity.Success] = new(ConsoleColor.Green, ConsoleColor.Black),
            [Severity.Warning] = new(ConsoleColor.DarkYellow, ConsoleColor.Black),
            [Severity.Danger] = new(ConsoleColor.White, ConsoleColor.DarkRed),
        };

        public readonly static ConsoleUI Instance = new();

        private ConsoleUI()
        {

        }

        [return: NotNullIfNotNull("defaultOption")]
        public string? InputString(string prompt, int? defaultOption, params string[] candidates)
        {
            throw new NotImplementedException();
        }

        public void Output(string message, Severity severity, bool newLine)
        {
            ConsoleColorSet? current;

            if (_palette.TryGetValue(severity, out var set))
            {
                current = set.Apply();
            }
            else
                current = null;

            Console.Write(message);
            if (newLine)
                Console.WriteLine();

            current?.Apply();
        }

        [return: NotNullIfNotNull("defaultOption")]
        public char? InputChar(string prompt, int? defaultOption, bool echo = true, params char[] options)
        {
        retry:
            Output(prompt, Severity.Confirmation, false);

            if (options.Any())
            {
                Console.Write('[');
                for (int i = 0; i < options.Length; i++)
                {
                    char c = options[i];
                    if (i > 0)
                        Console.Write('/');

                    if (i == defaultOption)
                        c = char.ToUpper(c);
                    else
                        c = char.ToLower(c);

                    Console.Write(c);
                }
                Console.Write(']');
            }

            var info = Console.ReadKey(true);

            char? ch = info.KeyChar;

            if (options.Any())
            {
                if (info.Key == ConsoleKey.Enter)
                {
                    if (defaultOption != null)
                    {
                        ch = options[defaultOption.Value];
                    }
                    else
                        ch = null;
                }
                else
                {
                    if (!options.Any(x => char.ToLower(x) == char.ToLower(ch.Value)))
                    {
                        if (echo)
                            Console.Write(ch.Value);
                        EnsureBeginLine();
                        Output("Invalid input, please try again.", Severity.Danger, true);
                        goto retry;
                    }
                }
            }

            if (echo)
                Console.Write(ch);
            EnsureBeginLine();

            return ch;
        }

        private static void EnsureBeginLine()
        {
            if (Console.CursorLeft > 0)
                Console.WriteLine();
        }
    }
}
