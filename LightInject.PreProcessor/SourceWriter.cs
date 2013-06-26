namespace LightInject.PreProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class SourceWriter
    {
        private static DirectiveEvaluator directiveEvaluator = new DirectiveEvaluator();

        public static void Write(string directive, string inputFile, string outputFile)
        {
            using (var reader = new StreamReader(inputFile))
            {
                using (var writer = new StreamWriter(outputFile))
                {
                    Write(directive, reader, writer);                    
                }
            }
        }



        public static void Write(string directive, StreamReader reader, StreamWriter writer)
        {
            bool shouldWrite = true;                        
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("#if"))
                {
                    shouldWrite = directiveEvaluator.Execute(directive, line.Substring(4));
                    continue;
                }

                if (line.StartsWith("#endif"))
                {
                    shouldWrite = true;
                    continue;
                }


                if (shouldWrite)
                {
                    if (line.Contains("namespace"))
                    {
                        line = line.Replace(line.Substring(10), "$rootnamespace$");
                    }

                    if ((line.Contains("public class") || line.Contains("internal class")) && directive != "NETFX_CORE")
                    {
                        string tabString = string.Empty;
                        string[] tabs = line.Where(c => c == '\t').Select(c => c.ToString()).ToArray();
                        if (tabs.Length > 0)
                        {
                            tabString = tabs.Aggregate((current, next) => current + next);
                        }                             
                                
                        
                        writer.WriteLine("{0}{1}", tabString, "[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");                        
                    }

                    
                    if (!reader.EndOfStream)
                    {
                        writer.WriteLine(line);
                    }
                    else
                    {
                        writer.Write(line);
                    }

                }
            }            
        }
    }
}