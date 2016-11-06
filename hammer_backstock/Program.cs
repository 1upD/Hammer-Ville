using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System;

namespace hammer_backstock
{
    class Program
    {
        static void Main(string[] args)
        {
            string myName = "Custom";
            string wcFile = "CmdSeq_custom.wc";
            string gameConfigFile = "GameConfig_custom.txt";
            string hammerExe = "hammer.exe";


            try
            {
                // Ensure I'm the only hammer
                if (Process.GetProcessesByName("hammer").Count() > 0)
                {
                    MessageBox.Show(myName + " Hammer can only support one instance at a time");
                    return;
                }

                // Move configurations into place
                if (File.Exists("CmdSeq.wc")) File.Copy("CmdSeq.wc", "CmdSeq_original.wc", true);
                if (File.Exists("CmdSeq_backstock.wc")) File.Copy(wcFile, "CmdSeq.wc", true);

                if (File.Exists("GameConfig.txt")) File.Copy("GameConfig.txt", "GameConfig_original.txt", true);
                if (File.Exists(gameConfigFile)) File.Copy(gameConfigFile, "GameConfig.txt", true);

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

                if (File.Exists("GameConfig.txt")) File.Copy("GameConfig.txt", gameConfigFile, true);
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
