using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoConsole.Classes
{
    public static class CommandLineCommandsExtension
    {
        public static void AddDefaultCommands(this CommandLineUtility clu)
        {
            clu.AddCommand("help", args =>
            {
                Console.WriteLine("help  => show summery of commands");
                Console.WriteLine("print => print args");
                Console.WriteLine("clr   => clear screen");
            });


            clu.AddCommand("print", args =>
            {
                clu.Print(string.Join(" ", args));
            });

            clu.AddCommand("clr", arg => { Console.Clear(); });

            clu.AddCommand("[[", args =>
            {
                if (args.Length < 4)
                    clu.ShowError("syntax incorrect..");

                var first = args[0];
                var op = args[1];
                var second = args[2];
                var finish = args[3];

                if (finish != "]]")
                    clu.ShowError("missing ]] at the end of Condition");

                bool result = false;
                if (op.StartsWith("-"))
                {
                    double firstd = 0; 
                    bool err = !double.TryParse(first,out firstd);
                    double secondd = 0; 
                    err = !double.TryParse(second,out secondd);

                    if (err)
                    {
                        clu.ShowError
                        ($"operand of {op} must be a number!");
                        return;
                    }

                    switch (op[1..])
                    {
                        case "eq":
                            result = firstd == secondd;
                            break;
                        case "nq":
                            result = firstd != secondd;
                            break;
                        case "gt":
                            result = firstd > secondd;
                            break;
                        case "lw":
                            result = firstd < secondd;
                            break;
                        case "ge":
                            result = firstd >= secondd;
                            break;
                        case "le":
                            result = firstd <= secondd;
                            break;
                        case "dv":
                            result = firstd % secondd == 0;
                            break;
                    }
                }
                else
                    switch (op)
                    {
                        case "=":
                            result = string.Compare(first, second) == 0;
                            break;
                        case "!=":
                            result = string.Compare(first, second) != 0;
                            break;
                        case ">":
                            result = string.Compare(first, second) > 0;
                            break;
                        case "<":
                            result = string.Compare(first, second) < 0;
                            break;
                        case ">=":
                            result = string.Compare(first, second) >= 0;
                            break;
                        case "<=":
                            result = string.Compare(first, second) <= 0;
                            break;
                        default:
                            clu.ShowError($"operation \'{op}\' not found ...");
                            return;
                    }

                clu.Print(result.ToString());
            });

            clu.AddCommand("if", args =>
            {
                clu.FetchRunCommand(string.Join(" ", args[..5]), pipe: true);

                if (args[5] != "{")
                    clu.ShowError("missing { after condition");
                if (args[args.Length - 1] != "}")
                    clu.ShowError("missing } after codes");

                int last = args.Length - 1;
                var code = string.Join(" ", args[6..last]);
                if (clu.LastOut == "True")
                {
                    var comands = code.Split(';');
                    foreach (var comand in comands)
                    {
                        clu.FetchRunCommand(comand.Trim());
                    }
                }
            });

        }
    }
}
