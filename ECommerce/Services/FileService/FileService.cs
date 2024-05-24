namespace ECommerce.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        bool IFileService.DeleteImage(string imageFileName)
        {
            try
            {
                var wwwPath = _environment.WebRootPath;
                var path = Path.Combine(wwwPath, "Uploads\\", imageFileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        Tuple<int, string> IFileService.SaveImage(IFormFile imageFile)
        {
            try
            {
                // Lấy đường dẫn Uploads
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads");

                // Kiểm tra tồn tại chưa nếu chưa thì tạo mới
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Kiểm tra đuôi file được phép upload ảnh
                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg", ".webp" };

                if (!allowedExtensions.Contains(ext))
                {
                    string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, msg);
                }

                string uniqueString = Guid.NewGuid().ToString();

                // Tạo tên file mới 
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);

                imageFile.CopyTo(stream);

                stream.Close();
                return new Tuple<int, string>(1, newFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, ex.Message);
            }
        }
    }
}
