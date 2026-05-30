using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.Shared.Repositories
{
    public interface IAuditLogRepository
    {
        Task CreateAuditLogAsync(
            int? userId,
            string action,
            string entity = null,
            int? entityId = null,
            string oldValues = null,
            string newValues = null,
            string ipAddress = null,
            string userAgent = null
        );
    }
}
