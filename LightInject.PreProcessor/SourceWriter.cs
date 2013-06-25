namespace LightInject.PreProcessor
{
    using System.IO;
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