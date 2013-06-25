namespace LightInject.PreProcessor
{
    using System;
    using System.Linq;

    public class DirectiveEvaluator
    {
        public bool Execute(string directive, string expression)
        {
            string[] subExpressions =
                expression.Split(new[] { "||" }, StringSplitOptions.None).Select(e => e.Trim()).ToArray();

            return subExpressions.Any(subExpression => directive == subExpression);
        }
    }
}