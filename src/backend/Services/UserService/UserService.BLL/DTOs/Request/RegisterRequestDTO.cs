namespace UserService.BLL.DTOs.Request
{
    public sealed record RegisterRequestDTO
    {
        required public string UserName { get; set; }
        required public string Email { get; set; }
        required public string Password { get; set; }
        required public string PhoneNumber { get; set; }
    }
}
