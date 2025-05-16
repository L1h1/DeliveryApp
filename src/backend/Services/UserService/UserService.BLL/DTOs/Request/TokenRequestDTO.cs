using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.DTOs.Request
{
    public sealed record TokenRequestDTO
    {
        required public string AccessToken { get; set; }
        required public string RefreshToken { get; set; }
    }
}
