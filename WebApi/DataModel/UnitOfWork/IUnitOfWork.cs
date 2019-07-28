using DataModel.GenericRepository;

namespace DataModel.UnitOfWork
{
    public interface IUnitOfWork
    {
        #region Properties
        
        GenericRepository<User> UserRepository { get; }
        GenericRepository<Tokens> TokenRepository { get; }

        GenericRepository<Contact> ContactRepository { get; }

        GenericRepository<Project> ProjectRepository { get; }

        GenericRepository<Task> TaskRepository { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Save method.
        /// </summary>
        void Save(); 
        #endregion
    }
}