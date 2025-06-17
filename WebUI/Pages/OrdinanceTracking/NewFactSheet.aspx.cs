using DataLibrary;
using DataLibrary.OrdinanceTracking;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class NewFactSheet : System.Web.UI.Page
    {
        // GLOBAL VARIABLES //
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();



        // PAGE LOADING //
        protected void Page_Load(object sender, EventArgs e)
        {
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("revenue");
                Session.Remove("expenditure");
                Session.Remove("ordDocs");
                Session.Remove("SigRequestEmails");
                GetAllDepartments();
                GetAllPurchaseMethods();
                SetStartupActives();
            }

            if (!Page.IsPostBack && Request.QueryString["id"] != null)
            {
                GetCopyOrdinance(Convert.ToInt32(Request.QueryString["id"].ToString()));
            }
            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideLoading", "hideLoadingModal();", true);
            }
            GetUploadedDocs();
        }



        // STARTUP DATA //
        protected void GetAllDepartments()
        {
            Dictionary<string, string> departments = DepartmentsList();
            foreach (var department in departments.Keys)
            {
                var value = departments[department];
                ListItem newItem = new ListItem(department, value);
                requestDepartment.Items.Add(newItem);
            }
        }
        protected void GetAllDivisions(DropDownList dd, string deptCode)
        {
            dd.Items.Clear();
            dd.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            List<Division> divisionList = GetDivisionsByDept(Convert.ToInt32(deptCode));
            foreach (Division item in divisionList)
            {
                ListItem newItem = new ListItem(item.DivisionName, item.DivisionCode.ToString());
                dd.Items.Add(newItem);
            }
        }
        protected void GetAllPurchaseMethods()
        {
            purchaseMethod.Items.Insert(0, new ListItem("Select Purchase Method...", ""));
            purchaseMethod.Items.Insert(1, new ListItem("Low Bid", "Low Bid"));
            purchaseMethod.Items.Insert(2, new ListItem("Low Bid Meeting Specs", "Low Bid Meeting Specs"));
            purchaseMethod.Items.Insert(3, new ListItem("Low Evaluated Bid", "Low Evaluated Bid"));
            purchaseMethod.Items.Insert(4, new ListItem("Other", "Other"));
            purchaseMethod.Items.Insert(5, new ListItem("Exception", "Exception"));
        }
        protected void SetStartupActives()
        {
            epJustificationGroup.Visible = false;
            epJustificationValid.Enabled = false;
            changeOrderNumber.Enabled = false;
            changeOrderNumberValid.Enabled = false;
            additionalAmount.Enabled = false;
            additionalAmountValid.Enabled = false;
            otherException.Enabled = false;
            otherExceptionValid.Enabled = false;

            requestDepartment.SelectedValue = userInfo.UserDepartment.DepartmentCode.ToString();
            requestContact.Text = $"{_user.FirstName} {_user.LastName}";
            requestEmail.Text = _user.Email.ToLower();
            requestPhone.Text = _user.Telephone;
            requestExt.Text = _user.IPPhone;

            requestDivision.Items.Clear();
            List<string> noDivisions = new List<string>()
            {
                "City Clerk",
                "Community Relations",
                "Convention & Visitor's Bureau",
                "Lincoln Library",
            };
            if (!requestDepartment.SelectedValue.IsNullOrWhiteSpace() && !noDivisions.Any(i => requestDepartment.SelectedItem.Text.Equals(i)))
            {
                requestDivision.Enabled = true;
                requestDivision.Attributes.Add("data-required", "true");
                requestDivisionValid.Enabled = true;
                GetAllDivisions(requestDivision, requestDepartment.SelectedValue);
                requestDivision.SelectedValue = userInfo.UserDivision.DivisionCode.ToString();
            }
            else
            {
                requestDivision.Enabled = false;
                requestDivision.Attributes.Add("data-required", "false");
                requestDivisionValid.Enabled = false;
                requestDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            }
        }
        protected void GetUploadedDocs()
        {
            if (Session["ordDocs"] != null)
            {
                List<OrdinanceDocument> ordDocList = Session["ordDocs"] as List<OrdinanceDocument>;
                rpSupportingDocumentation.DataSource = ordDocList;
                rpSupportingDocumentation.DataBind();
            }
        }
        protected void GetCopyOrdinance(int ordID)
        {
            Session.Remove("revenue");
            Session.Remove("expenditure");
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");


            requestDepartment.SelectedValue = DepartmentsList()[ord.RequestDepartment];

            switch (ord.EmergencyPassage)
            {
                case true:
                    epYes.Checked = true;
                    epNo.Checked = false;
                    epJustificationGroup.Visible = true;
                    epJustification.Attributes.Add("data-required", "true");
                    break;
                case false:
                    epYes.Checked = false;
                    epNo.Checked = true;
                    epJustificationGroup.Visible = false;
                    epJustification.Attributes.Remove("data-required");
                    break;
            }
            epJustification.Text = ord.EmergencyPassageReason;

            fiscalImpact.Text = NotApplicable(ord.OrdinanceFiscalImpact.ToString());
            suggestedTitle.Text = ord.OrdinanceTitle;

            vendorName.Text = ord.ContractVendorName;
            vendorNumber.Text = ord.ContractVendorNumber;
            contractStartDate.Text = ord.ContractStartDate;
            contractEndDate.Text = ord.ContractEndDate;
            contractTerm.Value = ord.ContractTerm;
            contractAmount.Text = NotApplicable(ord.ContractAmount.ToString());

            switch (ord.ScopeChange)
            {
                case true:
                    scYes.Checked = true;
                    scNo.Checked = false;
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;
                    changeOrderNumber.Attributes.Add("data-required", "true");
                    additionalAmount.Attributes.Add("data-required", "true");
                    break;
                case false:
                    scYes.Checked = false;
                    scNo.Checked = true;
                    changeOrderNumber.Enabled = false;
                    additionalAmount.Enabled = false;
                    changeOrderNumber.Attributes.Remove("data-required");
                    additionalAmount.Attributes.Remove("data-required");
                    break;
            }
            changeOrderNumber.Text = ord.ChangeOrderNumber;
            additionalAmount.Text = NotApplicable(ord.AdditionalAmount.ToString());


            purchaseMethod.SelectedValue = ord.ContractMethod;
            switch (purchaseMethod.SelectedItem.Value)
            {
                default:
                    otherException.Enabled = false;
                    otherException.Text = string.Empty;
                    otherException.Attributes.Remove("data-required");
                    break;
                case "Other":
                case "Exception":
                    otherException.Enabled = true;
                    otherException.Attributes.Add("data-required", "true");
                    break;
            }
            otherException.Text = ord.OtherException;

            List<string> prvOrdNums = ord.PreviousOrdinanceNumbers.Trim().Split(',').Where(i => !i.IsNullOrWhiteSpace()).ToList();

            if (!ord.OrdinanceNumber.IsNullOrWhiteSpace())
            {
                prvOrdNums.Add(ord.OrdinanceNumber);
            }
            prevOrdinanceNums.Text = string.Join(", ", prvOrdNums);
            ord.PreviousOrdinanceNumbers = prevOrdinanceNums.Text;



            codeProvision.Text = ord.CodeProvision;

            switch (ord.PAApprovalRequired)
            {
                case true:
                    paApprovalRequiredYes.Checked = true;
                    paApprovalRequiredNo.Checked = false;
                    break;
                case false:
                    paApprovalRequiredYes.Checked = false;
                    paApprovalRequiredNo.Checked = true;
                    break;
            }
            switch (ord.PAApprovalIncluded)
            {
                case true:
                    paApprovalAttachedYes.Checked = true;
                    paApprovalAttachedNo.Checked = false;
                    break;
                case false:
                    paApprovalAttachedYes.Checked = false;
                    paApprovalAttachedNo.Checked = true;
                    break;
            }

            List<OrdinanceAccounting> ordAcc = Factory.Instance.GetAllLookup<OrdinanceAccounting>(ordID, "sp_GetOrdinanceAccountingByOrdinanceID", "OrdinanceID");
            List<OrdinanceAccounting> revItems = new List<OrdinanceAccounting>();
            List<OrdinanceAccounting> expItems = new List<OrdinanceAccounting>();
            if (ordAcc.Count > 0)
            {
                foreach (OrdinanceAccounting item in ordAcc)
                {
                    switch (item.AccountingDesc)
                    {
                        case "revenue":
                            revItems.Add(item);
                            break;
                        case "expenditure":
                            expItems.Add(item);
                            break;
                    }
                }

                if (revItems.Count > 0)
                {
                    Session["revenue"] = revItems;
                    rpRevenueTable.DataSource = revItems;
                    rpRevenueTable.DataBind();
                }
                else
                {
                    Session.Remove("revenue");
                    rpRevenueTable.DataSource = null;
                    rpRevenueTable.DataBind();
                }
                if (expItems.Count > 0)
                {
                    Session["expenditure"] = expItems;
                    rpExpenditureTable.DataSource = expItems;
                    rpExpenditureTable.DataBind();
                }
                else
                {
                    Session.Remove("expenditure");
                    rpExpenditureTable.DataSource = null;
                    rpExpenditureTable.DataBind();
                }
            }
            else
            {
                Session.Remove("revenue");
                Session.Remove("expenditure");
                rpRevenueTable.DataSource = null;
                rpExpenditureTable.DataSource = null;
                rpRevenueTable.DataBind();
                rpExpenditureTable.DataBind();
            }

            staffAnalysis.Text = ord.OrdinanceAnalysis;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "CurrencyFormatting", "CurrencyFormatting();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "FormatForms", "FormatForms();", true);
        }



        // CONTROL CHANGES //
        protected void requestDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            requestDivision.Items.Clear();
            List<string> noDivisions = new List<string>()
            {
                "City Clerk",
                "Community Relations",
                "Convention & Visitor's Bureau",
                "Lincoln Library",
            };

            if (!requestDepartment.SelectedValue.IsNullOrWhiteSpace() && !noDivisions.Any(i => requestDepartment.SelectedItem.Text.Equals(i)))
            {
                requestDivision.Enabled = true;
                requestDivision.Attributes.Add("data-required", "true");
                requestDivisionValid.Enabled = true;
                GetAllDivisions(requestDivision, requestDepartment.SelectedValue);
            }
            else
            {
                requestDivision.Enabled = false;
                requestDivision.Attributes.Add("data-required", "false");
                requestDivisionValid.Enabled = false;
                requestDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            }
        }
        protected void PurchaseMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (purchaseMethod.SelectedItem.Value)
            {
                default:
                    otherException.Enabled = false;
                    otherExceptionValid.Enabled = false;
                    otherException.Text = string.Empty;
                    otherException.Attributes.Remove("data-required");
                    break;
                case "Other":
                case "Exception":
                    otherException.Enabled = true;
                    otherExceptionValid.Enabled = true;
                    otherException.Attributes.Add("data-required", "true");
                    break;
            }
        }
        protected void EPCheckedChanged(object sender, EventArgs e)
        {
            switch (epYes.Checked)
            {
                case true:
                    epJustificationGroup.Visible = true;
                    epJustification.Attributes.Add("data-required", "true");
                    epJustificationValid.Enabled = true;
                    break;

                case false:
                    epJustificationGroup.Visible = false;
                    epJustification.Attributes.Remove("data-required");
                    epJustificationValid.Enabled = false;
                    break;
            }
        }
        protected void SCCheckedChanged(object sender, EventArgs e)
        {
            switch (scYes.Checked)
            {
                case true:
                    changeOrderNumber.Enabled = true;
                    changeOrderNumberValid.Enabled = true;
                    additionalAmount.Enabled = true;
                    additionalAmountValid.Enabled = true;
                    
                    changeOrderNumber.Attributes.Add("data-required", "true");
                    changeOrderNumber.Attributes.Add("placeholder", "0123456789");
                    additionalAmount.Attributes.Add("data-required", "true");
                    additionalAmount.Attributes.Add("placeholder", "$0.00");
                    break;

                case false:
                    changeOrderNumber.Enabled = false;
                    changeOrderNumberValid.Enabled = false;
                    additionalAmount.Enabled = false;
                    additionalAmountValid.Enabled = false;

                    changeOrderNumber.Attributes.Remove("data-required");
                    changeOrderNumber.Attributes.Remove("placeholder");
                    additionalAmount.Attributes.Remove("data-required");
                    additionalAmount.Attributes.Remove("placeholder");
                    break;
            }
        }
        protected void UploadDocBtn_Click(object sender, EventArgs e)
        {
            List<OrdinanceDocument> ordDocList = (Session["ordDocs"] != null) ? Session["ordDocs"] as List<OrdinanceDocument> : new List<OrdinanceDocument>();

            for (int i = 0; i < supportingDocumentation.PostedFiles.Count; i++)
            {
                OrdinanceDocument ordDoc = new OrdinanceDocument();
                ordDoc.DocumentName = supportingDocumentation.PostedFiles[i].FileName;
                Stream stream = supportingDocumentation.PostedFiles[i].InputStream;
                using (var fileBytes = new BinaryReader(stream))
                {
                    ordDoc.DocumentData = fileBytes.ReadBytes((int)stream.Length);
                }
                ordDoc.LastUpdateBy = _user.Login;
                ordDoc.LastUpdateDate = DateTime.Now;
                ordDoc.EffectiveDate = DateTime.Now;
                ordDoc.ExpirationDate = DateTime.MaxValue;
                ordDocList.Add(ordDoc);
            }
            Session["ordDocs"] = ordDocList;
            rpSupportingDocumentation.DataSource = ordDocList;
            rpSupportingDocumentation.DataBind();
        }
        protected void AddRequestEmailAddress_Click(object sender, EventArgs e)
        {
            SignatureRequest sigRequests = new SignatureRequest();
            PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals("DirectorSupervisor"));
            List<string> emails = new List<string>();
            if (Session["SigRequestEmails"] != null)
            {
                sigRequests = Session["SigRequestEmails"] as SignatureRequest;
                emails = sigType.GetValue(sigRequests).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
            }

            string[] newEmailAddresses = signatureEmailAddress.Text.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            foreach (string item in newEmailAddresses)
            {
                emails.Add(item);
            }
            sigType.SetValue(sigRequests, string.Join(";", emails.OrderBy(i => i)));
            if (emails.Count > 0)
            {
                directorSupervisorEmailAddresses.Text = string.Join(";", emails.OrderBy(i => i));
                emailListDiv.Visible = true;
                rpEmailList.DataSource = emails.OrderBy(i => i);
                rpEmailList.DataBind();
            }
            else
            {
                directorSupervisorEmailAddresses.Text = string.Empty;
                emailListDiv.Visible = false;
                rpEmailList.DataSource = null;
                rpEmailList.DataBind();
            }
            signatureEmailAddress.Text = string.Empty;

            Session["SigRequestEmails"] = sigRequests;
        }



        // REPEATER COMMANDS //
        protected void rpAccountingTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string tableDesc = e.CommandArgument.ToString();
            List<Accounting> prvList = new List<Accounting>();
            List<Accounting> accountingList = new List<Accounting>();
            switch (e.CommandName)
            {
                case "delete":

                    switch (tableDesc)
                    {
                        case "revenue":
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                Accounting accountingItem = new Accounting();
                                var revItem = rpRevenueTable.Items[i];
                                TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                                TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                                TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                                TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                                TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                                TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = revFundCode.Text;
                                accountingItem.DepartmentCode = revAgencyCode.Text;
                                accountingItem.UnitCode = revOrgCode.Text;
                                accountingItem.ActivityCode = revActivityCode.Text;
                                accountingItem.ObjectCode = revObjectCode.Text;
                                accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField revHdnIndex = (HiddenField)e.Item.FindControl("hdnRevIndex");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }
                            accountingList.RemoveAt(Convert.ToInt32(revHdnIndex.Value));
                            Session[tableDesc] = accountingList;
                            rpRevenueTable.DataSource = accountingList;
                            rpRevenueTable.DataBind();
                            break;
                        case "expenditure":
                            for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                            {
                                Accounting accountingItem = new Accounting();
                                var expItem = rpExpenditureTable.Items[i];
                                TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                                TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                                TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                                TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                                TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                                TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = expFundCode.Text;
                                accountingItem.DepartmentCode = expAgencyCode.Text;
                                accountingItem.UnitCode = expOrgCode.Text;
                                accountingItem.ActivityCode = expActivityCode.Text;
                                accountingItem.ObjectCode = expObjectCode.Text;
                                accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField expHdnIndex = (HiddenField)e.Item.FindControl("hdnExpIndex");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }
                            accountingList.RemoveAt(Convert.ToInt32(expHdnIndex.Value));
                            Session[tableDesc] = accountingList;
                            rpExpenditureTable.DataSource = accountingList;
                            rpExpenditureTable.DataBind();
                            break;
                    }
                    break;
            }
        }
        protected void rpRevExpTable_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            Repeater rpTable = (Repeater)sender;
            string rpType = string.Empty;
            switch (rpTable.ClientID)
            {
                case "rpRevenueTable":
                    rpType = "revenue";
                    break;
                case "rpExpenditureTable":
                    rpType = "expenditure";
                    break;
            }
            RepeaterItem rpItem = (RepeaterItem)e.Item;
            List<TextBox> textBoxes = new List<TextBox>()
            {
                (TextBox)e.Item.FindControl($"{rpType}FundCode"),
                (TextBox)e.Item.FindControl($"{rpType}AgencyCode"),
                (TextBox)e.Item.FindControl($"{rpType}OrgCode"),
                (TextBox)e.Item.FindControl($"{rpType}ActivityCode"),
                (TextBox)e.Item.FindControl($"{rpType}ObjectCode")
            };

            foreach (TextBox box in textBoxes)
            {
                box.Attributes.Add("data-validate", $"{box.ID}r{rpItem.ItemIndex}");
                RequiredFieldValidator rfv = new RequiredFieldValidator()
                {
                    ID = $"{box.ID}Validr{rpItem.ItemIndex}",
                    ControlToValidate = box.ID,
                    ValidationGroup = "factSheetMain",
                    SetFocusOnError = false,
                    Display = ValidatorDisplay.None
                };
                rfv.Attributes.Add("data-table-validator", "true");
                box.Parent.Controls.Add(rfv);
            }
        }
        protected void rpSupportingDocumentation_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            //HiddenField hdnDocID = (HiddenField)e.Item.FindControl("hdnDocID");
            HiddenField hdnDocIndex = (HiddenField)e.Item.FindControl("hdnDocIndex");
            List<OrdinanceDocument> ordDocList = Session["ordDocs"] as List<OrdinanceDocument>;
            OrdinanceDocument ordDocItem = ordDocList[Convert.ToInt32(hdnDocIndex.Value)];

            switch (e.CommandName)
            {
                case "delete":
                    List<OrdinanceDocument> removeDocs = new List<OrdinanceDocument>();
                    if (Session["RemoveDocs"] != null)
                    {
                        removeDocs = Session["RemoveDocs"] as List<OrdinanceDocument>;
                    }
                    removeDocs.Add(ordDocItem);
                    Session["RemoveDocs"] = removeDocs;
                    ordDocList.Remove(ordDocItem);
                    Session["ordDocs"] = ordDocList;
                    rpSupportingDocumentation.DataSource = ordDocList;
                    rpSupportingDocumentation.DataBind();
                    break;
            }
        }
        protected void rpEmailList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    SignatureRequest sigRequests = Session["SigRequestEmails"] as SignatureRequest;
                    PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals("DirectorSupervisor"));

                    List<string> emails = sigType.GetValue(sigRequests).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
                    emails.Remove(e.CommandArgument.ToString());
                    sigType.SetValue(sigRequests, string.Join(";", emails.OrderBy(i => i)));
                    if (emails.Count > 0)
                    {
                        directorSupervisorEmailAddresses.Text = sigRequests.DirectorSupervisor;
                        rpEmailList.DataSource = emails.OrderBy(i => i);
                        rpEmailList.DataBind();
                    }
                    else
                    {
                        directorSupervisorEmailAddresses.Text = string.Empty;
                        rpEmailList.DataSource = null;
                        rpEmailList.DataBind();
                    }

                    int updateSigRequest = Factory.Instance.Update(sigRequests, "sp_UpdateOrdinance_SignatureRequest");
                    if (updateSigRequest > 0)
                    {
                        signatureEmailAddress.Text = string.Empty;
                    }
                    break;
            }
        }
        protected void rpEmailList_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            LinkButton btn = (LinkButton)e.Item.FindControl("removeBtn");
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn);
        }



        // ACCOUNTING TABLES //
        protected void NewAccountingRow(string tableDesc)
        {
            List<OrdinanceAccounting> prvList = new List<OrdinanceAccounting>();
            List<OrdinanceAccounting> accountingList = new List<OrdinanceAccounting>();
            OrdinanceAccounting newAccountingItem = new OrdinanceAccounting();
            newAccountingItem.Amount = CurrencyToDecimal("-1");

            switch (tableDesc)
            {
                case "revenue":
                    if (Session["revenue"] != null)
                    {
                        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                        {          
                            
                            OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);

                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = prvList;
                    }
                    accountingList.Add(newAccountingItem);
                    Session[tableDesc] = accountingList;
                    rpRevenueTable.DataSource = accountingList;
                    rpRevenueTable.DataBind();
                    break;
                case "expenditure":
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                        {
                            OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = prvList;
                    }
                    accountingList.Add(newAccountingItem);
                    Session[tableDesc] = accountingList;
                    rpExpenditureTable.DataSource = accountingList;
                    rpExpenditureTable.DataBind();
                    break;
            }
        }
        protected void newAccountingRow_ServerClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            NewAccountingRow(button.CommandName);
        }
        protected OrdinanceAccounting GetAccountingItem(string tableDesc, int itemIndex)
        {
            OrdinanceAccounting accountingItem = new OrdinanceAccounting();
            switch (tableDesc)
            {
                case "revenue":
                    var revItem = rpRevenueTable.Items[itemIndex];
                    TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                    TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                    TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                    TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                    TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                    TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                    accountingItem.AccountingDesc = tableDesc;
                    accountingItem.FundCode = revFundCode.Text.ToUpper();
                    accountingItem.DepartmentCode = revAgencyCode.Text.ToUpper();
                    accountingItem.UnitCode = revOrgCode.Text.ToUpper();
                    accountingItem.ActivityCode = revActivityCode.Text.ToUpper();
                    accountingItem.ObjectCode = revObjectCode.Text.ToUpper();
                    accountingItem.LastUpdateBy = _user.Login;
                    accountingItem.LastUpdateDate = DateTime.Now;
                    accountingItem.EffectiveDate = DateTime.Now;
                    accountingItem.ExpirationDate = DateTime.MaxValue;
                    accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                    break;
                case "expenditure":
                    var expItem = rpExpenditureTable.Items[itemIndex];
                    TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                    TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                    TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                    TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                    TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                    TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                    accountingItem.AccountingDesc = tableDesc;
                    accountingItem.FundCode = expFundCode.Text.ToUpper();
                    accountingItem.DepartmentCode = expAgencyCode.Text.ToUpper();
                    accountingItem.UnitCode = expOrgCode.Text.ToUpper();
                    accountingItem.ActivityCode = expActivityCode.Text.ToUpper();
                    accountingItem.ObjectCode = expObjectCode.Text.ToUpper();
                    accountingItem.LastUpdateBy = _user.Login;
                    accountingItem.LastUpdateDate = DateTime.Now;
                    accountingItem.EffectiveDate = DateTime.Now;
                    accountingItem.ExpirationDate = DateTime.MaxValue;
                    accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                    break;
            }
            return accountingItem;
        }



        // SUBMITS //
        protected void SubmitForm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Button btn = (Button)sender;
                Ordinance ordinance = new Ordinance();

                string xString = !requestExt.Text.Contains("x") ? "x" : string.Empty;

                ordinance.OrdinanceNumber = string.Empty;
                ordinance.AgendaNumber = string.Empty;
                ordinance.RequestDepartment = requestDepartment.SelectedItem.Text;
                ordinance.RequestDivision = !requestDivision.SelectedValue.IsNullOrWhiteSpace() ? requestDivision.SelectedItem.Text : requestDepartment.SelectedItem.Text;
                ordinance.RequestContact = requestContact.Text;
                ordinance.RequestPhone = $"{requestPhone.Text}{xString}{requestExt.Text}";
                ordinance.RequestEmail = requestEmail.Text;
                ordinance.FirstReadDate = Convert.ToDateTime(firstReadDate.Text);
                ordinance.EmergencyPassage = epYes.Checked;
                ordinance.EmergencyPassageReason = epJustification.Text ?? string.Empty;
                ordinance.OrdinanceFiscalImpact = CurrencyToDecimal(fiscalImpact.Text);
                ordinance.OrdinanceTitle = suggestedTitle.Text;
                ordinance.ContractVendorName = vendorName.Text;
                ordinance.ContractVendorNumber = vendorNumber.Text;
                ordinance.ContractStartDate = contractStartDate.Text;
                ordinance.ContractEndDate = contractEndDate.Text;
                ordinance.ContractTerm = contractTerm.Value;
                ordinance.ContractAmount = CurrencyToDecimal(contractAmount.Text);
                ordinance.ScopeChange = scYes.Checked;
                ordinance.ChangeOrderNumber = changeOrderNumber.Text ?? string.Empty;
                ordinance.AdditionalAmount = CurrencyToDecimal(additionalAmount.Text);
                ordinance.ContractMethod = purchaseMethod.SelectedValue;
                ordinance.OtherException = otherException.Text ?? string.Empty;
                ordinance.PreviousOrdinanceNumbers = prevOrdinanceNums.Text;
                ordinance.CodeProvision = codeProvision.Text;
                ordinance.PAApprovalRequired = paApprovalRequiredYes.Checked;
                ordinance.PAApprovalIncluded = paApprovalAttachedYes.Checked;
                ordinance.OrdinanceAnalysis = staffAnalysis.Text;
                ordinance.LastUpdateBy = _user.Login;
                ordinance.LastUpdateDate = DateTime.Now;
                ordinance.EffectiveDate = DateTime.Now;
                ordinance.ExpirationDate = DateTime.MaxValue;

                int retVal = Factory.Instance.Insert(ordinance, "sp_InsertOrdinance", Skips("ordInsert"));
                //int retVal = 0;
                if (retVal > 0)
                {
                    bool revExpTables = false;
                    bool documentation = false;
                    bool uploadedDocumentation = false;
                    List<OrdinanceDocument> ordDocs = new List<OrdinanceDocument>();
                    bool finishSubmit = true;
                    if (rpRevenueTable.Items.Count > 0 || rpExpenditureTable.Items.Count > 0)
                    {
                        revExpTables = true;
                    }
                    if (supportingDocumentation.HasFiles)
                    {
                        documentation = true;
                    }
                    if (Session["ordDocs"] != null)
                    {
                        uploadedDocumentation = true;
                        ordDocs = Session["ordDocs"] as List<OrdinanceDocument>;
                    }

                    switch (revExpTables)
                    {
                        case true:
                            bool revSubmit = false;
                            bool expSubmit = false;
                            if (rpRevenueTable.Items.Count > 0)
                            {
                                for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                                {
                                    OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
                                    accountingItem.OrdinanceID = retVal;
                                    int ret = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
                                    //int ret = 1;
                                    if (ret > 0)
                                    {
                                        revSubmit = true;
                                    }
                                    else
                                    {
                                        revSubmit = false;
                                    }
                                }
                            }
                            else
                            {
                                revSubmit = true;
                            }

                            if (rpExpenditureTable.Items.Count > 0)
                            {
                                for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                                {
                                    OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", i);
                                    accountingItem.OrdinanceID = retVal;
                                    int ret = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
                                    //int ret = 1;
                                    if (ret > 0)
                                    {
                                        expSubmit = true;
                                    }
                                    else
                                    {
                                        expSubmit = false;
                                    }
                                }
                            }
                            else
                            {
                                expSubmit = true;
                            }

                            if (!revSubmit || !expSubmit)
                            {
                                finishSubmit = false;
                            }
                            break;
                    }

                    switch (uploadedDocumentation)
                    {
                        case true:
                            foreach (OrdinanceDocument ordDoc in ordDocs)
                            {
                                ordDoc.OrdinanceID = retVal;
                                int ret = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                                //int ret = 1;
                                if (ret < 1)
                                {
                                    finishSubmit = false;
                                }
                            }

                            break;
                    }

                    switch (documentation)
                    {
                        case true:

                            for (int i = 0; i < supportingDocumentation.PostedFiles.Count; i++)
                            {
                                OrdinanceDocument ordDoc = new OrdinanceDocument();
                                ordDoc.OrdinanceID = retVal;
                                ordDoc.DocumentName = supportingDocumentation.PostedFiles[i].FileName;
                                Stream stream = supportingDocumentation.PostedFiles[i].InputStream;
                                using (var fileBytes = new BinaryReader(stream))
                                {
                                    ordDoc.DocumentData = fileBytes.ReadBytes((int)stream.Length);
                                }
                                ordDoc.LastUpdateBy = _user.Login;
                                ordDoc.LastUpdateDate = DateTime.Now;
                                ordDoc.EffectiveDate = DateTime.Now;
                                ordDoc.ExpirationDate = DateTime.MaxValue;
                                int ret = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                                //int ret = 1;
                                if (ret < 1)
                                {
                                    finishSubmit = false;
                                }
                            }

                            break;
                    }

                    OrdinanceStatus ordStatus = new OrdinanceStatus();
                    ordStatus.OrdinanceID = retVal;
                    switch (btn.CommandName)
                    {
                        case "submit":
                            ordStatus.StatusID = 1;
                            break;
                        case "save":
                            ordStatus.StatusID = 9;
                            break;
                    }
                    ordStatus.LastUpdateBy = _user.Login;
                    ordStatus.LastUpdateDate = DateTime.Now;
                    ordStatus.EffectiveDate = DateTime.Now;
                    ordStatus.ExpirationDate = DateTime.MaxValue;
                    int statusRet = Factory.Instance.Insert(ordStatus, "sp_InsertOrdinance_Status", Skips("ordStatusInsert"));

                    if (statusRet < 1)
                    {
                        finishSubmit = false;
                    }

                    int businessID = new int();
                    switch (requestDepartment.SelectedItem.Value)
                    {
                        default:
                            businessID = 11;
                            break;
                        case "Public Utilities":
                            businessID = 12;
                            break;
                    }
                    SignatureRequest signatureRequest = new SignatureRequest()
                    {
                        OrdinanceID = retVal,
                        FundsCheckBy = Factory.Instance.GetByID<DefaultEmails>(8, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress,
                        DirectorSupervisor = string.Empty,
                        CityPurchasingAgent = Factory.Instance.GetByID<DefaultEmails>(9, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress,
                        OBMDirector = Factory.Instance.GetByID<DefaultEmails>(10, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress,
                        Budget = Factory.Instance.GetByID<DefaultEmails>(businessID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress,
                        Mayor = Factory.Instance.GetByID<DefaultEmails>(13, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress,
                        CCDirector = Factory.Instance.GetByID<DefaultEmails>(14, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress,
                        LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                        LastUpdateDate = DateTime.Now,
                        EffectiveDate = DateTime.Now,
                        ExpirationDate = DateTime.MaxValue
                    };


                    string sigRequests = string.Empty;
                    if (Session["SigRequestEmails"] != null)
                    {
                        SignatureRequest directorSupervisorSigRequest = Session["SigRequestEmails"] as SignatureRequest;
                        signatureRequest.DirectorSupervisor = directorSupervisorSigRequest.DirectorSupervisor;
                    }

                    

                    string departmentName = requestDepartment.SelectedItem.Text;
                    string divisionName = !requestDivision.SelectedIndex.Equals(0) ? requestDivision.SelectedItem.Text : "None";
                    int defaultDeptEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, "None").DefaultEmailsID;
                    int defaultDeptDivEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, divisionName).DefaultEmailsID;
                    string departmentDefaultList = Factory.Instance.GetByID<DefaultEmails>(defaultDeptEmailID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress;
                    string deptDivDefaultList = string.Empty;
                    if (!defaultDeptDivEmailID.Equals(defaultDeptEmailID))
                    {
                       deptDivDefaultList = Factory.Instance.GetByID<DefaultEmails>(defaultDeptDivEmailID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress;
                    }
                    if (btn.CommandName.Equals("submit"))
                    {
                        if (signatureRequest.DirectorSupervisor.Length > 0 && departmentDefaultList.Length > 0)
                        {
                            signatureRequest.DirectorSupervisor += $";{departmentDefaultList}";
                        }

                        if (signatureRequest.DirectorSupervisor.Length > 0 && deptDivDefaultList.Length > 0)
                        {
                            signatureRequest.DirectorSupervisor += $";{deptDivDefaultList}";
                        }
                    }

                    int signatureRequestRet = Factory.Instance.Insert(signatureRequest, "sp_InsertOrdinance_SignatureRequest", Skips("ordSignatureRequestInsert"));



                    if (signatureRequestRet < 1)
                    {
                        finishSubmit = false;
                    }

                    OrdinanceAudit ordAudit = new OrdinanceAudit()
                    {
                        OrdinanceID = Convert.ToInt32(retVal),
                        UpdateType = "CREATED",
                        LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                        LastUpdateDate = DateTime.Now,
                    };
                    int ordAuditRet = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                    if (ordAuditRet < 1)
                    {
                        finishSubmit = false;
                    }


                    Email sigRequestEmail = Email.GetEmailType(new Dictionary<string, object>()
                    {
                        { "Type", "ds_signature" },
                        { "OrdinanceID", retVal },
                        { "Name", $"{_user.FirstName} {_user.LastName}" }
                    });
                    Email submittedEmail = Email.GetEmailType(new Dictionary<string, object>()
                    {
                        { "Type", "submit" },
                        { "OrdinanceID", retVal },
                        { "Name", $"{_user.FirstName} {_user.LastName}" },
                        { "Department", requestDepartment.SelectedItem.Text },
                        { "Contact", requestContact.Text },
                        { "Phone", ordinance.RequestPhone }
                    });

                    switch (btn.CommandName)
                    {
                        case "submit":
                            switch (finishSubmit)
                            {
                                case true:
                                    Session["SubmitStatus"] = "success";
                                    Session["ToastColor"] = "text-bg-success";
                                    Session["ToastMessage"] = "Form Submitted!";
                                    Email.Instance.SendEmail(sigRequestEmail, directorSupervisorEmailAddresses.Text);
                                    Email.Instance.SendEmail(submittedEmail, _user.Email);
                                    Response.Redirect($"./Ordinances?id={retVal}&v=view", false);
                                    break;
                                case false:
                                    Session["SubmitStatus"] = "error";
                                    Session["ToastColor"] = "text-bg-danger";
                                    Session["ToastMessage"] = "Something went wrong while submitting!";
                                    break;
                            }
                            break;
                        case "save":
                            switch (finishSubmit)
                            {
                                case true:
                                    Session["SubmitStatus"] = "success";
                                    Session["ToastColor"] = "text-bg-success";
                                    Session["ToastMessage"] = "Form Saved!";
                                    Response.Redirect("./FactSheetDrafts", false);
                                    break;
                                case false:
                                    Session["SubmitStatus"] = "error";
                                    Session["ToastColor"] = "text-bg-danger";
                                    Session["ToastMessage"] = "Something went wrong while saving!";
                                    break;
                            }
                            break;
                    }

                }
                else
                {
                    Session["SubmitStatus"] = "error";
                    Session["ToastColor"] = "text-bg-danger";
                    Session["ToastMessage"] = "Something went wrong while submitting!";
                }
            }
        }
        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("./FactSheetDrafts");
        }
    }
}