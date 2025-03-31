using System.Security.Cryptography;
using System.Text;

namespace Com.Store.Orders.Domain.Data.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();

            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
