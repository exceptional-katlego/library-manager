namespace LibraryManager.Helpers
{
    /// <summary>
    /// Safe ways to read input from the user.
    /// These keep asking until the input is valid, so the program never crashes on bad input.
    /// </summary>
    public static class InputHelper
    {
        public static string ReadText(string prompt)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (line == null)
            {
                // Input stream closed (for example Ctrl+Z / Ctrl+D). Exit cleanly
                // instead of looping forever on empty input.
                Console.WriteLine();
                Console.WriteLine("Input ended. Goodbye!");
                Environment.Exit(0);
            }
            return line;
        }

        public static string ReadRequiredText(string prompt)
        {
            while (true)
            {
                string input = ReadText(prompt).Trim();
                if (input.Length > 0)
                {
                    return input;
                }
                WriteError("This cannot be empty. Please try again.");
            }
        }

        public static int ReadInt(string prompt)
        {
            while (true)
            {
                string input = ReadText(prompt);
                // TryParse returns false instead of throwing a FormatException.
                if (int.TryParse(input, out int number))
                {
                    return number;
                }
                WriteError($"\"{input}\" is not a whole number. Please try again.");
            }
        }

        public static bool Confirm(string prompt)
        {
            string answer = ReadText($"{prompt} (y/n): ").Trim().ToLower();
            return answer == "y" || answer == "yes";
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
