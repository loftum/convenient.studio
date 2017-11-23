using System.Collections.Generic;
using System.Threading.Tasks;
using Convenient.CSharp.Completion;

namespace Convenient.Studio.Controls
{
    public interface IInteractiveVm
    {
        Task<IEnumerable<CompletionThingy>> GetCompletions(string statement);
        Task Execute(string statement);
    }
}