using System.Reflection;
using Convenient.Studio.Scripting;
using Convenient.Studio.Scripting.Cancellation;
using Microsoft.CodeAnalysis.Scripting;

namespace Convenient.Studio.ViewModels;

public class Interactive
{
    private static readonly Assembly[] Assemblies =
        Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(Assembly.LoadFile)
            .ToArray();

    private static readonly ScriptOptions Options;
        
    static Interactive()
    {
        Options = ScriptOptions.Default
            .WithReferences(Assemblies)
            .WithImports("System",
                "System.Reflection",
                "System.Linq",
                "System.Collections.Generic",
                "System.Security.Cryptography",
                "System.Text.RegularExpressions",
                "System.Threading.Tasks");
    }

    public CancellationTokenSource CancellationTokenSource => _context.CancellationTokenSource;
    public CSharpEvaluator Evaluator { get; }
    private readonly StudioContext _context;

    public Interactive(StudioContext context)
    {
        _context = context;
        Evaluator = new CSharpEvaluator(_context, Options);
        Evaluator.Reset().Wait();
    }

    public Task<object> EvaluateAsync(string code, CancellationInjection cancellationInjection)
    {
        _context.CancellationTokenSource = new CancellationTokenSource();
        return Evaluator.EvaluateAsync(code, _context.CancellationToken, cancellationInjection);
    }
}