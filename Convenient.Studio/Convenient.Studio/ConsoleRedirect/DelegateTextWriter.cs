using System.Globalization;
using System.Text;
using Avalonia.Threading;

namespace Convenient.Studio.ConsoleRedirect;

public class DelegateTextWriter : TextWriter
{
    public override Encoding Encoding => Encoding.Default;

    public IList<Dispatchable<TextWriter>> Dispatchables { get; } = new List<Dispatchable<TextWriter>>();

    public DelegateTextWriter() : base(CultureInfo.InvariantCulture)
    {
    }

    public void Add(TextWriter writer)
    {
        Dispatchables.Add(new Dispatchable<TextWriter>(writer));
    }

    public void Add(TextWriter writer, Dispatcher dispatcher)
    {
        Dispatchables.Add(new Dispatchable<TextWriter>(writer, dispatcher));
    }

    public void Remove(TextWriter writer)
    {
        var dispatchables = Dispatchables.Where(w => w.Item == writer).ToList();
        foreach (var dispatchable in dispatchables)
        {
            Dispatchables.Remove(dispatchable);
        }
    }

    public override void Write(char[] buffer, int index, int count)
    {
        ForEach(w => w.Write(buffer, index, count));
    }

    public override void Write(string value)
    {
        ForEach(w => w.Write(value));
    }

    public override void WriteLine()
    {
        ForEach(w => w.WriteLine());
    }

    public override void WriteLine(string value)
    {
        ForEach(w => w.WriteLine(value));
    }

    public override void WriteLine(object value)
    {
        ForEach(w => w.WriteLine(value));
    }

    private void ForEach(Action<TextWriter> action)
    {
        foreach (var writer in Dispatchables)
        {
            writer.Invoke(action);
        }
    }
}