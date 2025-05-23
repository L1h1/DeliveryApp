namespace UserService.BLL.DTOs.Request
{
    public sealed record ResetPasswordRequestDTO
    {
        required public string Email { get; set; }
        required public string ResetCode { get; set; }
        required public string NewPassword { get; set; }
    }
}
