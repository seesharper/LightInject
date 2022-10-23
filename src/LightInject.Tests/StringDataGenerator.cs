using System.Collections.Generic;

namespace LightInject.Tests;

public static class StringDataGenerator
{
    public static IEnumerable<object[]> NullOrWhiteSpaceData()
    {
        yield return new object[] { null };
        yield return new object[] { string.Empty };
        yield return new object[] { " " };
        yield return new object[] { "\t" };
        yield return new object[] { "\n" };
        yield return new object[] { "\r" };
    }
}