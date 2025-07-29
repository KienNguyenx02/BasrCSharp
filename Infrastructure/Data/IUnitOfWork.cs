using System.Threading.Tasks;

namespace WebApplication1.Infrastructure.Data
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();
    }
}