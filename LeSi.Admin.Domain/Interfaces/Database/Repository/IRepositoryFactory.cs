using LeSi.Admin.Shared.Enums;

namespace LeSi.Admin.Domain.Interfaces.Database.Repository;

public interface IRepositoryFactory
{
    IRepository GetRepository(DatabaseCategory category);
    IRepository DictionaryRepository();
    IRepository UserRepository();
}