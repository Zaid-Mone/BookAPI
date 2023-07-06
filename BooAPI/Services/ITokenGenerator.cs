using BooAPI.Models;

namespace BookAPI.Services
{
    public interface ITokenGenerator
    {
        public string CreateToken(AppUser user);
        public bool ValidateToken(string token="");
    }
}
