namespace beautysalon.Logic.Services.Validators.CompanyValidator
{
    public class ValidateCompany : IValidateCompany
    {
        public async Task<bool> ValidateCompanyNIP (string nip)
        {
            return PolishIDNumbersValidation.IsValidNIP(nip);
        }
    }
}
