
using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;



namespace DoAn4.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly DataContext _context;
        
        public ImageRepository(DataContext context)
        {
            _context = context;
        
        }
        public async Task CreateImageAsync(Images image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task<Images> GetImageByIdAsync(Guid imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            return image;
        }

        public async Task<Images> GetImageByIdPostAsync(Guid postId)
        {
            var image = await _context.Images.FindAsync(postId);
            return image;
        }

        public async Task<List<Images>> GetRemovedImagesFromPost(Post post, List<string>? Images)
        {
            var removedImages = post.Images.Where(i => Images.Contains(i.ImageLink)).ToList();
            return removedImages;   
        }

        public async Task RemoveImageAsync(Guid imageId)
        {
            var image = await _context.Images.FindAsync(imageId);

            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

       
    }
}
