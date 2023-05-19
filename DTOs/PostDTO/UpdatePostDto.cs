namespace DoAn4.DTOs.PostDTO
{
    public class UpdatePostDto
    {
        
        public string? Content { get; set; }=null;
        public List<Guid>? IdImagesRemove { get; set; } = null;
        public List<Guid>? IdVideosRemove { get; set; } = null;
        public List<IFormFile>? NewImages { get; set; } = null;
        public List<IFormFile>? Newvideos { get; set; } = null;
    }
}
