using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Infrastructure.Persistence.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ApplicationDbContext _context;

        public OtpRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(PasswordResetOtp otp)
        {
            _context.PasswordResetOtps.Add(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetOtp?> GetValidOtpAsync(string email, string otp)
        {
            return await _context.PasswordResetOtps
                .Where(x => x.Email == email &&
                            x.Otp == otp &&
                            !x.IsUsed &&
                            x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PasswordResetOtp otp)
        {
            _context.PasswordResetOtps.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}
