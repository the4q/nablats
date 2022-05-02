namespace Nabla.TypeScript.Tool.Interaction
{
    internal struct ConsoleColorSet
    {
        public ConsoleColorSet(ConsoleColor foreground, ConsoleColor background)
        {
            Foreground = foreground;
            Background = background;
        }

        public ConsoleColor Foreground { get; }

        public ConsoleColor Background { get; }

        public ConsoleColorSet Apply()
        {
            ConsoleColorSet current = new(Console.ForegroundColor, Console.BackgroundColor);

            Console.ForegroundColor = Foreground;
            Console.BackgroundColor = Background;

            return current;
        }
    }
}
