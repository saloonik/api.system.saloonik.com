
namespace beautysalon.Logic.Services.Validators.CompanyValidator
{
    public interface IValidateCompany
    {
        Task<bool> ValidateCompanyNIP (string nip);
    }
}