namespace UserService.BLL.DTOs.Request
{
    public sealed record ChangePasswordDTO
    {
        required public string CurrentPassword { get; set; }
        required public string NewPassword { get; set; }
    }
}
