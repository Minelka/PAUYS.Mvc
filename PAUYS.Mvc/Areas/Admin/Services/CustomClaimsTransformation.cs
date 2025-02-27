using Microsoft.AspNetCore.Authentication;
using PAUYS.ViewModel.Concrete;
using System.Security.Claims;

namespace PAUYS.AspNetCoreMvc.Areas.Admin.Services
{
    public class CustomClaimsTransformation : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                // Kullanıcıya ait claims bilgilerini alarak UserDTO'ya aktarıyoruz
                var userDTO = new AppUserViewModel();

                // ClaimsPrincipal içerisindeki claims bilgilerini alıyoruz
                var userNameClaim = principal.FindFirst(ClaimTypes.Name);
                var emailClaim = principal.FindFirst(ClaimTypes.Email);

                // Claims'leri DTO'ya aktaralım
                userDTO.UserName = userNameClaim?.Value;
                userDTO.Email = emailClaim?.Value;

                // Burada userDTO'yu kullanarak ilgili işlemleri yapabilirsiniz.
                // Örneğin: SaveUserDTO(userDTO); veya başka işlemler yapılabilir.

                // ClaimsPrincipal geri döndürülür (kimlik eklenmiş)
                principal.AddIdentity(identity);
            }

            return principal;
        }
    }

}

