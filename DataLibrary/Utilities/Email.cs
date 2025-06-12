using DataLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataLibrary
{
    public class Email
    {
        public string EmailAddress { get; set; }
        public string EmailFrom { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTitle { get; set; }
        public string EmailText { get; set; }
        public string VerificationCode { get; set; }

        public string UserName { get; set; }
        public string ComputerName { get; set; }
        public string FullName { get; set; }

        public string errorMsg = "";

        private static Email _Email;

        public static Email Instance
        {
            get
            {
                if (_Email == null)
                {
                    _Email = new Email();
                }
                return _Email;
            }
        }

        public string AddEmailAddress(string emailList, string emailAddress)
        {
            string currentList = emailList;
            switch (currentList.Contains(emailAddress))
            {
                case true:
                    emailList = currentList;
                    break;

                case false:
                    if (emailList.Length > 0)
                    {
                        emailList = $"{currentList},{emailAddress}";
                    }
                    else
                    {
                        emailList = emailAddress;
                    }
                        break;
            }

            return emailList;
        }

        public string ResetEmailList(string emailList, string permanentEmail)
        {
            Properties.Settings.Default[emailList] = permanentEmail;
            return null;
        }

        public string SendEmail(Email pEmail, string emailList, bool useAppSettings = false)
        {
            string retVal = string.Empty;
            string emailAddress = useAppSettings ? Properties.Settings.Default[emailList].ToString() : emailList;

            try
            {
                MailAddress from = new MailAddress("THΣMIS Application NoReply@cwlp.com");
                MailAddress to = new MailAddress(emailAddress);
                MailMessage NewEmail = new MailMessage(from, to);
                NewEmail.IsBodyHtml = true;
                NewEmail.Body = pEmail.EmailText;
                NewEmail.Subject = pEmail.EmailSubject.ToString();
                SmtpClient client = new SmtpClient(Properties.Settings.Default["ExchangeIP"].ToString());
                client.Send(NewEmail);
                retVal = "success";
            }
            catch (FormatException)
            {
                retVal = "failed";
                errorMsg = "Invalid email address format.";
            }
            catch (Exception e)
            {
                retVal = "failed";
                errorMsg = e.Message;
            }
            return retVal;
        }
    }


}
