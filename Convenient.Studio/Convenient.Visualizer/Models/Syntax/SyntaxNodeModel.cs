namespace Convenient.Visualizer.Models.Syntax
{
    public class SyntaxNodeModel
    {
        public string Type { get; set; }
        public string Kind { get; set; }
        public string Text { get; set; }
        public bool IsMissing { get; set; }
        public List<SyntaxTriviaModel> LeadingTrivia { get; set; }
        public List<SyntaxTriviaModel> TrailingTrivia { get; set; }
        public List<SyntaxNodeModel> ChildNodes { get; set; }
        public List<SyntaxTokenModel> ChildTokens { get; set; }

        public SyntaxNodeModel()
        {
            LeadingTrivia = new List<SyntaxTriviaModel>();
            TrailingTrivia = new List<SyntaxTriviaModel>();
            ChildNodes = new List<SyntaxNodeModel>();
            ChildTokens = new List<SyntaxTokenModel>();
        }

        public override string ToString()
        {
            return $"{Kind} {Text}";
        }
    }
}