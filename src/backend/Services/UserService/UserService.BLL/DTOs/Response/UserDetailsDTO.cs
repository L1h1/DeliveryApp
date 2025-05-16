using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.DTOs.Response
{
    public sealed record UserDetailsDTO
    {
        required public string UserName { get; set; }
        required public string Email { get; set; }
        required public string PhoneNumber { get; set; }
        required public bool EmailConfirmed { get; set; }
        required public bool PhoneNumberConfirmed { get; set; }
    }
}
