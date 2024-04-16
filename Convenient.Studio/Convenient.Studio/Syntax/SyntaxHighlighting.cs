using System.Xml;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;

namespace Convenient.Studio.Syntax;

public static class SyntaxHighlighting
{
    public static IHighlightingDefinition CSharp { get; }
    public static IHighlightingDefinition CSharpDark { get; }
    public static IHighlightingDefinition Json { get; }

    static SyntaxHighlighting()
    {
        var manager = HighlightingManager.Instance;
        CSharp = DoLoad("CSharp.xshd");
        CSharpDark = DoLoad("CSharp-Dark.xshd");
            
        manager.RegisterHighlighting("C#-2", new[] { ".cs" }, CSharp);
        manager.RegisterHighlighting("C#-Dark", new[] { ".cs" }, CSharpDark);
        //manager.RegisterHighlighting("Json-2", new[]{".json"}, Json);
    }

    public static void Load()
    {
    }

    private static IHighlightingDefinition DoLoad(string name)
    {
        var type = typeof(SyntaxHighlighting);
        var resource = $"{type.Namespace}.{name}";
        using (var s = type.Assembly.GetManifestResourceStream(resource))
        {
            if (s == null)
            {
                throw new IOException(resource);
            }
            using (XmlReader reader = new XmlTextReader(s))
            {
                var highlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                return highlighting;
                    
            }
        }
    }
}