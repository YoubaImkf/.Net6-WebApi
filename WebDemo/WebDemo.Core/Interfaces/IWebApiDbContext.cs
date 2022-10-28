using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using WebApiDemo.Core.Models;
using WebDemo.Core.Models;

namespace WebDemo.Core.Interfaces
{
    public interface IWebApiDbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Device> Device { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry Update([NotNull] object entity);
        EntityEntry<TEntity> Update<TEntity>([NotNull] TEntity entity) where TEntity : class;
        void UpdateRange([NotNull] IEnumerable<object> entities);
        void UpdateRange([NotNull] params object[] entities);
        EntityEntry Entry([NotNullAttribute] object entity);
        EntityEntry<TEntity> Entry<TEntity>([NotNullAttribute] TEntity entity) where TEntity : class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
