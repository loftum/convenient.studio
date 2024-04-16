using Avalonia.Threading;

namespace Convenient.Studio.ConsoleRedirect
{
    public class Dispatchable<T>
    {
        public T Item { get; }
        public Dispatcher Dispatcher { get; }

        public Dispatchable(T item) : this(item, null)
        {
        }

        public Dispatchable(T item, Dispatcher dispatcher)
        {
            Item = item;
            Dispatcher = dispatcher;
        }

        public void Invoke(Action<T> action)
        {
            if (Dispatcher != null)
            {
                Dispatcher.Post(() => action(Item));
            }
            else
            {
                action(Item);
            }
        }
    }
}