using CommandLine;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System;

namespace hammer_backstock
{
    class Program
    {
        [Option('n', "Custom", Required = false, HelpText = "Configuration name")]
        public string myName { get; set; }

        [Option('s', "CmdSeq_custom.wc", Required = false, HelpText = "Command Sequence")]
        public string wcFile { get; set; }

        [Option('c', "GameConfig_custom.txt", Required = false, HelpText = "Config file")]
        public string gameConfigFile { get; set; }

        [Option('e', "hammer.exe", Required = false, HelpText = "Hammer executable")]
        public string hammerExe { get; set; }


        static void Main(string[] args)
        {
            //var options = new MeerkatOptions();
            CommandLine.Parser parser = new CommandLine.Parser();

            // Instantiate our AI
            // Command line arguments will be passed in as properties through parser.ParseArguments
            var hammer = new Program();

            if (!parser.ParseArguments(args, hammer))
            {
                //log.Error("Command line arguments could not be parsed");
            }

            hammer.runHammer();

        }


        public void runHammer()
        {
            try
            {
                // Ensure I'm the only hammer
                if (Process.GetProcessesByName("hammer").Count() > 0)
                {
                    MessageBox.Show(this.myName + " Hammer can only support one instance at a time");
                    return;
                }

                // Move configurations into place
                if (File.Exists("CmdSeq.wc")) File.Copy("CmdSeq.wc", "CmdSeq_original.wc", true);
                if (File.Exists("CmdSeq_backstock.wc")) File.Copy(this.wcFile, "CmdSeq.wc", true);

                if (File.Exists("GameConfig.txt")) File.Copy("GameConfig.txt", "GameConfig_original.txt", true);
                if (File.Exists(gameConfigFile)) File.Copy(this.gameConfigFile, "GameConfig.txt", true);

                //Start hammer
                var ccTagIntermediateCompilerProcess = new Process();
                ccTagIntermediateCompilerProcess.StartInfo.FileName = hammerExe;
                ccTagIntermediateCompilerProcess.StartInfo.Arguments = "-nop4";
                ccTagIntermediateCompilerProcess.Start();
                ccTagIntermediateCompilerProcess.WaitForExit();

                // Restore original configurations

                if (File.Exists("CmdSeq.wc")) File.Copy("CmdSeq.wc", wcFile, true);
                if (File.Exists("CmdSeq_original.wc"))
                {
                    File.Copy("CmdSeq_original.wc", "CmdSeq.wc", true);
                    File.Delete("CmdSeq_original.wc");
                }

                if (File.Exists("GameConfig.txt")) File.Copy("GameConfig.txt", this.gameConfigFile, true);
                if (File.Exists("GameConfig_original.txt"))
                {
                    File.Copy("GameConfig_original.txt", "GameConfig.txt", true);
                    File.Delete("GameConfig_original.txt");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("hammer_custom_exceptions.txt", ex.ToString() + Environment.NewLine + Environment.NewLine +
                    "================================================================================================================================");
            }
        }
    }
}
