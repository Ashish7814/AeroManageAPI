using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IDapperUnitOfWork
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
    }
}
