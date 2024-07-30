using System.Data;
using System.Data.Common;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
