using System.Data.Common;
using LeSi.Admin.Contracts.Area;
using LeSi.Admin.Infrastructure.Extensions;
using LeSi.Admin.Infrastructure.Logging;
using LeSi.Admin.Infrastructure.Repository;
using MediatR;


namespace LeSi.Admin.Application.Area.QueryHandlers;

public class AreaQueryHandler : RepositoryFactory, IRequestHandler<Queries.GetAreaDtoQuery, List<Dtos.AreaDto>>
{
    public async Task<List<Dtos.AreaDto>> Handle(Queries.GetAreaDtoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            LogHelper.Info("开始处理区域查询请求");
            var sql = @"
SELECT 
    LEFT(tbl_region_province.region_code,2)     as Sheng,
    tbl_region_province.province_name           as ShengName,
    LEFT(tbl_region_city.region_code,4)         as Shi,
    tbl_region_city.city_name                   as ShiName,
    LEFT(tbl_region_county.region_code,6)       as Xian,
    tbl_region_county.county_name               as XianName,
    LEFT(tbl_region_township.region_code,9)     as Xiang,
    tbl_region_township.township_name           as XiangName
FROM tbl_region_province 
LEFT JOIN tbl_region_city ON  LEFT(tbl_region_province.region_code, 2) = LEFT(tbl_region_city.region_code, 2)
LEFT JOIN tbl_region_county ON  LEFT(tbl_region_city.region_code, 4) = LEFT(tbl_region_county.region_code, 4)
LEFT JOIN tbl_region_township ON  LEFT(tbl_region_county.region_code, 6) = LEFT(tbl_region_township.region_code, 6)
WHERE LEFT(tbl_region_province.region_code,2) = @Code
";
            var parameter = new List<DbParameter>();
            parameter.Add(DatabasesExtension.CreateDbParameter("@Code", request.Code));

            var areaList = await DictionaryRepository().FindList<Dtos.AreaDto>(sql, parameter.ToArray());
            return areaList.ToList();
        }
        catch (Exception ex)
        {
            LogHelper.Error("查询异常日志", ex);
            throw;
        }
    }
}