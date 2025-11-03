# LeSi-Admin-Api 项目类库结构详解

## 2. 类库结构详解

### 2.1 类库：LeSi.Admin.Shared

**主要职责：** 提供全项目共享的工具类、枚举和通用功能。

| 文件夹 | 主要职责描述 | 包含的主要类/文件 | 类的具体作用 |
|--------|------------|-----------------|------------|
| Enums | 定义项目中使用的枚举类型 | DatabaseEnum.cs | 定义数据库类型（SqlServer, MySql, Oracle, PostgreSql, Sqlite）和数据库分类（User, Dictionary） |
| Utilities/Encryption | 提供加密相关工具方法 | AesUtilities.cs | AES加密工具类，提供加密和解密方法 |
|  |  | Md5Utilities.cs | MD5加密工具类，提供对字符串和文件流的MD5哈希计算 |
| Utilities/Validation | 提供数据验证相关工具方法 | ValidationHelper.cs | 验证辅助类，提供邮箱、手机号和密码强度验证 |

### 2.2 类库：LeSi.Admin.Contracts

**主要职责：** 定义系统的接口、命令、查询、DTO和异常，作为各层之间的契约。

| 文件夹 | 主要职责描述 | 包含的主要类/文件 | 类的具体作用 |
|--------|------------|-----------------|------------|
| ApiResponse | 定义API统一响应模型 | ApiResponse.cs | API响应模型，包含状态码、消息、数据和错误信息 |
| Exceptions | 定义系统异常类 | BusinessException.cs | 业务异常类，包含错误代码和消息 |
|  |  | ForbiddenException.cs | 权限不足异常类 |
|  |  | NotFoundException.cs | 资源未找到异常类，可指定资源类型 |
|  |  | UnauthorizedException.cs | 未授权异常类 |
|  |  | ValidationException.cs | 验证异常类，包含错误代码和字段信息 |
| Models/UserManagement/Commands | 定义用户管理相关命令 | CreateUserCommand.cs | 创建用户命令，包含名称、密码、邮箱和角色字段 |
|  |  | UpdateUserCommand.cs | 更新用户命令，包含ID、名称、邮箱和角色字段 |
|  |  | DeleteUserCommand.cs | 删除用户命令，包含ID字段 |
| Models/User/Dtos | 定义用户相关数据传输对象 | GetPublicKeyDto.cs | 获取公钥的数据传输对象，包含公钥字段 |
|  |  | LoginDto.cs | 登录结果数据传输对象，包含令牌字段 |

### 2.3 类库：LeSi.Admin.Domain

**主要职责：** 包含领域模型（实体、值对象）、领域服务接口和仓储接口，实现核心业务逻辑。

| 文件夹 | 主要职责描述 | 包含的主要类/文件 | 类的具体作用 |
|--------|------------|-----------------|------------|
| Entities | 定义核心业务实体 | BaseEntity.cs | 基础实体抽象类，包含自增Id属性 |
|  |  | AuditableBaseEntity.cs | 带审计字段的实体基类，继承BaseEntity，包含创建人、创建日期等字段 |
|  |  | User/UsersEntity.cs | 用户实体类，继承AuditableBaseEntity，包含用户名、密码、角色等属性 |
| Interfaces | 定义领域服务接口 | ITokenService.cs | 令牌服务接口，定义GetToken方法 |
|  |  | IKeyPairManager.cs | 密钥对管理接口，提供获取并转移密钥对的方法 |
| Interfaces/Repository | 定义仓储接口 | IRepository.cs | 通用仓储接口，定义基础数据操作方法 |
|  |  | IRepositoryFactory.cs | 仓储工厂接口，提供获取各种仓储的方法 |

### 2.4 类库：LeSi.Admin.Application

**主要职责：** 实现CQRS模式的命令和查询处理器，协调领域对象和基础设施。

