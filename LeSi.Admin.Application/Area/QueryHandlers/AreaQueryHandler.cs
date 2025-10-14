using System.Data.Common;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Contracts.Models.Area;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.IdentityModel.Logging;


namespace LeSi.Admin.Application.Area.QueryHandlers;

public class AreaQueryHandler(
    IAppLogger logger,
    IRepositoryFactory repositoryFactory,
    IDatabaseParameterFactory parameterFactory)
    : IRequestHandler<Queries.GetAreaDtoQuery, List<Dtos.AreaDto>>
{
    public async Task<List<Dtos.AreaDto>> Handle(Queries.GetAreaDtoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.Info("开始处理区域查询请求");
            var sql = @"SELECT
LEFT(administrative_region_code::varchar, 4) AS Shi,
administrative_region_name AS ShiName
FROM administrative_region
WHERE administrative_division = 2
and Left(administrative_region_code::varchar, 2) = @Code";
            var parameter = new List<DbParameter>();
            parameter.Add(parameterFactory.CreateParameter("@Code", request.Code));

            var areaList = await repositoryFactory.DictionaryRepository()
                .QueryAsync<Dtos.AreaDto>(sql, parameter.ToArray());
            return areaList.ToList();
        }
        catch (Exception ex)
        {
            logger.Error("查询异常日志", ex);
            throw;
        }
    }
}