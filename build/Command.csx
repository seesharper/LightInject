using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

public static class Command
{
    private static StringBuilder lastProcessOutput = new StringBuilder();
    
    private static StringBuilder lastStandardErrorOutput = new StringBuilder();    
          
    public static string Execute(string commandPath, string arguments, string capture = null)
    {
        lastProcessOutput.Clear();
        lastStandardErrorOutput.Clear();
        var startInformation = CreateProcessStartInfo(commandPath, arguments);
        var process = CreateProcess(startInformation);
        SetVerbosityLevel(process, capture);        
        process.Start();        
        RunAndWait(process);                
               
        if (process.ExitCode != 0 && commandPath != "robocopy")
        {                      
            WriteLine(lastStandardErrorOutput.ToString());
            throw new InvalidOperationException("Command failed");
        }   
        
        return lastProcessOutput.ToString();
    }

    private static ProcessStartInfo CreateProcessStartInfo(string commandPath, string arguments)
    {
        var startInformation = new ProcessStartInfo($"\"{commandPath}\"");
        startInformation.CreateNoWindow = true;
        startInformation.Arguments =  arguments;
        startInformation.RedirectStandardOutput = true;
        startInformation.RedirectStandardError = true;
        startInformation.UseShellExecute = false;
        
        return startInformation;
    }

    private static void RunAndWait(Process process)
    {        
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();         
        process.WaitForExit();                
    }

    private static void SetVerbosityLevel(Process process, string capture = null)
    {
        if(capture != null)
        {
            process.OutputDataReceived += (s, e) => 
            {
                if (e.Data == null)
                {
                    return;
                }
                              
                if (Regex.Matches(e.Data, capture,RegexOptions.Multiline).Count > 0)
                {
                    lastProcessOutput.AppendLine(e.Data);
                    WriteLine(e.Data);        
                }                                             
            };                        
        }
        process.ErrorDataReceived += (s, e) => 
        {
            lastStandardErrorOutput.AppendLine();
            lastStandardErrorOutput.AppendLine(e.Data);                        
        };
    }

    private static Process CreateProcess(ProcessStartInfo startInformation)
    {
        var process = new Process();
        process.StartInfo = startInformation;               
        return process;
    }
}