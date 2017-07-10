public static class Write
{
    private static int depth = 0;

    private static string lastWriteOperation;
    
    public static void Start(string message, params object[] arguments)
    {
        StringBuilder sb = new StringBuilder();    
        sb.Append("\n");
        sb.Append(' ', depth);
        sb.Append(string.Format(message, arguments));            
        Console.Out.Flush();
        Console.Write(sb.ToString());
        Console.Out.Flush();
        lastWriteOperation = "WriteStart";
        depth++;
    }

    public static void Info(string message,  params object[] arguments)
    {
        if (message == null)
        {
            return;
        }
        StringBuilder sb = new StringBuilder();    
        if (lastWriteOperation == "WriteStart")
        {
            sb.Append("\n");
        }
        sb.Append(' ', depth);
        sb.Append(string.Format(message, arguments));            
        Console.WriteLine(sb.ToString());
        lastWriteOperation = "WriteLine";
    }

    public static void End(string message, params object[] arguments)
    {
        depth--;
        if (lastWriteOperation == "WriteStart")
        {
            Console.WriteLine(string.Format(message,arguments));
        }
        else
        {
            StringBuilder sb = new StringBuilder();    
            sb.Append(' ', depth);
            sb.Append(string.Format(message, arguments));            
            Console.WriteLine(sb.ToString());
            lastWriteOperation = "WriteLine";
        }
        
    }
}