| 文件夹 | 主要职责描述 | 包含的主要类/文件 | 类的具体作用 |
|--------|------------|-----------------|------------|
| User/QueryHandlers | 处理用户认证相关查询 | GetPublicKeyQueryHandler.cs | 获取公钥查询处理器，从keyPairManager获取密钥对 |
|  |  | GetTokenQueryHandler.cs | 获取令牌查询处理器，处理用户登录并生成令牌 |
| UserManagement/CommandHandlers | 处理用户管理相关命令 | CreateUserCommandHandler.cs | 创建用户命令处理器，实现用户名和邮箱唯一性检查、密码验证等 |
|  |  | UpdateUserCommandHandler.cs | 更新用户命令处理器（当前为空实现） |
|  |  | DeleteUserCommandHandler.cs | 删除用户命令处理器（当前为空实现） |
| UserManagement/QueryHandlers | 处理用户管理相关查询 | GetUserByIdQueryHandler.cs | 根据ID获取用户查询处理器（当前为空实现） |
|  |  | GetUserListQueryHandler.cs | 获取用户列表查询处理器（当前为空实现） |
| Area/QueryHandlers | 处理区域相关查询 | AreaQueryHandler.cs | 区域查询处理器（当前未查看具体实现） |
| Dictionary/QueryHandlers | 处理字典相关查询 | DictionaryQueryHandler.cs | 字典查询处理器（当前未查看具体实现） |

### 2.5 类库：LeSi.Admin.Infrastructure

**主要职责：** 提供基础设施实现，包括数据访问、缓存、服务实现等技术细节。

| 文件夹 | 主要职责描述 | 包含的主要类/文件 | 类的具体作用 |
|--------|------------|-----------------|------------|
| Data/Database | 提供数据库访问相关功能 | DatabaseParameterFactory.cs | 数据库参数工厂，创建数据库参数 |
|  |  | DbHelper.cs | 数据库帮助类，提供数据库操作辅助方法 |
|  |  | IDatabase.cs | 数据库接口，定义数据库操作方法 |
|  |  | MySqlDatabase.cs | MySQL数据库实现 |
|  |  | PostgreSqlDatabase.cs | PostgreSQL数据库实现 |
|  |  | SqLiteDatabase.cs | SQLite数据库实现 |
| Data/DbContexts | Entity Framework Core上下文 | MySqlDbContext.cs | MySQL的EF Core上下文实现 |
|  |  | PgsqlDbContext.cs | PostgreSQL的EF Core上下文实现 |
|  |  | SqlServerDbContext.cs | SQL Server的EF Core上下文实现 |
|  |  | SqliteDbContext.cs | SQLite的EF Core上下文实现 |
| CaChe | 缓存实现 | CacheKeys.cs | 缓存键定义类 |
|  |  | KeyPairManager.cs | 密钥对管理器实现 |
|  |  | MemoryCacheImp.cs | 内存缓存实现 |
|  |  | RedisCacheImp.cs | Redis缓存实现 |
| Repository | 仓储实现 | Repository.cs | 通用仓储实现，实现IRepository接口 |
|  |  | RepositoryFactory.cs | 仓储工厂实现，实现IRepositoryFactory接口 |
| Services | 服务实现 | TokenService.cs | 令牌服务实现，实现ITokenService接口 |
| Config | 配置相关 | GlobalContext.cs | 全局上下文 |
|  |  | SystemConfig.cs | 系统配置类 |
| Logging | 日志相关 | NLogLogger.cs | NLog日志实现 |

### 2.6 类库：LeSi.Admin.WebApi

**主要职责：** API层，处理HTTP请求，路由到相应的处理器，并返回响应。

| 文件夹 | 主要职责描述 | 包含的主要类/文件 | 类的具体作用 |
|--------|------------|-----------------|------------|
| Controllers | API控制器 | AreaController.cs | 区域相关API控制器 |
|  |  | DictionaryController.cs | 字典相关API控制器 |
|  |  | UserController.cs | 用户认证相关API控制器 |
|  |  | UserManagementController.cs | 用户管理相关API控制器 |
| Middleware | 中间件 | ExceptionHandlerMiddleware.cs | 异常处理中间件，统一处理系统异常 |
| Filter | 过滤器 | ApiResponseFilter.cs | API响应过滤器，统一包装响应格式 |
|  |  | ModelValidationFilter.cs | 模型验证过滤器，验证请求模型 |
| ProtoService | gRPC服务 | AuthService.cs | 认证相关gRPC服务实现 |
| Protos | gRPC协议定义 | user.proto | 用户相关gRPC服务协议定义 |
| Data | 数据文件 | admin.db | SQLite数据库文件 |
|  | 配置文件 | AutoMapperConfigs.cs | AutoMapper配置类 |
|  |  | HostBuilderExtend.cs | 主机构建器扩展方法 |
|  |  | Program.cs | 应用程序入口点 |
        