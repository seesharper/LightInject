
using System.Text.RegularExpressions;

string path = null;

if (Env.ScriptArgs.Count() == 0)
{
    path = Directory.GetCurrentDirectory();    
}
else
{
    path = Env.ScriptArgs[0];
}

Console.WriteLine("Migrating MsTest files in {0}", path);

var files = GetFiles(path);

Console.WriteLine("Found {0} files", files.Length);

foreach (var file in files)
{
    Console.WriteLine("Migrating {0}", file);
    var source = ReadFile(file);
    var result = Migrate(source);
    WriteFile(file, result);
}

if (files.Length > 0)
{
    Console.WriteLine("Migrated {0}", files.Length);
}

private static string ReadFile(string pathToFile)
{
    using(var reader = new StreamReader(pathToFile))
    {
        return reader.ReadToEnd();				
    }
}

private static void WriteFile(string pathToFile, string content)
{
    using(var writer = new StreamWriter(pathToFile))
    {
        writer.Write(content);
    }
}

private static string[] GetFiles(string path)
{
    return Directory.GetFiles(path, "*.cs");
}

private static string Migrate(string source)
{
    source = source.Replace("[TestMethod]", "[Fact]")
    .Replace("[TestClass]", "")
    .Replace("Assert.AreEqual", "Assert.Equal")
    .Replace("Assert.AreNotEqual", "Assert.NotEqual")
    .Replace("Assert.IsTrue", "Assert.True")
    .Replace("Assert.IsFalse", "Assert.False")
    .Replace("Assert.IsNotNull", "Assert.NotNull")
    .Replace("Assert.IsNull", "Assert.Null")
    .Replace("Assert.AreNotSame", "Assert.NotSame")
    .Replace("Assert.AreSame", "Assert.Same")
    .Replace("using Microsoft.VisualStudio.TestTools.UnitTesting", "using Xunit");
    source = HandleIsInstanceOfType(source);
   
    return source;
}

private static string HandleIsInstanceOfType(string source)
{
    Regex regex = new Regex(@"(Assert.IsInstanceOfType)\((.*),\s(typeof.*)\)");
    return regex.Replace(source, "Assert.IsAssignableFrom($3, $2)");    
}



