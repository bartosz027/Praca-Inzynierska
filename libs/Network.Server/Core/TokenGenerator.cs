using System;
using System.Text;

namespace Network.Server.Core {

    // Source: https://stackoverflow.com/questions/8877736/having-upper-case-lower-case-and-digits-in-a-guid
    internal static class TokenGenerator {
        public static string Next() {
            var rand = new Random();
            var sb = new StringBuilder();

            foreach (char c in Guid.NewGuid().ToString("N")) {
                sb.Append(rand.Next(0, 2) == 1 ? Char.ToUpper(c) : c);
            }

            return sb.ToString();
        }
    }

}