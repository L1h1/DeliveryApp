using System;
using System.Collections.Generic;
using System.Linq;
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
