namespace UserService.BLL.DTOs.Response
{
    public sealed record UserResponseDTO
    {
        required public string UserName { get; set; }
        required public string Email { get; set; }
        required public string AccessToken { get; set; }
        required public string RefreshToken { get; set; }
    }
}
