namespace LightInject.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;



    public static class AssemblyAssert
    {
        public static void IsValidAssembly(string filename)
        {
            var peverifyPath = GetPathToPEVerify();
            var startInformation = CreateProcessStartInfo(filename, peverifyPath);
            var process = CreateProcess(startInformation);
            var processOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                var exitInfo = string.Format("PEVerify Exit Code: {0}", process.ExitCode);
                throw new InvalidOperationException(string.Format("{0} {1}", exitInfo, processOutput));
            }
        }

        private static Process CreateProcess(ProcessStartInfo startInformation)
        {
            var process = new Process();
            process.StartInfo = startInformation;
            process.Start();
            return process;
        }

        private static ProcessStartInfo CreateProcessStartInfo(string filename, string peverifyPath)
        {
            var startInformation = new ProcessStartInfo(peverifyPath);
            startInformation.CreateNoWindow = true;
            startInformation.Arguments = "\"" + filename + "\" /MD /IL /UNIQUE /VERBOSE";
            startInformation.RedirectStandardOutput = true;
            startInformation.UseShellExecute = false;
            return startInformation;
        }

        private static string GetPathToPEVerify()
        {
            var peverifyPath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\x64\peverify.exe";

            //var peverifyPath = Path.Combine(
            //    ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.VersionLatest,VisualStudioVersion.VersionLatest), @"bin\NETFX 4.0 Tools\peverify.exe");
            return peverifyPath;
        }
    }
}