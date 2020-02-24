using System.Threading.Tasks;
using SampleArchitecture.Core.Implementations;

namespace SampleArchitecture.Core.Interfaces
{
    public interface IUnitOfWork
    {
        void RegisterDelete(IEntity entity, UnitOfWorkRepositoryBase<IEntity> entityRepository);

        void RegisterUpdate(IEntity entity, UnitOfWorkRepositoryBase<IEntity> entityRepository);

        void RegisterAdd(IEntity entity, UnitOfWorkRepositoryBase<IEntity> entityRepository);


         Task<bool> CommitChangesAsync();
    }

}
