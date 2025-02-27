
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.Common.Validations
{
    public class ImageFileAttribute : ValidationAttribute
    {
        #region Tek dosya formatı kontrolü
        //private readonly string _fileType;
        //public ImageFileAttribute(string FileType)
        //{
        //    _fileType = FileType;
        //}
        //protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        //{
        //    IFormFile formFile = value as IFormFile;
        //    if(formFile.ContentType != _fileType)
        //    {
        //        return new ValidationResult($"Lütfen sadece {_fileType} formatında dosya yükleyiniz");
        //    }
        //    return ValidationResult.Success;
        //} 
        #endregion

        #region Çok Dosya formatı kontrolü
        private readonly string[] _fileType;

        public ImageFileAttribute(params string[] FileType)
        {
            _fileType = FileType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null) 
            { 

                IFormFile? formFile = value as IFormFile;

                if (!_fileType.Contains(formFile?.ContentType))
                {
                    string message = $"Lütfen sadece {string.Join(',', _fileType)} formatlarında ki dosyaları yükleyebilirsiniz";

                    return new ValidationResult(string.IsNullOrEmpty(ErrorMessage) ? message : ErrorMessage);
                }
            }

            return ValidationResult.Success;
        } 
        #endregion
    }
}
