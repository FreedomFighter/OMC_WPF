using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_usercontrol_libs.src
{
    public class DataGridUserModel : ObservableCollection<UserModel>
    {
        public void CreateGenericSiteModelData(UserModel userModel)
        {
            Add(userModel);
        }
    }

    public class UserModel
    {
        public UserModel()
        {

        }

        public UserModel(string _username, string _password, string _mail)
        {
            username = _username;
            password = _password;
            mail = _mail;

            username1 = _username;
            password1 = _password;
            mail1 = _mail;

            username2 = _username;
            password2 = _password;
            mail2 = _mail;

            username3 = _username;
            password3 = _password;
            mail3 = _mail;

            username4 = _username;
            password4 = _password;
            mail4 = _mail;

            username5 = _username;
            password5 = _password;
            mail5 = _mail;

            username6 = _username;
            password6 = _password;
            mail6 = _mail;
        }

        public string username { get; set; }
        public string password { get; set; }
        public string mail { get; set; }

        public string username1 { get; set; }
        public string password1 { get; set; }
        public string mail1 { get; set; }

        public string username2 { get; set; }
        public string password2 { get; set; }
        public string mail2 { get; set; }

        public string username3 { get; set; }
        public string password3 { get; set; }
        public string mail3 { get; set; }

        public string username4 { get; set; }
        public string password4 { get; set; }
        public string mail4 { get; set; }

        public string username5 { get; set; }
        public string password5 { get; set; }
        public string mail5 { get; set; }

        public string username6 { get; set; }
        public string password6 { get; set; }
        public string mail6 { get; set; }
    }
}
