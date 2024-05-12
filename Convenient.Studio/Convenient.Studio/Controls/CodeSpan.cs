namespace Convenient.Studio.Controls;

public readonly struct CodeSpan
{
    /// <summary>
    /// Start index in editor
    /// </summary>
    public int Start { get; private init; }
    
    /// <summary>
    /// End index in editor
    /// </summary>
    public int End { get; private init; }
    
    /// <summary>
    /// "Global" offset (cursor or mouse pointer) in text editor
    /// </summary>
    public int Offset { get; private init; }
    
    /// <summary>
    /// "Local" offset (cursor or mouse pointer) for this span of code
    /// </summary>
    public int LocalOffset => Offset - Start;
    
    /// <summary>
    /// Length of code
    /// </summary>
    public int Length => End - Start;

    public int OffsetLength => Offset - Start;
    
    
    public bool IsEmpty => Length == 0;

    public static CodeSpan FromBounds(int start, int end, int offset)
    {
        return new CodeSpan
        {
            Start = start,
            End = end,
            Offset = offset
        };
    }
}