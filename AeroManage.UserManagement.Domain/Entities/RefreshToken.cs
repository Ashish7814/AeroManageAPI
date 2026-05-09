using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.UserManagement.Domain.Entities
{
    public class RefreshToken
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
