namespace AirbnbProj2.Models
{
    // User DTO for sending users without sensitive data (password) 
    public class UserDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        

        public static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Email = user.Email,
                FullName = $"{user.FirstName} {user.FamilyName}",
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin
            };
        }
    }
}
