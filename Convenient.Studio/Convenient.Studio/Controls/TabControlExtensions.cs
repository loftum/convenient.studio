using Avalonia.Controls.Primitives;

namespace Convenient.Studio.Controls;

public static class TabControlExtensions
{
    public static void SelectNextTab(this SelectingItemsControl tab)
    {
        if (tab.SelectedIndex >= tab.ItemCount - 1)
        {
            tab.SelectedIndex = 0;
            return;
        }

        tab.SelectedIndex++;
    }

    public static void SelectPreviousTab(this SelectingItemsControl tab)
    {
        if (tab.SelectedIndex <= 0)
        {
            tab.SelectedIndex = tab.ItemCount - 1;
            return;
        }
        tab.SelectedIndex--;
    }
}