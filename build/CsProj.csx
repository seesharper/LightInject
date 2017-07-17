using System.Xml.Linq;

public static class CsProj
{
    public static string ReadVersion(string pathToProjectFile)
    {
        var document = ReadDocument(pathToProjectFile);
        var versonElement = document.Descendants("Version").Single();
        return versonElement.Value;
    }

    private static XDocument ReadDocument(string pathToProjectFile)
    {
        using (var fileStream = new FileStream(pathToProjectFile,FileMode.Open, FileAccess.Read))
        {
            return XDocument.Load(fileStream);
        }
    }
}