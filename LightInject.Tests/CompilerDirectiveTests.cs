namespace LightInject.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using LightInject.PreProcessor;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CompilerDirectiveTests
    {
        [TestMethod]
        public void Evaluate_MatchingDirective_ReturnsTrue()
        {
            var evaluator = new DirectiveEvaluator();
            bool result = evaluator.Execute("SomeDirective", "SomeDirective");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Evaluate_NonMatchingDirective_ReturnsFalse()
        {
            var evaluator = new DirectiveEvaluator();
            bool result = evaluator.Execute("SomeDirective", "AnotherDirective");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Evaluate_MatchingDirectiveInOrExpression_ReturnTrue()
        {
            var evaluator = new DirectiveEvaluator();
            bool result = evaluator.Execute("SomeDirective", "SomeDirective || AnotherDirective");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Evaluate_NonMatchingDirectiveInOrExpression_ReturnFalse()
        {
            var evaluator = new DirectiveEvaluator();
            bool result = evaluator.Execute("SomeDirective", "AnotherDirective || YetAnotherDirective");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Write_NoDirective_WritesAllLines()
        {
            string input = "Line1\r\nLine2";
            var inputStream = CreateInputStream(input);
            
            var output = Process("SOMEDIRECTIVE", inputStream);

            Assert.AreEqual(input, output);            
        }

        [TestMethod]
        public void Write_MatchingDirective_WritesLinesInsideDirective()
        {            
            string input = "#if SOMEDIRECTIVE\r\nLine2\r\n#endif";
            var inputStream = CreateInputStream(input);

            var output = Process("SOMEDIRECTIVE", inputStream);

            Assert.AreEqual("Line2\r\n", output);
        }

        [TestMethod]
        public void Write_NonMatchingDirective_DoesWritesLinesInsideDirective()
        {
            string input = "Line1\r\n#if SOMEDIRECTIVE\r\nLine1\r\n#endif";

            var inputStream = CreateInputStream(input);

            var output = Process("ANOTHERDIRECTIVE", inputStream);

            Assert.AreEqual("Line1\r\n", output);
        }

        [TestMethod]
        public void Write_NonMatchingDirective_WritesLinesAfterDirective()
        {
            string input = "#if SOMEDIRECTIVE\r\nLine1\r\n#endif\r\nLine2";

            var inputStream = CreateInputStream(input);

            var output = Process("ANOTHERDIRECTIVE", inputStream);

            Assert.AreEqual("Line2", output);
        }

        [TestMethod]
        public void Write_NameSpace_WriteNamespaceMacro()
        {
            string input = "namespace SomeNamespace";

            var inputStream = CreateInputStream(input);

            var output = Process("SOMEDIRECTIVE", inputStream);

            Assert.AreEqual("namespace $rootnamespace$", output);
        }

        [TestMethod]
        public void Write_PublicClasses_AddsExcludeFromCodeCoverageAttribute()
        {
            string input = "public class SomeClass";

            var inputStream = CreateInputStream(input);

            var output = Process("SOMEDIRECTIVE", inputStream);

            Assert.AreEqual("[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]\r\npublic class SomeClass", output);
        }

        [TestMethod]
        public void Write_InternalClasses_AddsExcludeFromCodeCoverageAttribute()
        {
            string input = "internal class SomeClass";

            var inputStream = CreateInputStream(input);

            var output = Process("SOMEDIRECTIVE", inputStream);

            Assert.AreEqual("[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]\r\ninternal class SomeClass", output);
        }

        [TestMethod]
        public void Write_InternalStaticClasses_AddsExcludeFromCodeCoverageAttribute()
        {
            string input = "internal static class SomeClass";

            var inputStream = CreateInputStream(input);

            var output = Process("SOMEDIRECTIVE", inputStream);

            Assert.AreEqual("[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]\r\ninternal static class SomeClass", output);
        }


        [TestMethod]
        public void Write_InternalClasses_AddsExcludeFromCodeCoverageAttributeUsingSameIndentation()
        {
            string input = "    internal class SomeClass";

            var inputStream = CreateInputStream(input);

            var output = Process("SOMEDIRECTIVE", inputStream);

            Assert.AreEqual("    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]\r\n    internal class SomeClass", output);
        }


        [TestMethod]
        public void Write_InternalClasses_DoesNotAddExcludeFromCodeCoverageForWinRT()
        {
            string input = "\tinternal class SomeClass";

            var inputStream = CreateInputStream(input);

            var output = Process("NETFX_CORE", inputStream);

            Assert.AreEqual(input, output);
        }

        private static string Process(string directive, MemoryStream inputStream)
        {
            var outputStream = new MemoryStream();
            
            using (var reader = new StreamReader(inputStream))
            {
                using (var writer = new StreamWriter(outputStream))
                {
                    SourceWriter.Write(directive, reader, writer);                    
                    writer.Flush();
                    string output;
                    outputStream.Position = 0;
                    var resultReader = new StreamReader(outputStream);
                    
                        output = resultReader.ReadToEnd();
                        return output;
                    
                    
                }
            }                      
        }

        private static MemoryStream CreateInputStream(string input)
        {
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            return inputStream;
        }
    }

    


}