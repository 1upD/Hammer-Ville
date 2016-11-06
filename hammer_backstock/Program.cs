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
        [Option('n', DefaultValue = "Custom", Required = false, HelpText = "Configuration name")]
        public string myName { get; set; }

        [Option('s', DefaultValue = "CmdSeq_custom.wc", Required = false, HelpText = "Command Sequence")]
        public string wcFile { get; set; }

        [Option('c', DefaultValue = "GameConfig_custom.txt", Required = false, HelpText = "Config file")]
        public string gameConfigFile { get; set; }

        [Option('e', DefaultValue = "hammer.exe", Required = false, HelpText = "Hammer executable")]
        public string hammerExe { get; set; }

        [Option('d', DefaultValue = "", Required = false, HelpText = "Current Directory")]
        public string currentDirectory { get; set; }

        [Option('b', DefaultValue = "", Required = false, HelpText = "Binary Directory")]
        public string binDirectory { get; set; }


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
                if (File.Exists(this.binDirectory + "CmdSeq.wc")) File.Copy(this.binDirectory + "CmdSeq.wc", this.binDirectory + "CmdSeq_original.wc", true);
                if (File.Exists(this.currentDirectory + wcFile)) File.Copy(this.currentDirectory + this.wcFile, this.binDirectory + "CmdSeq.wc", true);

                if (File.Exists(this.binDirectory + "GameConfig.txt")) File.Copy(this.binDirectory + "GameConfig.txt", this.binDirectory + "GameConfig_original.txt", true);
                if (File.Exists(this.currentDirectory + gameConfigFile)) File.Copy(this.currentDirectory + this.gameConfigFile, this.binDirectory + "GameConfig.txt", true);

                //Start hammer
                var ccTagIntermediateCompilerProcess = new Process();

                Console.WriteLine(this.binDirectory + hammerExe);

                ccTagIntermediateCompilerProcess.StartInfo.FileName = this.binDirectory + hammerExe;
                ccTagIntermediateCompilerProcess.StartInfo.Arguments = "-nop4";
                ccTagIntermediateCompilerProcess.Start();
                ccTagIntermediateCompilerProcess.WaitForExit();

                // Restore original configurations

                if (File.Exists(this.binDirectory + "CmdSeq.wc")) File.Copy(this.binDirectory + "CmdSeq.wc", this.currentDirectory + wcFile, true);
                if (File.Exists(this.binDirectory + "CmdSeq_original.wc"))
                {
                    File.Copy(this.binDirectory + "CmdSeq_original.wc", this.binDirectory + "CmdSeq.wc", true);
                    File.Delete(this.binDirectory + "CmdSeq_original.wc");
                }

                if (File.Exists(this.binDirectory + "GameConfig.txt")) File.Copy(this.binDirectory + "GameConfig.txt", this.currentDirectory + this.gameConfigFile, true);
                if (File.Exists(this.binDirectory + "GameConfig_original.txt"))
                {
                    File.Copy(this.binDirectory + "GameConfig_original.txt", this.binDirectory + "GameConfig.txt", true);
                    File.Delete(this.binDirectory + "GameConfig_original.txt");
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
