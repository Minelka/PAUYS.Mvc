using PAUYS.Common.Enums;
using PAUYS.DTO.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAUYS.DTO.Concrete
{
    public class RoleDto: BaseDto
    {
        public string Name { get; set; }
        public UserTypes UserType { get; set; }  // Enum tipinde
    }
}
