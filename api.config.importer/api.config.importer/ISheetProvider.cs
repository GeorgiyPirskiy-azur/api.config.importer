using System;
using System.Threading;
using System.Threading.Tasks;

namespace api.config.importer
{
    public interface ISheetProvider : IDisposable
    {
        Task<ISheet> GetSheet(string name, CancellationToken token);
    }
}
