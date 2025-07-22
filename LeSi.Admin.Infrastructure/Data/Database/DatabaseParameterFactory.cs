using System.Data.Common;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Infrastructure.Extensions;

namespace LeSi.Admin.Infrastructure.Data.Database;

public class DatabaseParameterFactory: IDatabaseParameterFactory
{
    public DbParameter CreateParameter() 
        => DatabasesExtension.CreateDbParameter();

    public DbParameter CreateParameter(string paramName, object value) 
        => DatabasesExtension.CreateDbParameter(paramName, value);
}