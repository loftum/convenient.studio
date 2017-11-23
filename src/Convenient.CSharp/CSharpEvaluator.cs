using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Convenient.CSharp.Completion;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Convenient.CSharp
{
    public class CSharpEvaluator
    {
        private readonly object _context;
        private ScriptState _scriptState;
        private readonly ScriptOptions _options;

        public CSharpEvaluator(ScriptOptions options) : this(options, null)
        {
        }

        public CSharpEvaluator(ScriptOptions options, object context)
        {
            _options = options;
            _context = context;
            Reset().Wait();
        }

        public async Task Reset()
        {
            _scriptState = await CSharpScript.RunAsync("", _options, _context, _context?.GetType());
        }

        public async Task<object> EvaluateAsync(string statement, CancellationToken cancellationToken)
        {
            try
            {
                _scriptState = await _scriptState.ContinueWithAsync(statement, cancellationToken: cancellationToken).ConfigureAwait(false);
                var result = _scriptState.ReturnValue ?? _scriptState.Exception;
                if (result is Task t)
                {
                    await t.ConfigureAwait(false);
                    var taskType = t.GetType();
                    if (taskType.IsGenericType)
                    {
                        return taskType.GetProperty("Result").GetValue(t);
                    }
                }
                return result;
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

        public Task<IEnumerable<CompletionThingy>> GetCompletions(string code, int location)
        {
            var script = _scriptState.Script.ContinueWith(code);
            script.Compile();
            var compilation = script.GetCompilation();
            var tree = compilation.SyntaxTrees.Single();
            var completer = new CodeCompleter(tree, compilation, location);
            var completions = completer.GetCompletions();
            return Task.FromResult(completions);
        }
    }
}