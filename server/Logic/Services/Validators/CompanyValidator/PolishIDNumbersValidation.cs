using System.Threading;
using System.Threading.Tasks;

namespace beautysalon.Logic.Services.Validators
{
    public static class PolishIDNumbersValidation
    {
        public static async Task<bool> IsValidNIPAsync(this string input, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();


            int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };
            bool result = false;

            if (input.Length == 10)
            {
                int controlSum = CalculateControlSum(input, weights);
                int controlNum = controlSum % 11;
                if (controlNum == 10)
                {
                    controlNum = 0;
                }
                int lastDigit = int.Parse(input[input.Length - 1].ToString());
                result = controlNum == lastDigit;
            }

            return result;
        }

        private static int CalculateControlSum(string input, int[] weights, int offset = 0)
        {
            int controlSum = 0;

            for (int i = 0; i < input.Length - 1; i++)
            {
                controlSum += weights[i + offset] * int.Parse(input[i].ToString());
            }

            return controlSum;
        }
    }
}