using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaptorBasic
{
    class Compiler
    {
        public Lexer lexer;
        public List<Command> commands;
        public Compiler(string source)
        {
            lexer = new Lexer(source);
            commands = lexer.GetCommands();
        }
        public string CompileWindows32()
        {
            string data = "\n[extern printf]\n[extern scanf]\n[extern exit]\nsection .data\nint_string:\ndw `%d`,0\nread_in_st:\ndw `%s`,0\n";
            string text = "section .text\n" + "global main\n" +
                "main:\n";
            string bss = "section .bss\n";
            string exit = "push 0\n" +
                "call exit\n";
            int prt = 0;

            List<string> Lables = new List<string>();
            Lables.Add("main");
            List<(string name, TokenType type)> Variables = new List<(string name, TokenType type)>();
            foreach (Command cmd in commands)
            {
                switch (cmd.name)
                {
                    #region if

                    case "if":
                        {
                            if (cmd.args[0].tokenType == TokenType.word)
                            {
                                foreach (var Var in Variables)
                                {
                                    if (Var.name == cmd.args[0].Value.ToString() && Var.type == TokenType.number)
                                    {
                                        if (cmd.args[2].tokenType == TokenType.word)
                                        {
                                            foreach (var V in Variables)
                                            {
                                                if (V.name == cmd.args[2].Value.ToString())
                                                {
                                                    text += $"cmp [{Var.name}], dword {V.name}\n";
                                                    switch (cmd.args[1].Value.ToString())
                                                    {
                                                            case "=":
                                                            {
                                                                text += $"je {cmd.args[3].Value}\n";
                                                            }
                                                            break;
                                                        case "!=":
                                                            {
                                                                text += $"jne {cmd.args[3].Value}\n";
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    #endregion
                    #region dec 
                    case "dec":
                        {
                            if (cmd.args[0].tokenType == TokenType.word)
                            {
                                switch (cmd.args[0].Value.ToString())
                                {
                                    case "int":
                                        {
                                            if (cmd.args.Count == 1)
                                            {
                                                //error
                                            }
                                            else if (cmd.args.Count == 2)
                                            {
                                                string name = cmd.args[1].Value.ToString();
                                                Variables.Add((name, TokenType.number));
                                                bss += $"{name}:\n" +
                                                    $"\tresd 1\n";

                                            }
                                        }
                                        break;
                                    case "array":
                                        {
                                            if (cmd.args.Count != 3)
                                            {
                                                //error
                                            }
                                            else
                                            {
                                                string name = cmd.args[1].Value.ToString();
                                                Variables.Add((name, TokenType.str));
                                                if (cmd.args[2].tokenType == TokenType.number)
                                                {
                                                    int size = int.Parse(cmd.args[2].Value.ToString());
                                                    bss += $"{name}:\nresb {size}\n";
                                                }

                                            }
                                        }
                                        break;
                                    case "string":
                                        {
                                            if (cmd.args.Count != 3)
                                            {
                                                //error
                                            }
                                            else
                                            {
                                                string name = cmd.args[1].Value.ToString();
                                                Variables.Add((name, TokenType.str));
                                                if (cmd.args[2].tokenType == TokenType.number)
                                                {
                                                    int size = int.Parse(cmd.args[2].Value.ToString());
                                                    bss += $"{name}:\nresb {size}\n";
                                                    bss += $"{name}_end:\n";
                                                }

                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    #endregion
                    #region let
                    case "let":
                        {
                            if (cmd.args[0].tokenType == TokenType.word)
                            {
                                string name = cmd.args[0].Value.ToString();
                                foreach (var Var in Variables)
                                {
                                    if (Var.name == name)
                                    {
                                        //error
                                        break;
                                    }
                                }
                                if (cmd.args.Count == 1)
                                {
                                    //error
                                }
                                else
                                {
                                    if (cmd.args[1].Value.ToString() == "=")
                                    {
                                        if (cmd.args[2].tokenType == TokenType.str)
                                        {
                                            Variables.Add((name, TokenType.str));
                                            data += $"{name}:\n\tdw `{cmd.args[2].Value.ToString()}`,0\n";
                                        }
                                        else if (cmd.args[2].tokenType == TokenType.number)
                                        {
                                            Variables.Add((name, TokenType.number));
                                            data += $"{name}:\n\tdw {cmd.args[2].Value}\n";
                                        }
                                        else
                                        {
                                            //error
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                    #region label
                    case "label":
                        {
                            if (cmd.args[0].tokenType == TokenType.word)
                            {
                                Lables.Add(cmd.args[0].Value.ToString());
                                text += $"{cmd.args[0].Value.ToString()}:\n";
                            }
                            else
                            {
                                //error
                            }
                        }
                        break;
                    #endregion
                    #region go_to
                    case "goto":
                        {
                            if (cmd.args[0].tokenType == TokenType.word)
                            {
                                foreach (string label in Lables)
                                {
                                    if (label == cmd.args[0].Value.ToString())
                                    {
                                        text += $"jmp {label}\n";
                                    }
                                }
                                //error
                            }
                        }
                        break;
                    #endregion
                    #region print
                    case "print":
                        {
                            if (cmd.args[0].tokenType == TokenType.str)
                            {
                                ++prt;
                                data += $"prt_str{prt}:\n";
                                data += $"\tdw `{cmd.args[0].Value.ToString()}`,0\n";
                                text += $"push prt_str{prt}\n" +
                                            $"call printf\n";
                            }
                            else if (cmd.args[0].tokenType == TokenType.word)
                            {
                                string var = cmd.args[0].Value.ToString();
                                foreach (var Var in Variables)
                                {
                                    if (Var.type == TokenType.str && Var.name == var)
                                    {
                                        text += $"push {Var.name}\n" +
                                            $"call printf\n";
                                    }
                                    else if (Var.name == var)
                                    {
                                        text += $"push dword [{Var.name}]\n" +
                                            "push dword int_string\n" +
                                            $"call printf\n";
                                    }
                                }

                            }

                        }
                        break;
                    #endregion
                    #region asm
                    case "asm":
                        {
                            if (cmd.args[0].tokenType == TokenType.str)
                            {
                                text += cmd.args[0].Value.ToString();
                            }
                            else
                            {
                                //error
                            }
                        }
                        break;
                    #endregion
                    #region input
                    case "input":
                        {
                            if (cmd.args[0].tokenType == TokenType.word)
                            {
                                foreach (var Var in Variables)
                                {
                                    if (Var.name == cmd.args[0].Value.ToString() && Var.type == TokenType.str)
                                    {
                                        text += $"push {Var.name}\n" +
                                        "push read_in_st\n" +
                                        "call scanf\n";
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                    default:
                        {
                            foreach (var Var in Variables)
                            {
                                if (Var.name == cmd.name)
                                {
                                    if (cmd.args[0].Value.ToString() == "=")
                                    {
                                        if (cmd.args[1].tokenType == TokenType.word)
                                        {
                                            string nm = cmd.args[1].Value.ToString();
                                            foreach (var _Var in Variables)
                                            {
                                                if (nm == _Var.name && Var.type == _Var.type)
                                                {
                                                    if (cmd.args[2].Value.ToString() == "[")
                                                    {
                                                        int index = int.Parse(cmd.args[1].Value.ToString());
                                                        text += "push eax\n" +
                                                           $"mov eax, [{nm} + {index}]\n" +
                                                           $"mov [{Var.name}], eax\n";
                                                    }
                                                    if (_Var.type == TokenType.str)
                                                    {
                                                        text += "push eax\n" +
                                                            $"mov eax, [{nm}]\n" +
                                                            $"mov [{Var.name}], eax\n";
                                                    }
                                                    else
                                                    {
                                                        text += "push eax\n" +
                                                            $"mov eax, [{nm}]\n" +
                                                            $"mov [{Var.name}], eax\n";
                                                    }
                                                }
                                            }
                                            //error;
                                        }

                                    }
                                    else if (cmd.args[0].Value.ToString() == "[")
                                    {
                                        if (cmd.args[1].tokenType == TokenType.word)
                                        {
                                            string nm = cmd.args[1].Value.ToString();
                                            foreach (var Variable in Variables)
                                            {
                                                if (Variable.name == nm)
                                                {
                                                    if (cmd.args[3].Value.ToString() == "=")
                                                    {
                                                        string s = cmd.args[4].Value.ToString();

                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {
                                            int index = 0;
                                            if (cmd.args[1].tokenType == TokenType.number)
                                            {
                                                index = int.Parse(cmd.args[1].Value.ToString());
                                                if (cmd.args[3].Value.ToString() == "=")
                                                {
                                                    string nm = cmd.args[4].Value.ToString();
                                                    foreach (var _Var in Variables)
                                                    {
                                                        if (nm == _Var.name)
                                                        {
                                                            text += "push eax\n" +
                                                            $"mov eax,  [{nm} +     0]\n" +
                                                            $"mov [{Var.name} + {index}],  eax\n" +
                                                            "pop eax\n";
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }

                                //error
                            }
                            break;

                        }
                }
            }
            return bss + data + text + exit;
        }
    }
}
