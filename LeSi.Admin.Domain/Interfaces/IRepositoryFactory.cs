using LeSi.Admin.Domain.Enums;

namespace LeSi.Admin.Domain.Interfaces;

public interface IRepositoryFactory
{
    IRepository GetRepository(DatabaseCategory category);
    IRepository DictionaryRepository();
    IRepository UserRepository();
}