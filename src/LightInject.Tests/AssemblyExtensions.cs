using System.Reflection;

namespace LightInject.Tests
{
    public static class AssemblyExtensions
    {
        public static string GetFolder(this Assembly assembly)
        {
            return assembly.Location;
        }
    }
}