using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Entities
{
    public class PasswordResetOtp
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
