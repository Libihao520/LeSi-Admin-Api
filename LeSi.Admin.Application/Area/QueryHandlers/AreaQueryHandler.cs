using System.Data.Common;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Contracts.Models.Area;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Database;
using LeSi.Admin.Domain.Interfaces.Database.Repository;
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
        string sql = string.Empty;
        var parameter = new List<DbParameter>();
        if (string.IsNullOrEmpty(request.Code))
        {
            sql = @"SELECT 
LEFT(administrative_region_code::varchar, 4) AS Sheng,
    administrative_region_name AS ShengName
FROM administrative_region WHERE administrative_division=1";
        }
        else if (request.Code.Length == 2)
        {
            sql = @"SELECT
    LEFT(p.administrative_region_code::varchar, 2) AS Sheng,
    p.administrative_region_name AS ShengName,
    LEFT(c.administrative_region_code::varchar, 4) AS Shi,
    c.administrative_region_name AS ShiName
    FROM administrative_region p
    INNER JOIN administrative_region c ON LEFT(c.administrative_region_code::varchar, 2) = LEFT(p.administrative_region_code::varchar, 2)
    WHERE p.administrative_division = 1
    AND c.administrative_division = 2
    AND LEFT(p.administrative_region_code::varchar, 2) = @Code";
            parameter.Add(parameterFactory.CreateParameter("@Code", request.Code));
        }
        else if (request.Code.Length == 4)
        {
            sql = @"SELECT
    LEFT(p.administrative_region_code::varchar, 2) AS Sheng,
    p.administrative_region_name AS ShengName,
    LEFT(c.administrative_region_code::varchar, 4) AS Shi,
    c.administrative_region_name AS ShiName,
    LEFT(x.administrative_region_code::varchar, 6) AS Xian,
    x.administrative_region_name AS XianName
    FROM administrative_region p
    INNER JOIN administrative_region c ON LEFT(c.administrative_region_code::varchar, 2) = LEFT(p.administrative_region_code::varchar, 2)
    INNER JOIN administrative_region x ON LEFT(x.administrative_region_code::varchar, 4) = LEFT(c.administrative_region_code::varchar, 4)
    WHERE p.administrative_division = 1
    AND c.administrative_division = 2
    AND x.administrative_division = 3
    AND LEFT(c.administrative_region_code::varchar, 4) = @Code";
            parameter.Add(parameterFactory.CreateParameter("@Code", request.Code));
        }
        else if (request.Code.Length == 6)
        {
            sql = @"SELECT
    LEFT(p.administrative_region_code::varchar, 2) AS Sheng,
    p.administrative_region_name AS ShengName,
    LEFT(c.administrative_region_code::varchar, 4) AS Shi,
    c.administrative_region_name AS ShiName,
    LEFT(x.administrative_region_code::varchar, 6) AS Xian,
    x.administrative_region_name AS XianName,
    LEFT(t.administrative_region_code::varchar, 9) AS Xiang,
    t.administrative_region_name AS XiangName
    FROM administrative_region p
    INNER JOIN administrative_region c ON LEFT(c.administrative_region_code::varchar, 2) = LEFT(p.administrative_region_code::varchar, 2)
    INNER JOIN administrative_region x ON LEFT(x.administrative_region_code::varchar, 4) = LEFT(c.administrative_region_code::varchar, 4)
    INNER JOIN administrative_region t ON LEFT(t.administrative_region_code::varchar, 6) = LEFT(x.administrative_region_code::varchar, 6)
    WHERE p.administrative_division = 1
    AND c.administrative_division = 2
    AND x.administrative_division = 3
    AND t.administrative_division = 4
    AND LEFT(x.administrative_region_code::varchar, 6) = @Code";
            parameter.Add(parameterFactory.CreateParameter("@Code", request.Code));
        }
        else if (request.Code.Length == 9)
        {
            sql = @"SELECT
    LEFT(p.administrative_region_code::varchar, 2) AS Sheng,
    p.administrative_region_name AS ShengName,
    LEFT(c.administrative_region_code::varchar, 4) AS Shi,
    c.administrative_region_name AS ShiName,
    LEFT(x.administrative_region_code::varchar, 6) AS Xian,
    x.administrative_region_name AS XianName,
    LEFT(t.administrative_region_code::varchar, 9) AS Xiang,
    t.administrative_region_name AS XiangName,
    v.administrative_region_code AS Cun,
    v.administrative_region_name AS CunName
    FROM administrative_region p
    INNER JOIN administrative_region c ON LEFT(c.administrative_region_code::varchar, 2) = LEFT(p.administrative_region_code::varchar, 2)
    INNER JOIN administrative_region x ON LEFT(x.administrative_region_code::varchar, 4) = LEFT(c.administrative_region_code::varchar, 4)
    INNER JOIN administrative_region t ON LEFT(t.administrative_region_code::varchar, 6) = LEFT(x.administrative_region_code::varchar, 6)
    INNER JOIN administrative_region v ON LEFT(v.administrative_region_code::varchar, 9) = LEFT(t.administrative_region_code::varchar, 9)
    WHERE p.administrative_division = 1
    AND c.administrative_division = 2
    AND x.administrative_division = 3
    AND t.administrative_division = 4
    AND v.administrative_division = 5
    AND LEFT(v.administrative_region_code::varchar, 9) = @Code";
            parameter.Add(parameterFactory.CreateParameter("@Code", request.Code));
        }


        logger.Info("开始处理区域查询请求");
        try
        {
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