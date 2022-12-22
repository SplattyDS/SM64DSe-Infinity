/* 
 * Adopted from NSMBe's patch maker
 */ 

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace SM64DSe.Patcher
{
    class PatchCompiler
    {
        public static bool HideConsoleWindow;

        public static int compilePatch(uint destAddr, DirectoryInfo romDir)
        {
            return runProcess("make CODEADDR=0x" + destAddr.ToString("X8"), romDir.FullName);
        }

        /*public static int compilePatch(uint destAddr, DirectoryInfo romDir, string sources)
        {
            return runProcess("make CODEADDR=0x" + destAddr.ToString("X8") + " SOURCES=" + sources + " libfat_source", romDir.FullName);
        }*/

        public static int cleanPatch(DirectoryInfo romDir)
        {
            int exitCode = runProcess("make clean", romDir.FullName);

            if (exitCode == 0)
                exitCode = runProcess("make clean TARGET=newcode1", romDir.FullName);

            return exitCode;
        }

        public static int runProcess(string proc, string cwd)
        {
            Process p = new Process();

            p.StartInfo.FileName = "cmd";
            p.StartInfo.CreateNoWindow = HideConsoleWindow;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = cwd;
            p.StartInfo.Arguments = "/C " + proc + " || pause";
            p.StartInfo.RedirectStandardInput = HideConsoleWindow;
            p.StartInfo.RedirectStandardOutput = HideConsoleWindow;
            p.Start();

            if (HideConsoleWindow)
            {
                p.StandardInput.WriteLine();
                p.StandardInput.Close();
                p.StandardOutput.ReadToEnd();
            }

            p.WaitForExit();

            return p.ExitCode;
        }
    }
}
