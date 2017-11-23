using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Convenient.CSharp;
using Convenient.CSharp.Completion;
using Convenient.Studio.Controls;
using Microsoft.CodeAnalysis.Scripting;

namespace Convenient.Studio.ViewModels
{
    public class Interactive : ViewModelBase, IInteractiveVm
    {
        private static readonly ScriptOptions Options;

        static Interactive()
        {
            Options = ScriptOptions.Default
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
                .WithImports("System",
                    "System.Linq",
                    "System.Collections.Generic",
                    "Newtonsoft.Json");
        }

        private CancellationTokenSource _cancellationTokenSource;
        private CSharpEvaluator _evaluator;
        private InteractiveContext _context;
        private string _output;
        private bool _isExecuting;
        private bool _inputEnabled;

        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                InputEnabled = !value;
                OnPropertyChanged();
            }
        }

        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                _inputEnabled = value; 
                OnPropertyChanged();
            }
        }

        public ICommand Cancel { get; }


        public Interactive()
        {
            Reset();
            Cancel = new DelegateCommand(DoCancel);
            InputEnabled = true;
        }

        private void DoCancel()
        {
            if (IsExecuting)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public void Reset()
        {
            var context = new InteractiveContext();
            _evaluator = new CSharpEvaluator(Options, context);
            _context = context;
        }

        public Task<IEnumerable<CompletionThingy>> GetCompletions(string statement)
        {
            return _evaluator.GetCompletions(statement, statement.Length);
        }

        public async Task Execute(string statement)
        {
            IsExecuting = true;
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                
                _context.CancellationToken = _cancellationTokenSource.Token;
                var result = await Task.Run(() => _evaluator.EvaluateAsync(statement, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
                Output = result.ToResultString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                IsExecuting = false;
                _cancellationTokenSource.Dispose();
            }
        }
    }
}