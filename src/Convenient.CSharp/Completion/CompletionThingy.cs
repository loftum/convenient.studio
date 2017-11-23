namespace Convenient.CSharp.Completion
{
    public class CompletionThingy
    {
        public string Prefix { get; }
        public string Completion { get; }

        public string Text => $"{Prefix}{Completion}";
        public object Content { get; }
        public object Description { get; }

        public CompletionThingy(string prefix, string completion, string content, string description)
        {
            Prefix = prefix;
            Completion = completion;
            Content = content;
            Description = description;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}