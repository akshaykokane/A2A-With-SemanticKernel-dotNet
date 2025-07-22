using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace A2ADemo
{
    public class ConsoleOutputFunctionInvocationFilter : IFunctionInvocationFilter
    {
        private static string IndentMultilineString(string multilineText, int indentLevel = 1, int spacesPerIndent = 4)
        {
            var indentation = new string(' ', indentLevel * spacesPerIndent);
            char[] NewLineChars = { '\r', '\n' };
            string[] lines = multilineText.Split(NewLineChars, StringSplitOptions.None);
            return string.Join(Environment.NewLine, lines.Select(line => indentation + line));
        }

        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\nCalling Agent {context.Function.Name} with arguments:");
            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (var kvp in context.Arguments)
            {
                Console.WriteLine(IndentMultilineString($"  {kvp.Key}: {kvp.Value}"));
            }

            await next(context);

            if (context.Result.GetValue<object>() is ChatMessageContent[] chatMessages)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Response from Agent {context.Function.Name}:");
                foreach (var message in chatMessages)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(IndentMultilineString($"{message}"));
                }
            }
            Console.ResetColor();
        }
    }
}