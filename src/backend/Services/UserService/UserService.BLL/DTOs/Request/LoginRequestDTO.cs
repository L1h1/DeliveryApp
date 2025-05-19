namespace UserService.BLL.DTOs.Request
{
    public sealed record LoginRequestDTO
    {
        required public string Email { get; set; }
        required public string Password { get; set; }
    }
}
