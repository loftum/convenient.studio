using Avalonia.Media;

namespace Convenient.Studio.UI;

public static class CustomBrush
{
    public static readonly ISolidColorBrush SelectionColor = new SolidColorBrush(Color.Parse("#214283"), .6);
    public static readonly ISolidColorBrush StaticClassMagenta = new SolidColorBrush(Color.Parse("#b900b9"));
    public static readonly ISolidColorBrush ClassOrange = new SolidColorBrush(Color.Parse("#ff8040"));
    public static readonly ISolidColorBrush StructBrown = new SolidColorBrush(Color.Parse("#b05800"));
}