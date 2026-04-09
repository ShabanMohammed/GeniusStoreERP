using System.ComponentModel.DataAnnotations;
using GeniusStoreERP.UI.Common;

namespace GeniusStoreERP.UI.Models
{
    public class GeneralSettingEditModel : BaseViewModel
    {
        private string _companyName = string.Empty;
        private string? _legalName;
        private string? _address;
        private string? _phone1;
        private string? _phone2;
        private string? _email;
        private string? _website;
        private string? _taxNumber;
        private byte[]? _logo;
        private decimal _taxPercentage;
        private string _currencySymbol = "£";

        [Required(ErrorMessage = "اسم الشركة مطلوب")]
        public string CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }

        public string? LegalName
        {
            get => _legalName;
            set => SetProperty(ref _legalName, value);
        }

        public string? Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string? Phone1
        {
            get => _phone1;
            set => SetProperty(ref _phone1, value);
        }

        public string? Phone2
        {
            get => _phone2;
            set => SetProperty(ref _phone2, value);
        }

        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string? Website
        {
            get => _website;
            set => SetProperty(ref _website, value);
        }

        public string? TaxNumber
        {
            get => _taxNumber;
            set => SetProperty(ref _taxNumber, value);
        }

        public byte[]? Logo
        {
            get => _logo;
            set => SetProperty(ref _logo, value);
        }

        public decimal TaxPercentage
        {
            get => _taxPercentage;
            set => SetProperty(ref _taxPercentage, value);
        }

        public string CurrencySymbol
        {
            get => _currencySymbol;
            set => SetProperty(ref _currencySymbol, value);
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(CompanyName);
        }
    }
}
