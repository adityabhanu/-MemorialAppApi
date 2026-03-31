using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IUserRepository _userRepo;
        private readonly IOtpRepository _otpRepo;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepo,
            IOtpRepository otpRepo,
            IEmailService emailService)
        {
            _userRepo = userRepo;
            _otpRepo = otpRepo;
            _emailService = emailService;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            // 🔒 Do NOT reveal if user exists
            if (user == null)
                return true;

            var otp = new Random().Next(100000, 999999).ToString();

            await _otpRepo.CreateAsync(new PasswordResetOtp
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Otp = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            });

            await _emailService.SendAsync(
                request.Email,
                "RISE - Password Reset OTP",
                $"Your OTP to update your password is {otp}. It will expire in 15 minutes."
            );

            return true;
        }
    }
}
