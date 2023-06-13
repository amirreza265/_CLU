using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoConsole.Classes
{
    public delegate void CommandFunc(string[] args);
    public class CommandLineUtility
    {
        public static CommandLineUtility Instance = new();

        public Dictionary<string, CommandFunc> Commands { get; set; }
        public Dictionary<string, string> Variables { get; set; }
        public string LastOut { get => lastOut; }

        private event Action<string> OnPrint;
        private string lastOut;
        private bool piped = false;
        public CommandLineUtility()
        {
            Commands = new();
            Variables = new();

            OnPrint += (str) =>
            {
                lastOut = str;
            };
        }


        private ConsoleColor _errColor = ConsoleColor.Red;
        private ConsoleColor _inpColor = ConsoleColor.Green;
        private ConsoleColor _infoColor = ConsoleColor.Cyan;
        private ConsoleColor _hintColor = ConsoleColor.DarkGray;
        private ConsoleColor _dfltColor = ConsoleColor.White;

        public void AddCommand(string commandName, CommandFunc func)
        {
            if (Commands.ContainsKey(commandName))
                return;
            Commands.Add(commandName, func);
        }

        public void RunCommand(string commandName, params string[] args)
        {
            if (Commands.ContainsKey(commandName))
                Commands[commandName](args);
            else
                ShowError($"{commandName} not found ...");
        }

        public void ShowError(string message)
        {
            Out(message, _errColor);
        }

        public void Print(string message, ConsoleColor color = ConsoleColor.White, bool line = true)
        {
            if (!piped)
            {
                Out(message, color, line);
            }

            OnPrint(message);
        }

        private static void Out(string message, ConsoleColor color = ConsoleColor.White, bool line = true)
        {
            Console.ForegroundColor = color;
            string newLine = line ? "\n" : "";
            Console.Write($"{message}{newLine}");
            Console.ResetColor();
        }

        public string ReadInput(string label = "")
        {
            Out($"{label} > ", _inpColor, false);
            return Console.ReadLine() ?? "";
        }

        public void RunProgram()
        {
            Console.ForegroundColor = _hintColor;
            Out("Enter your Command for more info enter \'help\'", ConsoleColor.Gray);
            while (true)
            {
                Console.ForegroundColor = _inpColor;
                string input = ReadInput("PC");

                if (input is "close" or "exit" or "q")
                    return;

                //check for pipe
                FetchRunCommand(input);

                //piped = false;
                lastOut = "";
            }
        }

        public void FetchRunCommand(string command, bool pipe = false)
        {
            if (string.IsNullOrEmpty(command))
            {
                lastOut = "";
                piped = false;
                return;
            }

            string[] inputs = command.TrimStart().Split(" ");

            if (inputs.Length == 0)
                return;

            if (inputs.Length == 1)
                if (inputs[0].Contains("="))
                {
                    var vars = inputs[0].Split("=");

                    if (Variables.ContainsKey(vars[0]))
                        Variables[vars[0]] = vars[1];
                    else
                        Variables.Add(vars[0], vars[1]);
                    return;
                }
                else if (inputs[0].StartsWith("$"))
                {
                    Print(Variables[inputs[0][1..]]);
                    return;
                }


            Console.ForegroundColor = _dfltColor;

            piped = pipe;

            RunCommand(inputs[0], inputs[1..]);

            piped = false;
        }
    }
}
