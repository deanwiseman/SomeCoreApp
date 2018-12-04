using System.Collections.Generic;
using Newtonsoft.Json;
using SomeCoreApp.API.Models;
using User_PC.Desktop.SomeCoreApp.Data;

namespace SomeCoreApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            
            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;
                CreatePassWord("password", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _context.Users.Add(user);
            }

            _context.SaveChanges();
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
    }
}