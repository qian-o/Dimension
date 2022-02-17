using DimensionClient.Common;

namespace DimensionClient.Models.ViewModels
{
    public class PasswordLoginViewModel : ModelBase
    {
        private string loginName;
        private bool? remembPassword;
        private List<LoginUserModel> loginUsers;

        public string LoginName
        {
            get => loginName;
            set
            {
                loginName = value;
                OnPropertyChanged(nameof(LoginName));
            }
        }
        public bool? RemembPassword
        {
            get => remembPassword;
            set
            {
                remembPassword = value;
                OnPropertyChanged(nameof(RemembPassword));
            }
        }

        public List<LoginUserModel> LoginUsers
        {
            get => loginUsers;
            set
            {
                loginUsers = value;
                OnPropertyChanged(nameof(LoginUsers));
            }
        }

        public override void InitializeVariable()
        {
            loginName = string.Empty;
            remembPassword = false;
            loginUsers = null;
        }
    }
}