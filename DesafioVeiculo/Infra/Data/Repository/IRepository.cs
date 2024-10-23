
namespace DesafioVeiculos.Infra.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> ObterTodosAsync();
        Task<T> ObterPorIdAsync(int id);
        Task AdicionarAsync(T entity);
        Task AtualizarAsync(T entity);
        Task DeletarAsync(T entity);
        IQueryable<T> AsQueryable();
    }
}
