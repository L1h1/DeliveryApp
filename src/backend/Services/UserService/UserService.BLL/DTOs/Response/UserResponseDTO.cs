using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.DTOs.Response
{
    public class UserResponseDTO
    {
        required public string UserName { get; set; }
        required public string Email { get; set; }
        required public string AccessToken { get; set; }
        required public string RefreshToken { get; set; }
    }
}
