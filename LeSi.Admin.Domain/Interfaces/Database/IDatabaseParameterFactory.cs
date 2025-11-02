using System.Data.Common;

namespace LeSi.Admin.Domain.Interfaces.Database;

public interface IDatabaseParameterFactory
{
    DbParameter CreateParameter();
    DbParameter CreateParameter(string paramName, object value);
}