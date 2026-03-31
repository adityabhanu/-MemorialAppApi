using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IUserRepository _userRepo;
        private readonly IOtpRepository _otpRepo;

        public ResetPasswordCommandHandler(
            IUserRepository userRepo,
            IOtpRepository otpRepo)
        {
            _userRepo = userRepo;
            _otpRepo = otpRepo;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            var otpRecord = await _otpRepo.GetValidOtpAsync(request.Email, request.Otp);

            if (otpRecord == null)
                throw new Exception("Invalid or expired OTP");

            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null)
                throw new Exception("User not found");

            var passwordBytes = Encoding.UTF8.GetBytes(request.NewPassword);
            var passwordHash = Convert.ToBase64String(passwordBytes);

            user.PasswordHash = passwordHash;

            await _userRepo.UpdateAsync(user);

            otpRecord.IsUsed = true;
            await _otpRepo.UpdateAsync(otpRecord);

            return true;
        }
    }
}
