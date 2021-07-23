using DimensionClient.Common;

namespace DimensionClient.Models.ViewModels
{
    public class NoPasswordLoginViewModel : ModelBase
    {
        private string phoneNumber;
        private string code;
        private string getCode;

        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }
        public string Code
        {
            get => code;
            set
            {
                code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
        public string GetCode
        {
            get => getCode;
            set
            {
                if (value != ClassHelper.FindResource<string>("GetCode"))
                {
                    value = $"{value} 秒后重试";
                }
                getCode = value;
                OnPropertyChanged(nameof(GetCode));
            }
        }

        public override void InitializeVariable()
        {
            phoneNumber = string.Empty;
            code = string.Empty;
            GetCode = ClassHelper.FindResource<string>("GetCode");
        }
    }
}