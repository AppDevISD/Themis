using DataLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataLibrary
{
    public class Factory
    {
        public static Factory _Factory = null;
        private static SqlConnection _cn = null;
        public string errorMsg = "";

        public static Factory Instance
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new Factory();
                }
                return _Factory;
            }
        }

        private Factory()
        {
            _cn = new SqlConnection();
            _cn.Close();
        }

        public string GetUserTheme()
        {
            return Properties.Settings.Default.userTheme.ToString();
        }

        public void SetUserTheme(string theme)
        {
            Properties.Settings.Default["userTheme"] = theme;
        }
    }
}
