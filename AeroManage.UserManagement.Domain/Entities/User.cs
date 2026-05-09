using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.UserManagement.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int RoleId { get; set; }
        public bool IsEmailVerified { get; set; }
        public string EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Role Roles { get; set; }

        // Navigation properties
        //public string RoleName { get; set; }
        public UserProfile Profile { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
