namespace UserService.BLL.DTOs.Request
{
    public sealed record TokenRequestDTO
    {
        required public string AccessToken { get; set; }
        required public string RefreshToken { get; set; }
    }
}
