using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace UnionCheckers
{
    public class Users: INotifyPropertyChanged
    {
        private int _id;
        public int Id 
        { 
            get { return _id; } 
            set { _id = value; }
           
        }
        
        private string _login, _password;
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private int? _rating;
        
        public int? Rating
        {
            get { return _rating; }
            set { _rating = value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string  propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Users()
        {
        }

        public Users(string login, string password, int rating)
        {
            this._login = login;
            this._password = password;
            this._rating = 0;
        }


    }
}
