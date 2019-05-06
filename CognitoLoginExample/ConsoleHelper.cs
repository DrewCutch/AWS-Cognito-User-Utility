using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitoLoginExample
{
    static class ConsoleHelper
    {
        public static int MultipleChoice(string title, params string[] options)
        {
            int currentSelection = 0;
            ConsoleKeyInfo key;
            Console.CursorVisible = false;
            do
            {
                Console.Clear();
                Console.WriteLine(title + ":");
                Console.ForegroundColor = ConsoleColor.Gray;
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == currentSelection)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  ");
                    }

                    Console.WriteLine($"[{i}] {options[i]}");
                    Console.ResetColor();
                }

                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                    {
                        if (currentSelection > 0)
                            currentSelection--;
                        break;
                    }
                    case ConsoleKey.DownArrow:
                    {
                        if (currentSelection < options.Length - 1)
                            currentSelection++;
                        break;
                    }
                    default:
                    {
                        if (Int32.TryParse(key.KeyChar.ToString(), out int num) && num < options.Length)
                        {
                            currentSelection = num;
                        }
                        break;
                    }
                }
                

            } while (key.Key != ConsoleKey.Enter);

            Console.CursorVisible = true;
            return currentSelection;
        }

        public static bool TryAgain()
        {
            Console.WriteLine("Try Again? [y/n]");
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                    return true;
                if (key == ConsoleKey.N)
                    return false;
            }
        }

        public static string Prompt(string prompt, bool sameLine=true)
        {
            if(sameLine)
                Console.Write($"{prompt}: ");
            else
                Console.WriteLine($"{prompt}: ");
            return Console.ReadLine();

        }

        public static void PrintSuccess(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void PrintError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
