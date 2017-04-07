using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace LetMeKnowApi.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method to add pagination info to Response headers
        /// </summary>
        /// <param name="response"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalItems"></param>
        /// <param name="totalPages"></param>
        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            response.Headers.Add("Pagination",
               Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));
            // CORS
            response.Headers.Add("access-control-expose-headers", "Pagination");
        }

        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            // CORS
            response.Headers.Add("access-control-expose-headers", "Application-Error");
        }

        /// <summary>
        ///     Creates a random salt to be used for encrypting a password
        /// </summary>
        /// <returns></returns>
        public static string CreateSalt()
        {
            /*var data = new byte[0x10];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetBytes(data);
                return Convert.ToBase64String(data);
            }*/

            //https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create()) {
                keyGenerator.GetBytes(salt);        
                return BitConverter.ToString(salt).Replace("-", "").ToLower();
                //return Convert.ToBase64String(salt);
            }            
        }

        /// <summary>
        ///     Encrypts a password using the given salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string EncryptPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = string.Format("{0}{1}", salt, password);
                var saltedPasswordAsBytes = Encoding.UTF8.GetBytes(saltedPassword);
                return Convert.ToBase64String(sha256.ComputeHash(saltedPasswordAsBytes));
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            /*string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            
            return hashed;*/
        }
    }
}
