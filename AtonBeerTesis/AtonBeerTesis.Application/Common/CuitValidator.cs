using System.Text.RegularExpressions;

namespace AtonBeerTesis.Application.Common
{
    public static class CuitValidator
    {
        public static bool IsValid(string? cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit)) return false;

            var digits = Normalize(cuit);
            if (digits.Length != 11) return false;

            // Evita 00000000000, 11111111111, etc.
            if (digits.All(d => d == digits[0])) return false;

            int[] weights = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (digits[i] - '0') * weights[i];
            }

            int mod = sum % 11;
            int dv = 11 - mod;

            if (dv == 11) dv = 0;
            else if (dv == 10) dv = 9;

            return dv == (digits[10] - '0');
        }

        public static string Normalize(string cuit)
        {
            return Regex.Replace(cuit, @"\D", "");
        }
    }
}
