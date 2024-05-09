using System.Text;
using AvaloniaEdit.Document;

namespace Convenient.Studio.ConsoleRedirect;

public class DocumentTextWriter : TextWriter
{
    private readonly TextDocument _document;
        
    public override Encoding Encoding => Encoding.UTF8;
        
    public DocumentTextWriter(TextDocument document)
    {
        _document = document;
    }

    public override void Write(bool value) => _document.Append(value.ToString());
    public override void Write(char value) => _document.Append(value.ToString());
    public override void Write(char[] buffer) => _document.Append(new string(buffer));
    public override void Write(char[] buffer, int index, int count) => _document.Append(new string(buffer, index, count));
    public override void Write(decimal value) => _document.Append(value.ToString());
    public override void Write(double value) => _document.Append(value.ToString());
    public override void Write(int value) => _document.Append(value.ToString());
    public override void Write(long value) => _document.Append(value.ToString());
    public override void Write(object value) => _document.Append(value.ToString());
    public override void Write(ReadOnlySpan<char> buffer)  => _document.Append(new string(buffer));
    public override void Write(float value)  => _document.Append(value.ToString());
    public override void Write(string value) => _document.Append(value);
    public override void Write(string format, object arg0) => _document.Append(string.Format(format, arg0));
    public override void Write(string format, object arg0, object arg1) => _document.Append(string.Format(format, arg0, arg1));
    public override void Write(string format, object arg0, object arg1, object arg2) => _document.Append(string.Format(format, arg0, arg1, arg2));
    public override void Write(string format, params object[] arg) => _document.Append(string.Format(format, arg));
    public override void Write(StringBuilder value) => _document.Append(value.ToString());
    public override void Write(uint value) => _document.Append(value.ToString());
    public override void Write(ulong value) => _document.Append(value.ToString());
    public override Task WriteAsync(char value) => _document.AppendAsync(value.ToString());
    public override Task WriteAsync(char[] buffer, int index, int count) => _document.AppendAsync(new string(buffer, index, count));
    public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new CancellationToken()) => _document.AppendAsync(new string(buffer.ToArray()));
    public override Task WriteAsync(string value) => _document.AppendAsync(value);
    public override Task WriteAsync(StringBuilder value, CancellationToken cancellationToken = default) => _document.AppendAsync(value.ToString());
    public override void WriteLine() => _document.AppendLine();
    public override void WriteLine(bool value) => _document.AppendLine(value.ToString());
    public override void WriteLine(char value) => _document.AppendLine(value.ToString());
    public override void WriteLine(char[] buffer) => _document.AppendLine(new string(buffer));
    public override void WriteLine(char[] buffer, int index, int count) => _document.AppendLine(new string(buffer, index, count));
    public override void WriteLine(decimal value) => _document.AppendLine(value.ToString());
    public override void WriteLine(double value) => _document.AppendLine(value.ToString());
    public override void WriteLine(int value) => _document.AppendLine(value.ToString());
    public override void WriteLine(long value) => _document.AppendLine(value.ToString());
    public override void WriteLine(object value) => _document.AppendLine(value.ToString());
    public override void WriteLine(ReadOnlySpan<char> buffer) => _document.AppendLine(new string(buffer));
    public override void WriteLine(float value) => _document.AppendLine(value.ToString());
    public override void WriteLine(string value) => _document.AppendLine(value);
    public override void WriteLine(string format, object arg0) => _document.AppendLine(string.Format(format, arg0));
    public override void WriteLine(string format, object arg0, object arg1) => _document.AppendLine(string.Format(format, arg0, arg1));
    public override void WriteLine(string format, object arg0, object arg1, object arg2) => _document.AppendLine(string.Format(format, arg0, arg1, arg2));
    public override void WriteLine(string format, params object[] arg) => _document.AppendLine(string.Format(format, arg));
    public override void WriteLine(StringBuilder value) => _document.AppendLine(value.ToString());
    public override void WriteLine(uint value) => _document.AppendLine(value.ToString());
    public override void WriteLine(ulong value) => _document.AppendLine(value.ToString());
    public override Task WriteLineAsync() => _document.AppendLineAsync();
    public override Task WriteLineAsync(char value) => _document.AppendLineAsync(value.ToString());
    public override Task WriteLineAsync(char[] buffer, int index, int count) => _document.AppendLineAsync(new string(buffer, index, count));
    public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new CancellationToken()) => _document.AppendLineAsync(new string(buffer.ToArray()));
    public override Task WriteLineAsync(string value) => _document.AppendLineAsync(value);
    public override Task WriteLineAsync(StringBuilder value, CancellationToken cancellationToken = new CancellationToken()) => _document.AppendLineAsync(value.ToString());
}