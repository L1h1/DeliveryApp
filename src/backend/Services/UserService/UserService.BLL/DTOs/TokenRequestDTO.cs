using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.DTOs
{
    public class TokenRequestDTO
    {
        required public string AccessToken { get; set; }
        required public string RefreshToken { get; set; }
    }
}
