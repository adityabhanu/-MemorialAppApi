using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Interfaces
{
    public interface IOtpRepository
    {
        Task CreateAsync(PasswordResetOtp otp);

        Task<PasswordResetOtp?> GetValidOtpAsync(string email, string otp);

        Task UpdateAsync(PasswordResetOtp otp);
    }
}
