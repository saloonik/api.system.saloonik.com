using ServiceReference;
using System.Text.RegularExpressions;

namespace beautysalon.Logic.Services.Validators
{
    public static class PolishIDNumbersValidation
    {
        public static async Task<bool> IsValidNIPAsync(this string input, CancellationToken cancellationToken = default)
        {
            string cleanedInput = input.Trim();

            // If the input starts with a country code (e.g., "PL", "DE"), we validate the VAT number using VIES
            if (cleanedInput.Length > 2 && cleanedInput.StartsWith("PL", StringComparison.OrdinalIgnoreCase))
            {
                string cleanedNIP = new string(cleanedInput.Where(char.IsDigit).ToArray());

                if (cleanedNIP.Length != 10) return false;

                int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };

                int checksum = cleanedNIP
                    .Take(9)
                    .Select((digit, index) => (digit - '0') * weights[index])
                    .Sum();

                int controlDigit = checksum % 11;

                return controlDigit != 10 && controlDigit == (cleanedNIP[9] - '0');
            }

            // If no country code is provided, we assume it's a Polish NIP and validate
            if (cleanedInput.Length == 10)
            {
                string cleanedNIP = new string(cleanedInput.Where(char.IsDigit).ToArray());

                if (cleanedNIP.Length != 10) return false;

                int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };

                int checksum = cleanedNIP
                    .Take(9)
                    .Select((digit, index) => (digit - '0') * weights[index])
                    .Sum();

                int controlDigit = checksum % 11;

                return controlDigit != 10 && controlDigit == (cleanedNIP[9] - '0');
            }

            // If input doesn't match any of the above conditions, check if it's a VAT number for international validation
            return await IsValidVatWithVIESAsync(cleanedInput, cancellationToken);
        }


        public static async Task<bool> IsValidREGONAsync(this string input, CancellationToken cancellationToken = default)
        {
            string cleanedREGON = new string(input.Where(char.IsDigit).ToArray());

            if (cleanedREGON.Length != 9) return false;

            int[] weights = { 8, 9, 2, 3, 4, 5, 6, 7 };

            int checksum = cleanedREGON
                .Take(8)
                .Select((digit, index) => (digit - '0') * weights[index]) 
                .Sum();

            int controlDigit = checksum % 11;

            return controlDigit != 10 && controlDigit == (cleanedREGON[8] - '0');
        }


        public static async Task<bool> IsValidVatWithVIESAsync(string vatNumber, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(vatNumber) || vatNumber.Length < 3)
                return false;

            string countryCode = vatNumber.Substring(0, 2).ToUpperInvariant();
            string number = vatNumber.Substring(2);

            try
            {
                var client = new checkVatPortTypeClient(checkVatPortTypeClient.EndpointConfiguration.checkVatPort);
                var response = await client.checkVatAsync(new checkVatRequest
                {
                    countryCode = countryCode,
                    vatNumber = number
                });

                return response.valid;
            }
            catch
            {
                return false;
            }
        }


    }
}
