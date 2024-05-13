namespace Convenient.Visualizer.Models.Syntax
{
    public struct SyntaxTokenModel
    {
        public string Kind { get; set; }
        public string Text { get; set; }
        public bool IsMissing { get; set; }
    }
}