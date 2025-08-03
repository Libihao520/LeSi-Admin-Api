# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. 复制解决方案和项目文件（利用Docker缓存机制）
COPY ["LeSi-Admin-Api.sln", "./"]
COPY ["LeSi.Admin.WebApi/LeSi.Admin.WebApi.csproj", "LeSi.Admin.WebApi/"]
COPY ["LeSi.Admin.Application/LeSi.Admin.Application.csproj", "LeSi.Admin.Application/"]
COPY ["LeSi.Admin.Contracts/LeSi.Admin.Contracts.csproj", "LeSi.Admin.Contracts/"]
COPY ["LeSi.Admin.Domain/LeSi.Admin.Domain.csproj", "LeSi.Admin.Domain/"]
COPY ["LeSi.Admin.Infrastructure/LeSi.Admin.Infrastructure.csproj", "LeSi.Admin.Infrastructure/"]
COPY ["LeSi.Admin.Shared/LeSi.Admin.Shared.csproj", "LeSi.Admin.Shared/"]

# 2. 还原依赖
RUN dotnet restore "LeSi-Admin-Api.sln"

# 3. 复制所有源代码
COPY . .

# 4. 发布项目（指定Release配置，禁用AppHost）
RUN dotnet publish "LeSi.Admin.WebApi/LeSi.Admin.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
# 使用非root用户运行（安全性）
USER $APP_UID
WORKDIR /app

# 复制发布产物
COPY --from=build /app/publish .

# 暴露端口（HTTP + gRPC）
EXPOSE 5158
EXPOSE 5159

# 入口点
ENTRYPOINT ["dotnet", "LeSi.Admin.WebApi.dll"]