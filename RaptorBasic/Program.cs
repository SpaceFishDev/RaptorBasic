    


using System.Diagnostics;

namespace RaptorBasic
{
    
        internal class Program
        {
            public const double Version = 0.1;
            static void Main(string[] args)
            {
                int i = 0;
                string outfp = "out.exe ";
                string infp = "";
                while (i != args.Length)
                {
                    if (args[i] == "-o")
                    {
                        outfp = args[i + 1];
                        ++i;
                    }
                    else
                    {
                        infp = args[i];
                    }
                    ++i;
                }
                if (outfp == "out.exe")
                {
                    outfp = "out.exe";
                }
                if (infp == "")
                {
                    infp = "main.Rbasic";
                }
                string src = File.ReadAllText(infp);
                Compiler compiler = new Compiler(src);
                string output = compiler.CompileWindows32();
                Console.WriteLine($"RaptorBasic: V{Version}. \nOutputFile: {outfp}\nInputFile: {infp}\n");
                File.WriteAllText("out.asm", output);
                Process.Start("nasm", $"out.asm -f win32 -o {outfp.Replace(".exe", ".obj")}");
                Thread.Sleep(300);
                Process.Start("golink", $"/entry:main /console kernel32.dll user32.dll msvcrt.dll {outfp.Replace(".exe", ".obj")}");
            }
        }
    }
