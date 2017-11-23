using System;
using System.Windows.Input;
using Convenient.Studio.ConsoleRedirect;
using Convenient.Studio.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Convenient.Studio.Views
{
    public partial class InteractiveView
    {
        private readonly DocumentTextWriter _consoleWriter;

        protected IInteractiveVm Vm => (IInteractiveVm) DataContext;

        public InteractiveView()
        {
            InitializeComponent();
            Input.Vm = Vm;
            _consoleWriter = new DocumentTextWriter(Console.Document, 0);
            ConsoleOut.Writer.Add(_consoleWriter, Dispatcher);
        }

        ~InteractiveView()
        {
            ConsoleOut.Writer.Remove(_consoleWriter);
        }

        private async void Input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                var statement = Input.GetSelectedOrAllText()?.Trim();
                await Vm.Execute(statement);
                Focus();
            }
        }

        private void Console_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextEditor;
            textBox?.ScrollToEnd();
        }
    }
}
