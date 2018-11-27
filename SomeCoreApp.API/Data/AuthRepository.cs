using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SomeCoreApp.API.Models;
using User_PC.Desktop.SomeCoreApp.Data;

namespace SomeCoreApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;

        }

        // Login a user
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
                 return null;

            if(!VerifyPassWordHash(password, user.PasswordHash, user.PasswordSalt))
                 return null;

            return user;

        }

        private bool VerifyPassWordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }

            return true;
        }

        // Register a new User
        public async Task<User> Register(User user, string password)
        {
            
            byte[] passWordHash, passwordSalt;
            CreatePassWord(password, out passWordHash, out passwordSalt);

            // Set Hash and Salt values into user object, passed through via OUT variables
            user.PasswordHash = passWordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePassWord(string password, out byte[] passWordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // Generate random salt kkey from hmac
                passwordSalt = hmac.Key;
                // Generate Hash, encode + translate string password into byte array
                passWordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }
    }
}