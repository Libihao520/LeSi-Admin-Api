using System.Data.Common;

namespace LeSi.Admin.Domain.Interfaces;

public interface IDatabaseParameterFactory
{
    DbParameter CreateParameter();
    DbParameter CreateParameter(string paramName, object value);
}