using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Domain.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task BeginAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
