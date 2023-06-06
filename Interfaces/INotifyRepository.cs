using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface INotifyRepository
    {
        Task CreateNotifyAsync(Notify notify);
        Task<List<Notify>> GetAllNotifyByIdUser(Guid userId);

        Task<int> SaveChangesAsync();
    }
}
