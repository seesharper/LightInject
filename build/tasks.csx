#load "Write.csx"
#r "nuget:Microsoft.Extensions.CommandLineUtils, 1.1.1"


using Microsoft.Extensions.CommandLineUtils;

public static class Tasks
{        
    private static readonly Dictionary<string, Action> actions = new Dictionary<string, Action>();     
    private static readonly CommandLineApplication cli;

    static Tasks()
    {
        cli = new CommandLineApplication(true);        
    }

    private static void AddSingle(Expression<Action> action)
    {        
        var actionName = GetActionName(action);
        var compiledAction = action.Compile();
        var command = CreateCommand(actionName, compiledAction);     
        cli.Commands.Add(command);   
        actions.Add(GetActionName(action), compiledAction);        
    }
    public static void Add(params Expression<Action>[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            if (i >= 1)
            {
                AddWithBeforeAction(actions[i], actions[i-1]);
            }
            else            
            {
                AddSingle(actions[i]);
            }

        }
    }
    
    private static void AddWithBeforeAction(Expression<Action> action, Expression<Action> beforeAction)
    {        
        var compiledAction = action.Compile();
        Action compositeAction = () => 
        {                                    
            actions[GetActionName(beforeAction)]();                        
            compiledAction();
        };
        var command = CreateCommand(GetActionName(action), compositeAction);     
        cli.Commands.Add(command); 
        actions.Add(GetActionName(action), compositeAction);
    }

    public static void Execute(Expression<Action> action)
    {
       var actionName = GetActionName(action);
       Action compiledAction = null;
       if (!actions.TryGetValue(actionName, out compiledAction))
       {
            compiledAction = action.Compile();
       }

        Write.Start(actionName);    
        Stopwatch watch = new Stopwatch();
        watch.Start();
        compiledAction();
        watch.Stop();        
        Write.End("...Done! ({0} ms)", watch.ElapsedMilliseconds);                             
    }

    public static void Execute(params string[] args)
    {
        try
        {
            cli.Execute(args);
        }
        catch (CommandParsingException)
        {
            cli.ShowHelp();            
        }
                    
    }

    private static CommandLineApplication CreateCommand(string name, Action action)
    {
        var command = new CommandLineApplication();
        command.Name = name;        
        command.OnExecute(() => {
            action();
            return 0;
        });
        return command;
    }

    private static string GetActionName(Expression<Action> action)
    {
        MethodCallExpression methodCall = (MethodCallExpression)action.Body;
        return methodCall.Method.Name;
    }    
}

// Tasks.Add(() => Init());
// Tasks.Add(() => Build2(), () => Init());
// Tasks.Add(() => RunUnitTests(), () => Build2());

// Tasks.Execute(() => Build2());


// public void Init()
// {
//     WriteLine("Init");
// }

// public void Build2()
// {
//     WriteLine("Build");
// }

// public void RunUnitTests()
// {
//     WriteLine("RunUnitTests");
// }
