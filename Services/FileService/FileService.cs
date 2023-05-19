

namespace DoAn4.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        public FileService(IWebHostEnvironment env)
        {
            _environment = env;
        }

        public async Task<string> SaveFile(IFormFile File)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "FileUploads");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check the allowed extensions
                var ext = Path.GetExtension(File.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg", ".mp4", ".avi", ".mov" };

                if (!allowedExtensions.Contains(ext))
                {
                    string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return msg;
                }

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                string subPath = "";

                if (ext == ".mp4" || ext == ".avi" || ext == ".mov")
                {
                    subPath = "Videos";
                }
                else
                {
                    subPath = "Images";
                }

                path = Path.Combine(path, subPath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fileWithPath = Path.Combine(path, newFileName);
                using (var stream = new FileStream(fileWithPath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }

                return Path.Combine("FileUploads", subPath, newFileName);
            }
            catch (Exception ex)
            {
                return "Error has occurred";
            }
        }


        public async Task<bool> DeleteFiles(List<string> filePaths)
        {
            try
            {
                foreach (var filePath in filePaths)
                {
                    if (File.Exists(Path.Combine(_environment.ContentRootPath, filePath)))
                    {
                        File.Delete(Path.Combine(_environment.ContentRootPath, filePath));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                return false;
            }
        }
    }
}