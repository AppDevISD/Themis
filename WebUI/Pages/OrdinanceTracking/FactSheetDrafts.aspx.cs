using DataLibrary;
using DataLibrary.OrdinanceTracking;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static DataLibrary.TablePagination;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class FactSheetDrafts : System.Web.UI.Page
    {
        // GLOBAL VARIABLES //
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();
        public string deptDivColumnType = "RequestDepartment";
        public readonly List<string> lockedStatus = new List<string>()
        {
            "Pending",
            "Under Review",
            "Being Held",
            "Drafted",
            "Approved",
            "Deleted"
        };



        // PAGE LOADING //
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;

            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("ord_list");
                Session.Remove("revenue");
                Session.Remove("expenditure");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session.Remove("RemoveDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "LastUpdateDate";
                Session["curDir"] = "desc";
                Session["DeptDivColumn"] = "department";
                GetAllDepartments();
                GetAllPurchaseMethods();
                SetStartupActives();
                Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
                SetPagination(rpDraftsTable, pageBtns, pnlPagingP, pnlFooter, lblCurrentPageBottomSearchP, 10);
                Session["ViewState"] = ViewState;
                GetStartupData();
            }
            if (Page.IsPostBack && Page.Request.Params.Get("__EVENTTARGET").Contains("adminSwitch"))
            {
                userInfo = ((SiteMaster)Page.Master).UserView();
                Session.Remove("revenue");
                Session.Remove("expenditure");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "EffectiveDate";
                Session["curDir"] = "desc";
                SetStartupActives();
                Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
                SetPagination(rpDraftsTable, pageBtns, pnlPagingP, pnlFooter, lblCurrentPageBottomSearchP, 10);
                Session["ViewState"] = ViewState;
                GetStartupData();
            }

            foreach (RepeaterItem item in rpDraftsTable.Items)
            {
                LinkButton editButton = item.FindControl("editOrd") as LinkButton;
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(editButton);
            }
            if (!Page.IsPostBack && Request.QueryString["id"] != null)
            {
                string id = Request.QueryString["id"];
                Ordinance checkOrd = Factory.Instance.GetByID<Ordinance>(Convert.ToInt32(id), "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
                switch (checkOrd.LastUpdateBy.ToLower().Equals(_user.Login.ToLower()))
                {
                    case true:
                        GetByID(id.ToString());
                        break;
                    case false:
                        Response.Redirect("./AccessDenied");
                        break;
                }
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
            purchaseMethod.Items.Insert(0, new ListItem("Select Purchase Method...", null));
            purchaseMethod.Items.Insert(1, new ListItem("Low Bid", "Low Bid"));
            purchaseMethod.Items.Insert(2, new ListItem("Low Bid Meeting Specs", "Low Bid Meeting Specs"));
            purchaseMethod.Items.Insert(3, new ListItem("Low Evaluated Bid", "Low Evaluated Bid"));
            purchaseMethod.Items.Insert(4, new ListItem("Other", "Other"));
            purchaseMethod.Items.Insert(5, new ListItem("Exception", "Exception"));
        }
        protected void SetStartupActives()
        {
            draftsTable.Visible = true;
            ordView.Visible = false;
            lblNoItems.Visible = false;
        }
        protected void SetPagination(Repeater rpTable, Dictionary<string, LinkButton> pageBtns, Panel pnlPaging, HtmlGenericControl pnlFooter, Label lblPage, int ItemsPerPage, bool GetViewState = false)
        {
            if (GetViewState)
            {
                ViewState["PgNumP"] = Convert.ToInt32(Session["ViewStatePage"]);
            }
            SetViewState(ViewState, ItemsPerPage);
            GetControls(pageBtns["firstBtn"], pageBtns["previousBtn"], pageBtns["nextBtn"], pageBtns["lastBtn"], rpTable, pnlPaging, pnlFooter, lblPage);
        }
        public void GetStartupData()
        {
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = Factory.Instance.GetFilteredOrdinances(-1, string.Empty, string.Empty, string.Empty, _user.Login.ToLower());
            if (ord_list.Count > 0)
            {
                foreach (Ordinance ord in ord_list)
                {
                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                }                
                BindDataRepeaterPagination("yes", ord_list);
            }
            Dictionary<string, object> sortRet = new Dictionary<string, object>();
            sortRet = GetCurrentSort(ord_list, Session["curCmd"].ToString(), Session["sortDir"].ToString());

            Session["ord_list"] = sortRet["list"];
            Session["noFilterOrdList"] = ord_list;
            if (ord_list.Count > 0)
            {
                formTableDiv.Visible = true;
                lblNoItems.Visible = false;
                lblNoItemsTxt.InnerHtml = "There are no items to show for the current search";
            }
            else
            {
                formTableDiv.Visible = false;
                lblNoItems.Visible = true;
                lblNoItemsTxt.InnerHtml = "You have no saved drafts";
            }
        }
        protected void GetUploadedDocs()
        {
            if (Session["addOrdDocs"] != null && Session["ordDocs"] != null)
            {
                List<OrdinanceDocument> originalOrdDocList = Session["ordDocs"] as List<OrdinanceDocument>;
                rpSupportingDocumentation.DataSource = originalOrdDocList;
                rpSupportingDocumentation.DataBind();
            }
        }
        protected void GetByID(string id)
        {
            CommandEventArgs eventArgs = new CommandEventArgs("edit", id);
            RepeaterItem rpItem = new RepeaterItem(0, ListItemType.Item);
            RepeaterCommandEventArgs args = new RepeaterCommandEventArgs(rpItem, rpDraftsTable, eventArgs);
            rpDraftsTable_ItemCommand(rpDraftsTable, args);
        }



        // CONTROL CHANGES //
        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
            {
                { "firstBtn", lnkFirstSearchP },
                { "previousBtn", lnkPreviousSearchP },
                { "nextBtn", lnkNextSearchP },
                { "lastBtn", lnkLastSearchP },
            };

            SetPagination(rpDraftsTable, pageBtns, pnlPagingP, pnlFooter, lblCurrentPageBottomSearchP, 10);
            userInfo = (UserInfo)Session["UserInformation"];

            string title = !filterSearchTitle.Text.IsNullOrWhiteSpace() ? filterSearchTitle.Text : string.Empty;
            string user = _user.Login.ToLower();

            List<Ordinance> filteredList = new List<Ordinance>();

            filteredList = Factory.Instance.GetFilteredOrdinances(-1, string.Empty, string.Empty, title, user);

            if (filteredList.Count > 0)
            {
                foreach (Ordinance ord in filteredList)
                {
                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                }
            }

            Dictionary<string, object> sortRet = new Dictionary<string, object>();

            sortRet = GetCurrentSort(filteredList, Session["curCmd"].ToString(), Session["sortDir"].ToString());


            Session["ord_list"] = sortRet["list"];
            if (filteredList.Count > 0)
            {
                formTableDiv.Visible = true;
                lblNoItems.Visible = false;
            }
            else
            {
                formTableDiv.Visible = false;
                lblNoItems.Visible = true;
            }
            Session["ViewState"] = ViewState;
        }
        protected void SortBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
            SetPagination(rpDraftsTable, pageBtns, pnlPagingP, pnlFooter, lblCurrentPageBottomSearchP, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = (List<Ordinance>)Session["ord_list"];
            LinkButton button = (LinkButton)sender;
            string commandName = button.Attributes["data-command"];
            string commandArgument = Session["sortDir"].ToString();
            string commandText = button.Attributes["data-text"];

            Dictionary<string, object> sortRet = new Dictionary<string, object>();

            string prvBtn = Session["sortBtn"].ToString();
            switch (button.ID.Equals(prvBtn))
            {
                case true:
                    sortRet = SortButtonClick(ord_list, commandName, commandArgument);
                    break;

                case false:
                    sortRet = SortButtonClick(ord_list, commandName, "asc");
                    break;
            }



            Session["sortBtn"] = button.ID;
            Session["sortDir"] = sortRet["dir"];
            Session["ord_list"] = sortRet["list"];
            Session["curDir"] = sortRet["curDir"];
            Session["curCmd"] = sortRet["curCmd"];

            List<LinkButton> sortButtonsList = new List<LinkButton>()
            {
                sortDate,
                sortTitle,
            };
            foreach (LinkButton item in sortButtonsList)
            {
                item.Text = $"<strong>{item.Attributes["data-text"]}<span runat='server' class='float-end lh-1p5'></span></strong>";

            }

            button.Text = $"<strong>{commandText}<span runat='server' class='float-end lh-1p5 fas fa-arrow-{sortRet["arrow"]}'></span></strong>";


            Session["ViewState"] = ViewState;
        }
        protected void paginationBtn_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            string commandName = button.Attributes["data-command"];
            string listType = button.Attributes["data-list"];
            Dictionary<string, LinkButton> pageBtns;
            pageBtns = new Dictionary<string, LinkButton>()
            {
                { "firstBtn", lnkFirstSearchP },
                { "previousBtn", lnkPreviousSearchP },
                { "nextBtn", lnkNextSearchP },
                { "lastBtn", lnkLastSearchP },
            };
            SetPagination(rpDraftsTable, pageBtns, pnlPagingP, pnlFooter, lblCurrentPageBottomSearchP, 10, false);
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = (List<Ordinance>)Session["ord_list"];
            PageButtonClick(ord_list, commandName);
            Session["ViewState"] = ViewState;
        }
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
        protected void UploadDocBtn_Click(object sender, EventArgs e)
        {
            List<OrdinanceDocument> ordDocs = Session["ordDocs"] as List<OrdinanceDocument>;

            rpSupportingDocumentation.DataSource = ordDocs;
            rpSupportingDocumentation.DataBind();
        }
        protected void AddRequestEmailAddress_Click(object sender, EventArgs e)
        {
            SignatureRequest sigRequests = Session["SigRequestEmails"] as SignatureRequest;
            PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals("DirectorSupervisor"));

            List<string> emails = sigType.GetValue(sigRequests).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
            string[] newEmailAddresses = signatureEmailAddress.Text.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            foreach (string item in newEmailAddresses)
            {
                emails.Add(item.ToLower());
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

            int updateSigRequest = Factory.Instance.Update(sigRequests, "sp_UpdateOrdinance_SignatureRequest");
            if (updateSigRequest > 0)
            {
                signatureEmailAddress.Text = string.Empty;
            }
        }
        protected void BtnNewFactSheet_Click(object sender, EventArgs e)
        {
            Response.Redirect("./NewFactSheet");
        }



        // REPEATER COMMANDS //        
        protected void rpDraftsTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(backBtn);

            Session.Remove("OriginalOrdinance");
            Session.Remove("OriginalStatus");
            Session.Remove("OriginalRevTable");
            Session.Remove("OriginalExpTable");
            Session.Remove("SigRequestEmails");


            Session.Remove("revenue");
            Session.Remove("expenditure");
            Session.Remove("insertSigList");

            int ordID = Convert.ToInt32(e.CommandArgument);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            hdnOrdID.Value = ordID.ToString();
            hdnEffectiveDate.Value = ord.EffectiveDate.ToString();

            requestDepartment.SelectedValue = DepartmentsList()[ord.RequestDepartment];

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
                requestDivision.SelectedValue = GetDivisionsByDept(Convert.ToInt32(requestDepartment.SelectedValue)).First(i => i.DivisionName.Equals(ord.RequestDivision)).DivisionCode.ToString();
            }
            else
            {
                requestDivision.Enabled = false;
                requestDivision.Attributes.Add("data-required", "false");
                requestDivisionValid.Enabled = false;
                requestDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            }

            firstReadDate.Text = ord.FirstReadDate.ToString("yyyy-MM-dd");
            requestContact.Text = ord.RequestContact;
            requestEmail.Text = ord.RequestEmail;
            requestPhone.Text = ord.RequestPhone.SubstringUpToFirst('x');
            requestExt.Text = ord.RequestPhone.Substring(14);

            hdnEmail.Value = ord.RequestEmail;

            switch (ord.EmergencyPassage)
            {
                case true:
                    epYes.Checked = true;
                    epNo.Checked = false;
                    epJustificationGroup.Visible = true;
                    epJustificationValid.Enabled = true;
                    break;
                case false:
                    epYes.Checked = false;
                    epNo.Checked = true;
                    epJustificationGroup.Visible = false;
                    epJustificationValid.Enabled = false;
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
                    changeOrderNumberValid.Enabled = true;
                    additionalAmountValid.Enabled = true;
                    break;
                case false:
                    scYes.Checked = false;
                    scNo.Checked = true;
                    changeOrderNumberValid.Enabled = false;
                    additionalAmountValid.Enabled = false;
                    break;
            }
            changeOrderNumber.Text = ord.ChangeOrderNumber;
            additionalAmount.Text = NotApplicable(ord.AdditionalAmount.ToString());


            purchaseMethod.SelectedValue = ord.ContractMethod;
            switch (purchaseMethod.SelectedValue)
            {
                default:
                    otherExceptionDiv.Visible = false;
                    otherExceptionValid.Enabled = false;
                    break;
                case "Other":
                case "Exception":
                    otherExceptionDiv.Visible = true;
                    otherExceptionValid.Enabled = true;
                    break;
            }
            otherException.Text = ord.OtherException;
            prevOrdinanceNums.Text = ord.PreviousOrdinanceNumbers;
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
                    Session["OriginalRevTable"] = revItems;
                    rpRevenueTable.DataSource = revItems.OrderBy(i => i.OrdinanceAccountingID);
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
                    Session["OriginalExpTable"] = expItems;
                    rpExpenditureTable.DataSource = expItems.OrderBy(i => i.OrdinanceAccountingID);
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

            List<OrdinanceDocument> ordDocs = Factory.Instance.GetAllLookup<OrdinanceDocument>(ordID, "sp_GetOrdinanceDocumentsByOrdinanceID", "OrdinanceID");
            if (ordDocs.Count > 0)
            {
                supportingDocumentationDiv.Visible = true;
                Session["ordDocs"] = ordDocs;
                rpSupportingDocumentation.DataSource = ordDocs.OrderBy(i => i.DocumentID);
                rpSupportingDocumentation.DataBind();

                foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                {
                    LinkButton downloadFile = item.FindControl("supportingDocDownload") as LinkButton;
                    ScriptManager.GetCurrent(Page).RegisterPostBackControl(downloadFile);
                }
            }
            else
            {
                Session.Remove("ordDocs");
                rpSupportingDocumentation.DataSource = null;
                rpSupportingDocumentation.DataBind();
                supportingDocumentationDiv.Visible = false;
            }

            staffAnalysis.Text = ord.OrdinanceAnalysis;

            List<OrdinanceSignature> ordSigs = Factory.Instance.GetAllLookup<OrdinanceSignature>(ordID, "sp_GetOrdinanceSignatureByOrdinanceID", "OrdinanceID");

            SignatureRequest signatureRequest = Factory.Instance.GetByID<SignatureRequest>(Convert.ToInt32(hdnOrdID.Value), "sp_GetOrdinanceSignatureRequestByOrdinanceID", "OrdinanceID");
            PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals("DirectorSupervisor")); ;
            string[] emails = sigType.GetValue(signatureRequest).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            if (emails.Length > 0)
            {
                directorSupervisorEmailAddresses.Text = signatureRequest.DirectorSupervisor;
                rpEmailList.DataSource = emails;
                rpEmailList.DataBind();
            }
            else
            {
                rpEmailList.DataSource = null;
                rpEmailList.DataBind();
            }
            Session["SigRequestEmails"] = signatureRequest;

            OrdinanceStatus ordStatus = new OrdinanceStatus();
            ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
            bool adminUser = (userInfo.IsAdmin || !userInfo.UserView);
            hdnStatusID.Value = ordStatus.StatusID.ToString();
            hdnOrdStatusID.Value = ordStatus.OrdinanceStatusID.ToString();
            fiscalImpact.Attributes["placeholder"] = "$0.00";
            vendorNumber.Attributes["placeholder"] = "0123456789";
            contractTerm.Attributes["placeholder"] = "Calculating Term...";
            contractAmount.Attributes["placeholder"] = "$0.00";
            prevOrdinanceNums.Attributes["placeholder"] = "123-45-6789";
            codeProvision.Attributes["placeholder"] = "0123456789";
            contractStartDate.TextMode = TextBoxMode.Date;
            contractEndDate.TextMode = TextBoxMode.Date;
            newRevenueRowDiv.Visible = true;
            newExpenditureRowDiv.Visible = true;
            supportingDocumentationDiv.Visible = true;
            supportingDocumentation.Visible = true;
            UploadDocBtn.Visible = true;
            Session.Remove("RemoveAccs");
            Session.Remove("RemoveOrdAccs");
            Session.Remove("RemoveDocs");
            submitSection.Visible = true;
            if (rpRevenueTable.Items.Count > 0)
            {
                foreach (RepeaterItem item in rpRevenueTable.Items)
                {
                    HtmlGenericControl removeRevRowDiv = item.FindControl("removeRevRowDiv") as HtmlGenericControl;
                    Button removeRevRow = item.FindControl("removeRevenueRow") as Button;
                    TextBox revAmount = item.FindControl("revenueAmount") as TextBox;
                    removeRevRow.Visible = true;
                    revAmount.Attributes["placeholder"] = "$0.00";
                    ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeRevRow);
                }
            }
            if (rpExpenditureTable.Items.Count > 0)
            {
                foreach (RepeaterItem item in rpExpenditureTable.Items)
                {
                    HtmlGenericControl removeExpRowDiv = item.FindControl("removeExpRowDiv") as HtmlGenericControl;
                    Button removeExpRow = item.FindControl("removeExpenditureRow") as Button;
                    TextBox expAmount = item.FindControl("expenditureAmount") as TextBox;
                    removeExpRow.Visible = true;
                    expAmount.Attributes["placeholder"] = "$0.00";
                    ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeExpRow);
                }
            }
            foreach (RepeaterItem item in rpSupportingDocumentation.Items)
            {
                LinkButton deleteFile = item.FindControl("deleteFile") as LinkButton;
                deleteFile.Visible = true;
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(deleteFile);
            }


            switch (epYes.Checked)
            {
                case true:
                    epJustification.Attributes.Add("data-required", "true");
                    break;
                case false:
                    epJustification.Attributes.Remove("data-required");
                    break;
            }
            switch (scYes.Checked)
            {
                case true:
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;
                    changeOrderNumber.Attributes.Add("data-required", "true");
                    changeOrderNumber.Attributes.Add("placeholder", "0123456789");
                    additionalAmount.Attributes.Add("data-required", "true");
                    additionalAmount.Attributes.Add("placeholder", "$0.00");
                    break;

                case false:
                    changeOrderNumber.Enabled = false;
                    additionalAmount.Enabled = false;
                    changeOrderNumber.Attributes.Remove("data-required");
                    changeOrderNumber.Attributes.Remove("placeholder");
                    additionalAmount.Attributes.Remove("data-required");
                    additionalAmount.Attributes.Remove("placeholder");
                    break;
            }
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
            scopeChangeOptions.Visible = true;
            otherExceptionDiv.Visible = true;

            Session["OriginalOrdinance"] = ord;
            Session["OriginalStatus"] = ordStatus;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "CurrencyFormatting", "CurrencyFormatting();", true);

            ordView.Visible = true;
            draftsTable.Visible = false;
        }
        protected void rpAccountingTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string tableDesc = e.CommandArgument.ToString();
            List<OrdinanceAccounting> prvList = new List<OrdinanceAccounting>();
            List<OrdinanceAccounting> accountingList = new List<OrdinanceAccounting>();

            List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();

            switch (e.CommandName)
            {
                case "delete":
                    switch (tableDesc)
                    {
                        case "revenue":
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                OrdinanceAccounting accountingItem = new OrdinanceAccounting();
                                var revItem = rpRevenueTable.Items[i];
                                HiddenField revHdnIDField = (HiddenField)revItem.FindControl("hdnRevID");
                                TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                                TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                                TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                                TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                                TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                                TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                                accountingItem.OrdinanceAccountingID = Convert.ToInt32(revHdnIDField.Value);
                                accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
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
                                accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
                            }


                            HiddenField revHdnID = (HiddenField)e.Item.FindControl("hdnRevID");

                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(revHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(revHdnID.Value);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveOrdAccs"] = removeOrdAccs;

                            accountingList.RemoveAt(Convert.ToInt32(revHdnIndex.Value));
                            Session[tableDesc] = accountingList;
                            rpRevenueTable.DataSource = accountingList;
                            rpRevenueTable.DataBind();
                            break;
                        case "expenditure":
                            for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                            {
                                OrdinanceAccounting accountingItem = new OrdinanceAccounting();
                                var expItem = rpExpenditureTable.Items[i];
                                HiddenField expHdnIDField = (HiddenField)expItem.FindControl("hdnExpID");
                                TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                                TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                                TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                                TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                                TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                                TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                                accountingItem.OrdinanceAccountingID = Convert.ToInt32(expHdnIDField.Value);
                                accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
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
                                accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
                            }

                            HiddenField expHdnID = (HiddenField)e.Item.FindControl("hdnExpID");

                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(expHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(expHdnID.Value);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveOrdAccs"] = removeOrdAccs;

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
            HiddenField hdnDocID = (HiddenField)e.Item.FindControl("hdnDocID");
            HiddenField hdnDocIndex = (HiddenField)e.Item.FindControl("hdnDocIndex");
            List<OrdinanceDocument> ordDocList = Session["ordDocs"] as List<OrdinanceDocument>;
            List<OrdinanceDocument> addOrdDocList = new List<OrdinanceDocument>();
            if (Session["addOrdDocs"] != null)
            {
                addOrdDocList = Session["addOrdDocs"] as List<OrdinanceDocument>;
            }
            OrdinanceDocument ordDocItem = ordDocList[Convert.ToInt32(hdnDocIndex.Value)];

            switch (e.CommandName)
            {
                case "download":
                    string delivery = HttpContext.Current.IsDebuggingEnabled ? "inline" : "attachment";
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Length", ordDocItem.DocumentData.Length.ToString());
                    Response.AddHeader("Content-type", MimeMapping.GetMimeMapping(ordDocItem.DocumentName));
                    Response.AddHeader("Content-Disposition", $"{delivery}; filename={ordDocItem.DocumentName}");
                    Response.BinaryWrite(ordDocItem.DocumentData);
                    Response.Flush();
                    Response.End();
                    break;
                case "delete":
                    List<OrdinanceDocument> removeDocs = new List<OrdinanceDocument>();
                    if (Session["RemoveDocs"] != null)
                    {
                        removeDocs = Session["RemoveDocs"] as List<OrdinanceDocument>;
                    }
                    if (addOrdDocList.Any(i => i.Equals(ordDocItem)))
                    {
                        addOrdDocList.Remove(ordDocItem);
                        Session["addOrdDocs"] = addOrdDocList;
                    }
                    else
                    {
                        removeDocs.Add(ordDocItem);
                        Session["RemoveDocs"] = removeDocs;
                    }
                    ordDocList.Remove(ordDocItem);
                    Session["ordDocs"] = ordDocList;
                    rpSupportingDocumentation.DataSource = ordDocList;
                    rpSupportingDocumentation.DataBind();
                    break;
            }
        }
        protected void rpSupportingDocumentation_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            LinkButton btn = (LinkButton)e.Item.FindControl("deleteFile");
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn);
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
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                        {
                            OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
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
                        accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
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
                    HiddenField revHdnID = (HiddenField)revItem.FindControl("hdnRevID");
                    TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                    TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                    TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                    TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                    TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                    TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                    accountingItem.OrdinanceAccountingID = Convert.ToInt32(revHdnID.Value);
                    accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
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
                    HiddenField expHdnID = (HiddenField)expItem.FindControl("hdnExpID");
                    TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                    TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                    TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                    TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                    TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                    TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                    accountingItem.OrdinanceAccountingID = Convert.ToInt32(expHdnID.Value);
                    accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
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
        protected void SaveFactSheet_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Button btn = (Button)sender;
                Ordinance ordinance = new Ordinance();

                ordinance.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                ordinance.OrdinanceNumber = string.Empty;
                ordinance.AgendaNumber = string.Empty;
                ordinance.RequestDepartment = requestDepartment.SelectedItem.Text;
                ordinance.RequestDivision = !requestDivision.SelectedValue.IsNullOrWhiteSpace() ? requestDivision.SelectedItem.Text : requestDepartment.SelectedItem.Text;
                ordinance.RequestContact = requestContact.Text;
                ordinance.RequestPhone = $"{requestPhone.Text}{requestExt.Text}";
                ordinance.RequestEmail = requestEmail.Text.ToLower();
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
                ordinance.EffectiveDate = btn.CommandName.Equals("save") ? Convert.ToDateTime(hdnEffectiveDate.Value) : DateTime.Now;
                ordinance.ExpirationDate = DateTime.MaxValue;

                int retVal = Factory.Instance.Update(ordinance, "sp_UpdateOrdinance", Skips(key: "ordUpdate"));

                OrdinanceStatus ordStatus = new OrdinanceStatus();
                ordStatus.OrdinanceStatusID = Convert.ToInt32(hdnOrdStatusID.Value);
                ordStatus.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
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
                ordStatus.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
                ordStatus.ExpirationDate = DateTime.MaxValue;
                int statusVal = Factory.Instance.Update(ordStatus, "sp_UpdateOrdinance_Status", Skips("ordStatusUpdate"));

                int addDocsVal = new int();
                int addUploadedDocsVal = new int();
                
                List<string> addDocNames = new List<string>();
                if (Session["addOrdDocs"] != null)
                {
                    List<OrdinanceDocument> ordDocs = Session["addOrdDocs"] as List<OrdinanceDocument>;
                    if (ordDocs.Count > 0)
                    {
                        foreach (OrdinanceDocument ordDoc in ordDocs)
                        {
                            ordDoc.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                            addUploadedDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                            if (addUploadedDocsVal > 0)
                            {
                                addDocNames.Add(ordDoc.DocumentName);
                            }
                            else if (addUploadedDocsVal < 1)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        addUploadedDocsVal = 1;
                    }
                }
                else
                {
                    addUploadedDocsVal = 1;
                }
                if (supportingDocumentation.HasFiles)
                {
                    for (int i = 0; i < supportingDocumentation.PostedFiles.Count; i++)
                    {
                        OrdinanceDocument ordDoc = new OrdinanceDocument();
                        ordDoc.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
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
                        addDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                        if (addDocsVal > 0)
                        {
                            addDocNames.Add(ordDoc.DocumentName);
                        }
                        else if (addDocsVal < 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    addDocsVal = 1;
                }

                int removeDocVal = new int();
                List<OrdinanceDocument> removeDocs = new List<OrdinanceDocument>();
                List<string> removeDocNames = new List<string>();
                if (Session["RemoveDocs"] != null)
                {
                    removeDocs = Session["RemoveDocs"] as List<OrdinanceDocument>;
                }
                if (removeDocs.Count > 0)
                {
                    foreach (OrdinanceDocument item in removeDocs)
                    {
                        removeDocVal = Factory.Instance.Delete<OrdinanceDocument>(item.DocumentID, "OrdinanceDocument");
                        if (removeDocVal > 0)
                        {
                            removeDocNames.Add(item.DocumentName);
                        }
                        else if (removeDocVal < 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    removeDocVal = 1;
                }

                int removeOrdAccsVal = new int();
                List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();
                if (Session["RemoveOrdAccs"] != null)
                {
                    removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                }
                if (removeOrdAccs.Count > 0)
                {
                    foreach (OrdinanceAccounting item in removeOrdAccs)
                    {
                        item.LastUpdateBy = _user.Login;
                        item.LastUpdateDate = DateTime.Now;
                        item.EffectiveDate = DateTime.Now;
                        removeOrdAccsVal = Factory.Instance.Expire(item, "sp_UpdateOrdinance_Accounting");
                        if (removeOrdAccsVal < 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    removeOrdAccsVal = 1;
                }

                int updateRevAccsVal = new int();
                int updateExpAccsVal = new int();
                if (removeOrdAccsVal > 0)
                {
                    if (rpRevenueTable.Items.Count > 0)
                    {
                        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                        {
                            OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
                            if (accountingItem.OrdinanceAccountingID > 0)
                            {
                                updateRevAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdateOrdinance_Accounting");
                                List<OrdinanceAccounting> originalRevList = Session["OriginalRevTable"] as List<OrdinanceAccounting>;
                                OrdinanceAccounting originalRevItem = originalRevList.First(r => r.OrdinanceAccountingID.Equals(accountingItem.OrdinanceAccountingID));
                            }
                            else
                            {
                                updateRevAccsVal = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
                            }
                            if (updateRevAccsVal < 1)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        updateRevAccsVal = 1;
                    }

                    if (rpExpenditureTable.Items.Count > 0)
                    {
                        for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                        {
                            OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", i);
                            if (accountingItem.OrdinanceAccountingID > 0)
                            {
                                updateExpAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdateOrdinance_Accounting");
                                List<OrdinanceAccounting> originalExpList = Session["OriginalExpTable"] as List<OrdinanceAccounting>;
                                OrdinanceAccounting originalExpItem = originalExpList.First(r => r.OrdinanceAccountingID.Equals(accountingItem.OrdinanceAccountingID));
                            }
                            else
                            {
                                updateExpAccsVal = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
                            }
                            if (updateExpAccsVal < 1)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        updateExpAccsVal = 1;
                    }
                }

                string sigRequests = string.Empty;
                SignatureRequest signatureRequest = Factory.Instance.GetByID<SignatureRequest>(Convert.ToInt32(hdnOrdID.Value), "sp_GetOrdinanceSignatureRequestByOrdinanceID", "OrdinanceID");

                string departmentName = requestDepartment.SelectedItem.Text;
                string divisionName = !requestDivision.SelectedIndex.Equals(0) ? requestDivision.SelectedItem.Text : "None";
                int defaultDeptEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, "None").DefaultEmailsID;
                int defaultDeptDivEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, divisionName).DefaultEmailsID;
                string departmentDefaultList = Factory.Instance.GetByID<DefaultEmails>(defaultDeptEmailID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID").EmailAddress;
                string deptDivDefaultList = string.Empty;
                int updateSigRequest = 0;
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

                    signatureRequest.LastUpdateBy = _user.Login;
                    signatureRequest.LastUpdateDate = DateTime.Now;
                    updateSigRequest = Factory.Instance.Update(signatureRequest, "sp_UpdateOrdinance_SignatureRequest");
                }
                else
                {
                    updateSigRequest = 1;
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


                List<int> submitVals = new List<int>(new int[]
                {
                    retVal,
                    statusVal,
                    removeDocVal,
                    addDocsVal,
                    addUploadedDocsVal,
                    removeOrdAccsVal,
                    updateRevAccsVal,
                    updateExpAccsVal,
                    updateSigRequest
                });

                if (submitVals.All(i => i > 0))
                {
                    Session["SubmitStatus"] = "success";
                    Session["ToastColor"] = "text-bg-success";
                    switch (btn.CommandName)
                    {
                        case "submit":
                            Session["ToastMessage"] = "Form Submitted!";
                            Email.Instance.SendEmail(sigRequestEmail, directorSupervisorEmailAddresses.Text);
                            Email.Instance.SendEmail(submittedEmail, _user.Email);
                            break;
                        case "save":
                            Session["ToastMessage"] = "Form Saved!";
                            break;
                    }                    
                    Response.Redirect("./FactSheetDrafts");
                }
                else
                {
                    Session["SubmitStatus"] = "error";
                    Session["ToastColor"] = "text-bg-danger";
                    switch (btn.CommandName)
                    {
                        case "submit":
                            Session["ToastMessage"] = "Something went wrong while submitting!";
                            break;
                        case "save":
                            Session["ToastMessage"] = "Something went wrong while saving!";
                            break;
                    }
                }
            }
        }
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            int ordID = Convert.ToInt32(hdnDeleteID.Value);
            int ret = Factory.Instance.Delete<Ordinance>(ordID, "Ordinance");
            if (ret > 0)
            {

                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                Session["ToastMessage"] = "Entry Deleted!";
                Response.Redirect("./FactSheetDrafts");
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while deleting!";
            }
        }
        protected void backBtn_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                Response.Redirect("./FactSheetDrafts");
            }
            else
            {
                draftsTable.Visible = true;

                ordView.Visible = false;

                Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
                SetPagination(rpDraftsTable, pageBtns, pnlPagingP, pnlFooter, lblCurrentPageBottomSearchP, 10, true);
                List<Ordinance> ord_list = new List<Ordinance>();
                ord_list = (List<Ordinance>)Session["ord_list"];
                BindDataRepeaterPagination("no", ord_list);
            }
        }
    }
}