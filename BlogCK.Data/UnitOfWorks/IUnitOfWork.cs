using BlogCK.Core.Entities;
using BlogCK.Data.Repositories.Abstractions;

namespace BlogCK.Data.UnitOfWorks
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntityBase, new();

        Task<int> SaveAsync();
        int Save();
    }
}