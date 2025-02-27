using PAUYS.Common.Enums;
using PAUYS.DTO.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAUYS.DTO.Concrete
{
    public class AppUserDto:BaseDto
    {
        public string Name { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; }
        public string Email { get; set; }

        public UserTypes UserType { get; set; }

        // Opsiyonel olarak, kullanıcının profil fotoğrafı URL'sini tutabilirsiniz
        public string? PictureFileName { get; set; } = string.Empty;
    }
}
