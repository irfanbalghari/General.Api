namespace General.Core.Application.Feature.UserManagement.DTO
{
    public class ResetPasswordDto
    {
        public string userEmail { get; set; }
        public string resetToken { get; set; }
        public string newPassword { get; set; }
    }
}
