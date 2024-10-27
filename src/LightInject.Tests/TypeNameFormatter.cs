using System;
using System.Text;

namespace LightInject;

/// <summary>
/// [assembly: DebuggerDisplay("{LightInject.TypeNameFormatter.GetHumanFriendlyTypeName(this)}", Target = typeof(Type))]
/// </summary>    
public static class TypeNameFormatter
{
    public static string GetHumanFriendlyTypeName(Type type)
    {
        StringBuilder humanFriendlyName = new StringBuilder();
        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {

            humanFriendlyName.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
            humanFriendlyName.Append('<');
            foreach (Type argument in type.GenericTypeArguments)
            {
                humanFriendlyName.Append(GetHumanFriendlyTypeName(argument));
                humanFriendlyName.Append(", ");
            }
            humanFriendlyName.Remove(humanFriendlyName.Length - 2, 2);
            humanFriendlyName.Append('>');
        }
        else if (type.IsGenericTypeDefinition)
        {
            humanFriendlyName.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
            humanFriendlyName.Append('<');
            foreach (Type parameter in type.GetGenericArguments())
            {
                humanFriendlyName.Append(parameter.Name);
                humanFriendlyName.Append(", ");
            }
            humanFriendlyName.Remove(humanFriendlyName.Length - 2, 2);
            humanFriendlyName.Append('>');
        }
        else
        {
            humanFriendlyName.Append(type.Name);
        }
        return humanFriendlyName.ToString();
    }
}
