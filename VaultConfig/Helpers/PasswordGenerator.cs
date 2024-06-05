using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Helpers
{
    public static class PasswordGenerator
    {
        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        /// Source: https://github.com/Darkseal/PasswordGenerator/tree/master


        public static string GeneratePassword(
            int requiredLength = 8,
            int requiredUniqueChars = 4,
            bool requireDigit = true,
            bool requireLowercase = true,
            bool requireSpecialChars = true,
            bool requireUppercase = true)
        {
            
            List<string> randomChars = new List<string>();
            Random rand = new Random();
            List<char> chars = new List<char>();


            // If statements will add the chars to be included in the range of passwords. Special chars will add at least 1 if specified
            if (requireUppercase)
            {
                randomChars.Add("ABCDEFGHJKLMNOPQRSTUVWXYZ");
            }

            if (requireLowercase)
            {
                randomChars.Add("abcdefghijkmnopqrstuvwxyz");
            }

            if (requireDigit)
            {
                randomChars.Add("0123456789");
            }
            
            if (requireSpecialChars)
            {
                randomChars.Add("!@$?_-");
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);
            }
                

            for (int i = chars.Count; i < requiredLength
                || chars.Distinct().Count() < requiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Count)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
