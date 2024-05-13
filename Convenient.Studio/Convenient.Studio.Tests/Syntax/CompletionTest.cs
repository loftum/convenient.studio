using Convenient.Studio.Scripting;
using Convenient.Studio.Tests.Assertions;
using Xunit;
using Xunit.Abstractions;

namespace Convenient.Studio.Tests.Syntax;

public class CompletionTest
{
    protected readonly ITestOutputHelper Output;

    private readonly CSharpEvaluator _evaluator;

    public CompletionTest(ITestOutputHelper output)
    {
        Output = output;
        _evaluator = new CSharpEvaluator(new Car());
    }

    [Fact]
    public async Task Completion()
    {
        var (code, caret) = GetCodeAndCaretPosition("new Car().|");
        var completions = await _evaluator.GetCompletionsAsync(code, caret);
        
        Verify.That(completions.First().Text.IsStringEqualTo("SetSpeed(Int32 speed)"));
    }

    [Fact]
    public async Task METHOD()
    {
        var (code, caret) = GetCodeAndCaretPosition("new Car().SetSpeed(|)");

        var completer = _evaluator.GetCodeCompleter(code);
        var method = completer.GetSymbolAt(caret);

        var name = method.Name;
    }

    private static (string, int) GetCodeAndCaretPosition(string code)
    {
        var index = code.LastIndexOf('|');
        if (index < 0)
        {
            return (code, code.Length);
        }
        return (code.Remove(index, 1), index);
    }
}

public class Car
{
    public string Brand { get; set; }
    public void SetSpeed(int speed)
    {
    
    }
}