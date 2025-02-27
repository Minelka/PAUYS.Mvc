

using PAUYS.AspNetCoreMvc.Services.Abstract;

namespace BookStore.AspNetCoreMvc.Services.Concrete
{
    public class FileUpload : IFileUpload
    {
        private readonly string _uploadPath;
        private string _uploadFileName = "";

        public FileUpload(string subFolderName = "")
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin", "uploads");
            
            if(!string.IsNullOrEmpty(subFolderName))
                _uploadPath = Path.Combine(_uploadPath, subFolderName);

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

        }

        public string UploadFileName => _uploadFileName;

        public void Upload(IFormFile formFile)
        {
            var fileNameWithOutExtention = Path.GetFileNameWithoutExtension(formFile.FileName);
            var fileExtention = Path.GetExtension(formFile.FileName);

            var fileName = $"{fileNameWithOutExtention}_{Guid.NewGuid()}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtention}";



            var filePath = Path.Combine(_uploadPath, fileName);


            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                //Bu fiziksel dosya kopyalaması
                formFile.CopyTo(fileStream);

                _uploadFileName = fileStream.Name.Split("wwwroot")[1].Replace("//","/");
            }
        }
    }
}
