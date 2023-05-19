using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;


namespace DoAn4.Repositories
{
    public class NotifyRepository : INotifyRepository
    {
        private readonly DataContext _context;
     

        public NotifyRepository(DataContext context)
        {
            _context = context;
            
        }
        public async Task CreateNotifyAsync(Notify notify)
        {
            await _context.Notifies.AddAsync(notify);
            await _context.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
