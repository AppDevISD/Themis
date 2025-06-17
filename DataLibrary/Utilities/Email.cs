using DataLibrary;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.Routing;

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
            char[] separators = new[] { ';', ',' };

            try
            {
                MailMessage NewEmail = new MailMessage();
                NewEmail.From = new MailAddress("THΣMIS Application NoReply@cwlp.com");
                foreach (string item in emailList.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                {
                    NewEmail.To.Add(item.Trim());
                }
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

        public static Email GetEmailType(Dictionary<string, object> config)
        {
            Email newEmail = new Email();

            string formType;
            string href;
            string ordinanceNumInfo;
            string type = config["Type"].ToString().ToUpper();

            switch (type)
            {
                case "SUBMIT":
                    formType = "Ordinance Fact Sheet";
                    href = $"apptest/Themis/Ordinances?id={config["OrdinanceID"]}&v=view";

                    newEmail.EmailSubject = $"{formType} SUBMITTED";
                    newEmail.EmailTitle = $"{formType} SUBMITTED";
                    newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been SUBMITTED by <b>{config["Name"]}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {config["OrdinanceID"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {config["Department"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {config["Contact"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {config["Phone"]}</span></p><br /><p><span>Please click the button below to review the document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";
                    break;
                case "UPDATED":
                    formType = "Ordinance Fact Sheet";
                    href = $"apptest/Themis/Ordinances?id={config["OrdinanceID"]}&v=view";
                    ordinanceNumInfo = !config["OrdinanceNum"].ToString().IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {config["OrdinanceNum"]}</span></p>" : string.Empty;

                    newEmail.EmailSubject = $"{formType} UPDATED";
                    newEmail.EmailTitle = $"{formType} UPDATED";
                    newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been UPDATED by <b>{config["Name"]}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {config["OrdinanceID"]}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {config["Department"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {config["Contact"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {config["Phone"]}</span></p><p><span>Status: {config["Status"]}</span></p><br /><p><span>Please click the button below to review the document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";
                    break;

                case "SIGNATURE":
                    formType = "THΣMIS";
                    href = $"apptest/Themis/Ordinances?id={config["OrdinanceID"]}&v=edit&f={config["Target"]}";

                    newEmail.EmailSubject = $"{formType} Signature Requested";
                    newEmail.EmailTitle = $"{formType} Signature Requested";
                    newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>You are receiving this message because your signature is required in the role of <b>{config["Label"]}</b> for Ordinance ID #{config["OrdinanceID"]} on THΣMIS.</span></p><p><span>Please click the button below to review and sign the document</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #198754; border-radius: 5px; text-align: center;' valign='top' bgcolor='#198754' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #198754; border: solid 1px #198754; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #198754; '>Sign Ordinance</a></td></tr></table><br /><p><span>Thank you for your prompt attention to this matter.</span></p>";
                    break;
                case "DS_SIGNATURE":
                    formType = "THΣMIS";
                    href = $"apptest/Themis/Ordinances?id={config["OrdinanceID"]}&v=edit&f=directorSupervisorBtn";

                    newEmail.EmailSubject = $"{formType} Signature Requested";
                    newEmail.EmailTitle = $"{formType} Signature Requested";
                    newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>You are receiving this message because an Ordinance Fact Sheet has been SUBMITTED by <b>{config["Name"]}</b> and your signature is required in the role of <b>Director/Supervisor</b> for Ordinance ID #{config["OrdinanceID"]} on THΣMIS.</span></p><p><span>Please click the button below to review and sign the document</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #198754; border-radius: 5px; text-align: center;' valign='top' bgcolor='#198754' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #198754; border: solid 1px #198754; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #198754; '>Sign Ordinance</a></td></tr></table><br /><p><span>Thank you for your prompt attention to this matter.</span></p>";
                    break;

                case "PENDING":
                case "UNDER REVIEW":
                    formType = "Ordinance Fact Sheet";
                    href = $"apptest/Themis/Ordinances?id={config["OrdinanceID"]}&v=view";
                    ordinanceNumInfo = !config["OrdinanceNum"].ToString().IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {config["OrdinanceNum"]}</span></p>" : string.Empty;

                    newEmail.EmailSubject = $"{formType} {type}";
                    newEmail.EmailTitle = $"{formType} {type}";
                    newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been moved to <b>{type}</b> status.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {config["OrdinanceID"]}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {config["Department"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {config["Contact"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {config["Phone"]}</span></p><p><span>Status: {config["Status"]}</span></p><br /><p><span>Please click the button below to review the document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";
                    break;
                case "REJECTED":
                    formType = "Ordinance Fact Sheet";
                    href = $"apptest/Themis/Ordinances?id={config["OrdinanceID"]}&v=edit";
                    ordinanceNumInfo = !config["OrdinanceNum"].ToString().IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {config["OrdinanceNum"]}</span></p>" : string.Empty;

                    newEmail.EmailSubject = $"{formType} REJECTED";
                    newEmail.EmailTitle = $"{formType} REJECTED";
                    newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been REJECTED by <b>{config["Name"]}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {config["OrdinanceID"]}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p><span>Status: Rejected</span></p><br /><p style='margin: 0; line-height: 1.5;'><span><b>Rejection Reason:</b></span></p><p style='margin: 0; line-height: 1.5;'><span>{config["Reason"]}</span></p><p><span>Please click the button below to review the document and make changes if necessary:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table><br /><p><span>If you believe this is a mistake or have any questions please contact the rejector at <a href='mailto:{config["Email"]}'>{config["Email"]}</a></span></p>";
                    break;
                case "DELETED":
                    formType = "Ordinance Fact Sheet";
                    href = $"apptest/Themis/Ordinances?id={config["OrdinanceID"]}&v=view";
                    ordinanceNumInfo = !config["OrdinanceNum"].ToString().IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {config["OrdinanceNum"]}</span></p>" : string.Empty;

                    newEmail.EmailSubject = $"{formType} DELETED";
                    newEmail.EmailTitle = $"{formType} DELETED";
                    newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been DELETED by <b>{config["Name"]}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {config["OrdinanceID"]}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {config["Department"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {config["Contact"]}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {config["Phone"]}</span></p><p><span>Status: Deleted</span></p><br /><p><span>Please click the button below to view the deleted document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";
                    break;
            }

            return newEmail;
        }
    }
}
