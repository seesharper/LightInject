namespace LightInject.PreProcessor
{
    using System.Collections.Generic;
    using System.IO;

    public class Publicizer
    {
        private static readonly List<string> Exceptions = new List<string>();
        
        static Publicizer()
        {
            Exceptions.Add("internal static class TypeHelper");
        }
        
        public static void Publicize(string inputFile, string outputFile)
         {
             using (var reader = new StreamReader(inputFile))
             {
                 using (var writer = new StreamWriter(outputFile))
                 {
                     Write(reader, writer);
                 }
             }
         }

        public static void Write(StreamReader reader, StreamWriter writer)
        {
             while (!reader.EndOfStream)
             {
                 var line = reader.ReadLine();
                 if (line.Contains("internal") && !Exceptions.Contains(line))
                 {
                     line = line.Replace("internal", "public");
                 }
                 writer.Write(line);
             }
        }
    }
}