using Nabla.TypeScript.Tool.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool
{
    public static class UI
    {
        public static IUserInterface Interface { get; set; } = ConsoleUI.Instance;

        public static bool Silent { get; set; }

        public static bool DebugMode { get; set; }

        public static void Say(string message, Severity severity = default, bool newLine = true)
        {
            if (!Silent
#if DEBUG
                || DebugMode
#endif 
                )
                Interface.Output(message, severity, newLine);
        }

        public static void NewLine()
            => Say(string.Empty);

        public static void Info(string message)
            => Say(message, Severity.Information);

        public static void Success(string message)
            => Say(message, Severity.Success);

        public static void Warning(string message)
            => Say(message, Severity.Warning);

        public static void Error(string message)
            => Say(message, Severity.Danger);

        public static void Comment(string message)
            => Say(message, Severity.Comment);

        public static void Blank()
            => Say(string.Empty);

        public static void Pause(string? prompt = null)
        {
            if (!Silent)
                Interface.InputChar(prompt ?? "Press any key to continue", null, echo: false);
        }

        private static readonly char[] _confirmationOptions = new[] { 'y', 'n' };

        public static bool Confirm(string prompt, bool defaultOption = true)
        {
            if (Silent)
                return defaultOption;

            char? answer = Interface.InputChar(prompt, defaultOption ? 0 : 1, true, _confirmationOptions);
            return answer.Value == char.ToLower(_confirmationOptions[0]);
        }
    }


}
