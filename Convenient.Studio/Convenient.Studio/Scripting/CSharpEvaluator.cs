using System.Reflection;
using Convenient.Studio.Scripting.Cancellation;
using Convenient.Studio.Scripting.Completion;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Convenient.Studio.Scripting;

public interface ICompletionProvider
{
    ScriptState ScriptState { get; }
    Task<List<CompletionThing>> GetCompletions(string code, int location);
    CodeCompleter GetCodeCompleter(string code, int location);
}
    
public class CSharpEvaluator : ICompletionProvider
{
    private static readonly Assembly[] Assemblies =
        Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(Assembly.LoadFile)
            .ToArray();

    public static ScriptOptions DefaultOptions => ScriptOptions.Default
        .WithReferences(Assemblies)
            
        .WithImports("System",
            "System.Threading.Tasks",
            "System.Reflection",
            "System.Linq",
            "System.Collections.Generic",
            "System.Security.Cryptography",
            "System.Text.RegularExpressions",
            "System.Text.Json");

    private readonly object _context;
    public ScriptState ScriptState { get; private set; }
    private readonly ScriptOptions _options;

    public CSharpEvaluator(object context) : this(context, DefaultOptions)
    {
    }

    public CSharpEvaluator(object context, ScriptOptions options)
    {
        _options = options;
        _context = context;
        Reset().Wait();
    }

    public async Task Reset()
    {
        ScriptState = await CSharpScript.RunAsync("", _options, _context, _context.GetType());
    }

    public async Task<object> EvaluateAsync(string code, CancellationToken cancellationToken, CancellationInjection cancellationInjection)
    {
        try
        {
            code = InjectCancellationDetectionCode(code, cancellationInjection);
            ScriptState = await ScriptState.ContinueWithAsync(code, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = ScriptState.GetResult();
            while (result is Task t)
            {
                await t.ConfigureAwait(false);
                var taskType = t.GetType();
                if (taskType.IsGenericType)
                {
                    result = taskType.GetProperty("Result")!.GetValue(t);
                }
            }

            return result;
        }
        catch (CancelExecutionException ceex)
        {
            return ceex;
        }
        catch (CompilationErrorException ex)
        {
            return ex.Message;
        }
        catch (Exception ex)
        {
            return ex.GetBaseException();
        }
    }

    private string InjectCancellationDetectionCode(string code, CancellationInjection? cancellationInjection)
    {
        switch (cancellationInjection)
        {
            case CancellationInjection.EveryStatement:
            {
                var lines = code.Split(Environment.NewLine).Select(x => x.TrimEnd());
                var ret = new List<string>();
                foreach (var line in lines)
                {
                    ret.Add(line);
                    if (line.EndsWith(";"))
                    {
                        ret.Add(CancellationDetectionSnippet);
                    }
                }

                return string.Join(Environment.NewLine, ret);
            }
            case CancellationInjection.ByComment:
            {
                var lines = code.Split(Environment.NewLine).Select(x => x.TrimEnd());
                var ret = new List<string>();
                foreach (var line in lines)
                {
                    ret.Add(line);
                    if (line.EndsWith("////"))
                    {
                        ret.Add(CancellationDetectionSnippet);
                    }
                }
                return string.Join(Environment.NewLine, ret);
            }
            default:
                return code;
        }
    }

    private const string CancellationDetectionSnippet = "if(CancellationToken.IsCancellationRequested){throw new Studio.Scripting.Cancellation.CancelExecutionException();}";

    public Task<List<CompletionThing>> GetCompletions(string code, int location)
    {
        var completer = GetCodeCompleter(code, location);
        var completions = completer.GetCompletions();
        return Task.FromResult(completions.ToList());
    }

    public CodeCompleter GetCodeCompleter(string code, int location)
    {
        var script = ScriptState.Script.ContinueWith(code);
        script.Compile();
        var compilation = script.GetCompilation();
        var tree = compilation.SyntaxTrees.Single();
        var completer = new CodeCompleter(tree, compilation, location);
        return completer;
    }
}