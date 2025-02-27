namespace PAUYS.AspNetCoreMvc.Services.Abstract
{
    public interface IFileUpload
    {
        string UploadFileName { get; }
        void Upload(IFormFile formFile);
    }
}
