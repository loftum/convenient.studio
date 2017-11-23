using System;
using System.Threading;

namespace Convenient.Studio.ViewModels
{
    public class InteractiveContext
    {
        public CancellationToken CancellationToken { get; set; }

        public void SayHello()
        {
            Console.WriteLine("Hello");
        }
    }
}