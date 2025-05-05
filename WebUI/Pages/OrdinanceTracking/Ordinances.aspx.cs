using DataLibrary;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static DataLibrary.TablePagination;
using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using System.Web.Services;
using static DataLibrary.Utility;
using ISD.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Reporting.WebForms;
using System.Data;
using DataLibrary.OrdinanceTracking;
using System.Reflection;

namespace WebUI
{
    public partial class Ordinances : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();
        private readonly string emailList = HttpContext.Current.IsDebuggingEnabled ? "NewFactSheetEmailListTEST" : "NewFactSheetEmailList";
        private readonly Dictionary<string, string> fieldLabels = FieldLabels();
        
        public string editSymbol = "<span class='fas fa-arrow-right-long mx-1 text-warning-light fw-bold align-self-center'></span>";
        public string addSymbol = "<span class='fas fa-plus mx-1 text-success fw-bold align-self-center'></span>";
        public string removeSymbol = "<span class='fas fa-minus mx-1 text-danger fw-bold align-self-center'></span>";

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;

            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("ordRevTable");
                Session.Remove("ordExpTable");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "EffectiveDate";
                Session["curDir"] = "desc";
                GetAllDepartments();
                GetAllStatuses();
                GetAllPurchaseMethods();
                SetStartupActives();
                SetPagination(rpOrdinanceTable, 10);
                GetStartupData(userInfo.IsAdmin);

                GetTestAuditData();
            }            
            if (Page.IsPostBack && Page.Request.Params.Get("__EVENTTARGET").Contains("adminSwitch"))
            {
                userInfo = ((SiteMaster)Page.Master).UserView();
                Session.Remove("ordRevTable");
                Session.Remove("ordExpTable");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "EffectiveDate";
                Session["curDir"] = "desc";
                SetStartupActives();
                SetPagination(rpOrdinanceTable, 10);
                filterStatus.SelectedIndex = 0;
                filterDepartment.SelectedIndex = 0;
                GetStartupData(userInfo.IsAdmin);
            }

            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                LinkButton editButton = item.FindControl("editOrd") as LinkButton;
                LinkButton viewButton = item.FindControl("viewOrd") as LinkButton;
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(editButton);
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(viewButton);
            }

            if (!Page.IsPostBack && Request.QueryString["id"] != null)
            {
                string id = Request.QueryString["id"];
                string cmd = Request.QueryString["v"] ?? "view";
                
                GetByID(id.ToString(), cmd.ToString());
                if (Request.QueryString["f"] != null)
                {
                    string ctrl = Request.QueryString["f"];
                    List<HtmlGenericControl> signBtnDivs = new List<HtmlGenericControl>()
                    {
                        fundsCheckByBtnDiv,
                        directorSupervisorBtnDiv,
                        cPABtnDiv,
                        obmDirectorBtnDiv,
                        mayorBtnDiv
                    };
                    List<Button> signBtns = new List<Button>()
                    {
                        fundsCheckByBtn,
                        directorSupervisorBtn,
                        cPABtn,
                        obmDirectorBtn,
                        mayorBtn
                    };
                    foreach (HtmlGenericControl item in signBtnDivs.Where(i => !i.ClientID.Equals($"{ctrl.ToString()}Div")))
                    {
                        item.Attributes["readonly"] = "true";
                    }
                    foreach (Button item in signBtns.Where(i => !i.ClientID.Equals(ctrl.ToString())))
                    {
                        item.Text = "Awaiting Signature...";
                    }
                    //Page.SetFocus(ctrl.ToString());
                }
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
                filterDepartment.Items.Add(newItem);
            }
        }
        protected void GetAllStatuses()
        {
            Dictionary<string, string> statuses = StatusList();
            foreach (var status in statuses.Keys)
            {
                var value = statuses[status];
                ListItem newItem = new ListItem(status, value);
                if (newItem.Text != "New" && newItem.Text != "Deleted")
                {
                    ddStatus.Items.Add(newItem);
                }
                filterStatus.Items.Add(newItem);
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
            ordTable.Visible = true;
            ordView.Visible = false;
            lblNoItems.Visible = false;
            filterDepartmentDiv.Visible = !userInfo.IsAdmin || userInfo.UserView ? false : true;
        }
        protected void SetPagination(Repeater rpTable, int ItemsPerPage)
        {
            SetViewState(ViewState, ItemsPerPage);
            GetControls(lnkFirstSearchP, lnkPreviousSearchP, lnkNextSearchP, lnkLastSearchP, rpTable, pnlPagingP, lblCurrentPageBottomSearchP);
        }
        public void GetStartupData(bool isAdmin)
        {
            List<Ordinance> ord_list = new List<Ordinance>();
            //ord_list = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
            ord_list = Factory.Instance.GetAllLookup<Ordinance>(0, "sp_GetOrdinanceByFilteredStatusID", "StatusID");
            if (ord_list.Count > 0)
            {
                foreach (Ordinance ord in ord_list)
                {
                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                }
                if ((userInfo.UserDepartmentName != null && !isAdmin) || userInfo.UserView)
                {
                    ord_list = FilterList(ord_list, "department", userInfo.UserDepartmentName);
                }
                BindDataRepeaterPagination("yes", ord_list);
            }

            Session["ord_list"] = ord_list;
            Session["noFilterOrdList"] = ord_list;
            if (ord_list.Count > 0)
            {
                formTableDiv.Visible = true;
                lblNoItems.Visible = false;
            }
            else
            {
                formTableDiv.Visible = false;
                lblNoItems.Visible = true;
            }
        }
        protected void GetUploadedDocs()
        {
            if (Session["addOrdDocs"] != null && Session["ordDocs"] != null)
            {
                List<OrdinanceDocument> originalOrdDocList = Session["ordDocs"] as List<OrdinanceDocument>;
                List<OrdinanceDocument> ordDocList = Session["addOrdDocs"] as List<OrdinanceDocument>;
                originalOrdDocList.AddRange(ordDocList);
                rpSupportingDocumentation.DataSource = originalOrdDocList;
                rpSupportingDocumentation.DataBind();
            }
        }
        protected void GetByID(string id, string cmd)
        {
            CommandEventArgs eventArgs = new CommandEventArgs(cmd, id);
            RepeaterItem rpItem = new RepeaterItem(0, ListItemType.Item);
            RepeaterCommandEventArgs args = new RepeaterCommandEventArgs(rpItem, rpOrdinanceTable, eventArgs);
            rpOrdinanceTable_ItemCommand(rpOrdinanceTable, args);
        }



        // CONTROL CHANGES //
        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            List<Ordinance> noFilterOrdList = new List<Ordinance>();

            DropDownList dropDown = (DropDownList)sender;
            string commandName = dropDown.Attributes["data-command"];
            string commandArgument = dropDown.SelectedItem.ToString();
            List<Ordinance> filteredList = new List<Ordinance>();
            userInfo = (UserInfo)Session["UserInformation"];


            switch (commandName)
            {
                case "department":
                    if (filterDepartment.SelectedValue != "")
                    {
                        switch (filterStatus.SelectedValue.Equals(""))
                        {
                            case true:
                                //filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                                filteredList = Factory.Instance.GetAllLookup<Ordinance>(0, "sp_GetOrdinanceByFilteredStatusID", "StatusID");
                                if (filteredList.Count > 0)
                                {
                                    foreach (Ordinance ord in filteredList)
                                    {
                                        OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                        ord.StatusDescription = ordStatus.StatusDescription;
                                    }
                                }
                                break;

                            case false:
                                if (filterStatus.SelectedValue != "7")
                                {
                                    filteredList = Factory.Instance.GetAllLookup<Ordinance>(Convert.ToInt32(filterStatus.SelectedValue), "sp_GetOrdinanceByStatusID", "StatusID");
                                    if (filteredList.Count > 0)
                                    {
                                        foreach (Ordinance ord in filteredList)
                                        {
                                            OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                            ord.StatusDescription = ordStatus.StatusDescription;
                                        }
                                    }
                                }
                                else
                                {
                                    filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetDeletedOrdinanceByEffective");

                                    if (filteredList.Count > 0)
                                    {
                                        foreach (Ordinance ord in filteredList)
                                        {
                                            OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                            ord.StatusDescription = ordStatus.StatusDescription;
                                        }
                                    }
                                }
                                break;
                        }
                        filteredList = FilterList(filteredList, commandName, commandArgument);
                    }
                    else
                    {
                        switch (filterStatus.SelectedValue.Equals(""))
                        {
                            case true:
                                //filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                                filteredList = Factory.Instance.GetAllLookup<Ordinance>(0, "sp_GetOrdinanceByFilteredStatusID", "StatusID");
                                if (filteredList.Count > 0)
                                {
                                    foreach (Ordinance ord in filteredList)
                                    {
                                        OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                        ord.StatusDescription = ordStatus.StatusDescription;
                                    }
                                }
                                BindDataRepeaterPagination("no", filteredList);
                                break;

                            case false:
                                if (filterStatus.SelectedValue != "7")
                                {
                                    filteredList = Factory.Instance.GetAllLookup<Ordinance>(Convert.ToInt32(filterStatus.SelectedValue), "sp_GetOrdinanceByStatusID", "StatusID");
                                    if (filteredList.Count > 0)
                                    {
                                        foreach (Ordinance ord in filteredList)
                                        {
                                            OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                            ord.StatusDescription = ordStatus.StatusDescription;
                                        }
                                    }
                                }
                                else
                                {
                                    filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetDeletedOrdinanceByEffective");

                                    if (filteredList.Count > 0)
                                    {
                                        foreach (Ordinance ord in filteredList)
                                        {
                                            OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                            ord.StatusDescription = ordStatus.StatusDescription;
                                        }
                                    }
                                }
                                BindDataRepeaterPagination("no", filteredList);
                                break;
                        }
                    }
                    break;

                case "status":
                    if (filterStatus.SelectedValue != "")
                    {
                        if (filterStatus.SelectedValue != "7")
                        {
                            filteredList = Factory.Instance.GetAllLookup<Ordinance>(Convert.ToInt32(dropDown.SelectedValue), "sp_GetOrdinanceByStatusID", "StatusID");

                            if (filteredList.Count > 0)
                            {
                                foreach (Ordinance ord in filteredList)
                                {
                                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                    ord.StatusDescription = ordStatus.StatusDescription;
                                }
                            }

                            switch (filterDepartment.SelectedValue.Equals(""))
                            {
                                case true:
                                    switch (userInfo.IsAdmin && !userInfo.UserView)
                                    {
                                        case true:
                                            BindDataRepeaterPagination("no", filteredList);
                                            break;

                                        case false:
                                            filteredList = FilterList(filteredList, "department", userInfo.UserDepartmentName);
                                            break;
                                    }
                                    break;

                                case false:
                                    filteredList = FilterList(filteredList, "department", filterDepartment.SelectedItem.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetDeletedOrdinanceByEffective");

                            if (filteredList.Count > 0)
                            {
                                foreach (Ordinance ord in filteredList)
                                {
                                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                    ord.StatusDescription = ordStatus.StatusDescription;
                                }
                            }

                            switch (filterDepartment.SelectedValue.Equals(""))
                            {
                                case true:
                                    switch (userInfo.IsAdmin && !userInfo.UserView)
                                    {
                                        case true:
                                            BindDataRepeaterPagination("no", filteredList);
                                            break;

                                        case false:
                                            filteredList = FilterList(filteredList, "department", userInfo.UserDepartmentName);
                                            break;
                                    }
                                    break;

                                case false:
                                    filteredList = FilterList(filteredList, "department", filterDepartment.SelectedItem.ToString());
                                    break;
                            }
                        }
                    }
                    else
                    {
                        //filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                        filteredList = Factory.Instance.GetAllLookup<Ordinance>(0, "sp_GetOrdinanceByFilteredStatusID", "StatusID");
                        if (filteredList.Count > 0)
                        {
                            foreach (Ordinance ord in filteredList)
                            {
                                OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                ord.StatusDescription = ordStatus.StatusDescription;
                            }
                        }
                        switch (filterDepartment.SelectedValue.Equals(""))
                        {
                            case true:
                                switch (userInfo.IsAdmin && !userInfo.UserView)
                                {
                                    case true:
                                        BindDataRepeaterPagination("no", filteredList);
                                        break;

                                    case false:
                                        filteredList = FilterList(filteredList, "department", userInfo.UserDepartmentName);
                                        break;
                                }
                                break;

                            case false:
                                filteredList = FilterList(filteredList, "department", filterDepartment.SelectedItem.ToString());
                                break;
                        }

                    }
                    break;
            }
            Dictionary<string, object> sortRet = new Dictionary<string, object>();

            sortRet = SortButtonClick(filteredList, Session["curCmd"].ToString(), Session["curDir"].ToString());


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
        }
        protected void SortBtn_Click(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
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
                sortID,
                sortDate,
                sortTitle,
                sortDepartment,
                sortContact,
                sortStatus
            };
            foreach (LinkButton item in sortButtonsList)
            {
                item.Text = $"<strong>{item.Attributes["data-text"]}<span runat='server' class='float-end lh-1p5'></span></strong>";
            }

            button.Text = $"<strong>{commandText}<span runat='server' class='float-end lh-1p5 fas fa-arrow-{sortRet["arrow"]}'></span></strong>";
        }
        protected void paginationBtn_Click(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = (List<Ordinance>)Session["ord_list"];
            LinkButton button = (LinkButton)sender;
            string commandName = button.Attributes["data-command"];
            PageButtonClick(ord_list, commandName);
        }
        protected void PurchaseMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (purchaseMethod.SelectedItem.Value)
            {
                default:
                    otherException.Enabled = false;
                    otherException.Text = string.Empty;
                    otherException.Attributes.Remove("required");
                    break;
                case "Other":
                case "Exception":
                    otherException.Enabled = true;
                    otherException.Attributes.Add("required", "true");
                    break;
            }
        }
        protected void EPCheckedChanged(object sender, EventArgs e)
        {
            switch (epYes.Checked)
            {
                case true:
                    epJustificationGroup.Visible = true;
                    epJustification.Attributes.Add("required", "true");
                    break;

                case false:
                    epJustificationGroup.Visible = false;
                    epJustification.Attributes.Remove("required");
                    break;
            }
        }
        protected void SCCheckedChanged(object sender, EventArgs e)
        {
            switch (scYes.Checked)
            {
                case true:
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;
                    changeOrderNumber.Attributes.Add("required", "true");
                    additionalAmount.Attributes.Add("required", "true");
                    break;

                case false:
                    changeOrderNumber.Enabled = false;
                    additionalAmount.Enabled = false;
                    changeOrderNumber.Attributes.Remove("required");
                    additionalAmount.Attributes.Remove("required");
                    break;
            }
        }
        protected void UploadDocBtn_Click(object sender, EventArgs e)
        {
            List<OrdinanceDocument> originalOrdDocList = (Session["ordDocs"] != null) ? Session["ordDocs"] as List<OrdinanceDocument> : new List<OrdinanceDocument>();
            List<OrdinanceDocument> ordDocList = (Session["addOrdDocs"] != null) ? Session["addOrdDocs"] as List<OrdinanceDocument> : new List<OrdinanceDocument>();

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
            Session["addOrdDocs"] = ordDocList;
            originalOrdDocList.AddRange(ordDocList);
            rpSupportingDocumentation.DataSource = originalOrdDocList;
            rpSupportingDocumentation.DataBind();
        }
        protected void btnSendSigEmail_Click(object sender, EventArgs e)
        {
            Email.Instance.AddEmailAddress("SingleEmail", signatureEmailAddress.Text);
            string href = $"apptest/Themis/Ordinances?id={hdnOrdID.Value.ToString()}&v=edit&f={sigBtnTarget.Value.ToString()}";
            string formType = "THΣMIS";

            Email newEmail = new Email();

            newEmail.EmailSubject = $"{formType} Signature Requested";
            newEmail.EmailTitle = $"{formType} Signature Requested";
            newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>You are receiving this message because your signature is required in the role of <b>{sigBtnLabel.Value.ToString()}</b> for an ordinance on THΣMIS.</span></p><p><span>Please click the button below to review and sign the document</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #198754; border-radius: 5px; text-align: center;' valign='top' bgcolor='#198754' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #198754; border: solid 1px #198754; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #198754; '>Sign Ordinance</a></td></tr></table><br /><p><span>Thank you for your prompt attention to this matter.</span></p>";

            Email.Instance.SendEmail(newEmail, "SingleEmail");
        }



        // REPEATER COMMANDS //        
        protected void rpOrdinanceTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(backBtn);

            Session.Remove("OriginalOrdinance");
            Session.Remove("ordRevTable");
            Session.Remove("ordExpTable");
            Session.Remove("insertSigList");            

            List<TextBox> sigTextBoxes = new List<TextBox>()
            {
                fundsCheckBySig,
                directorSupervisorSig,
                cPASig,
                obmDirectorSig,
                mayorSig
            };
            List<LinkButton> emailBtns = new List<LinkButton>() 
            {
                fundsCheckEmailBtn,
                directorSupervisorEmailBtn,
                cPAEmailBtn,
                obmDirectorEmailBtn,
                mayorEmailBtn,
            };
            List<Button> signBtns = new List<Button>()
            {
                fundsCheckByBtn,
                directorSupervisorBtn,
                cPABtn,
                obmDirectorBtn,
                mayorBtn
            };

            int ordID = Convert.ToInt32(e.CommandArgument);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            hdnOrdID.Value = ordID.ToString();
            hdnEffectiveDate.Value = ord.EffectiveDate.ToString();

            lblOrdID.Text = $"ID: {ordID.ToString()}";
            ordinanceNumber.Text = ord.OrdinanceNumber;
            requestDepartment.SelectedValue = DepartmentsList()[ord.RequestDepartment];
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
                    break;
                case false:
                    epYes.Checked = false;
                    epNo.Checked = true;
                    epJustificationGroup.Visible = false;
                    break;
            }
            epJustification.Text = ord.EmergencyPassageReason;

            fiscalImpact.Text = ord.OrdinanceFiscalImpact.ToString();
            suggestedTitle.Text = ord.OrdinanceTitle;

            vendorName.Text = ord.ContractVendorName;
            vendorNumber.Text = ord.ContractVendorNumber;
            contractStartDate.Text = ord.ContractStartDate;
            contractEndDate.Text = ord.ContractEndDate;
            contractTerm.Value = ord.ContractTerm;
            contractAmount.Text = ord.ContractAmount.ToString();

            switch (ord.ScopeChange)
            {
                case true:
                    scYes.Checked = true;
                    scNo.Checked = false;
                    scopeChangeOptions.Visible = true;
                    break;
                case false:
                    scYes.Checked = false;
                    scNo.Checked = true;
                    scopeChangeOptions.Visible = false;
                    break;
            }
            changeOrderNumber.Text = ord.ChangeOrderNumber;
            additionalAmount.Text = (ord.AdditionalAmount.ToString() != "-1.00") ? ord.AdditionalAmount.ToString() : string.Empty;


            purchaseMethod.SelectedValue = ord.ContractMethod;
            switch (purchaseMethod.SelectedValue)
            {
                default:
                    otherExceptionDiv.Visible = false;
                    break;
                case "Other":
                case "Exception":
                    otherExceptionDiv.Visible = true;
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
            List<Accounting> revItems = new List<Accounting>();
            List<Accounting> expItems = new List<Accounting>();
            if (ordAcc.Count > 0)
            {
                foreach (OrdinanceAccounting item in ordAcc)
                {
                    Accounting acctItem = Factory.Instance.GetByID<Accounting>(item.AccountingID, "sp_GetLkAccountingByAccountingID", "AccountingID");
                    switch (acctItem.AccountingDesc)
                    {
                        case "revenue":
                            revItems.Add(acctItem);
                            break;
                        case "expenditure":
                            expItems.Add(acctItem);
                            break;
                    }
                }

                if (revItems.Count > 0)
                {
                    Session["ordRevTable"] = revItems;
                    rpRevenueTable.DataSource = revItems;
                    rpRevenueTable.DataBind();
                }
                else
                {
                    Session.Remove("ordRevTable");
                    rpRevenueTable.DataSource = null;
                    rpRevenueTable.DataBind();
                }
                if (expItems.Count > 0)
                {
                    Session["ordExpTable"] = expItems;
                    rpExpenditureTable.DataSource = expItems;
                    rpExpenditureTable.DataBind();
                }
                else
                {
                    Session.Remove("ordExpTable");
                    rpExpenditureTable.DataSource = null;
                    rpExpenditureTable.DataBind();
                }
            }
            else
            {
                Session.Remove("ordRevTable");
                Session.Remove("ordExpTable");
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
                rpSupportingDocumentation.DataSource = ordDocs;
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
            
            

            OrdinanceStatus ordStatus = new OrdinanceStatus();

            switch (e.CommandName)
            {
                case "view":
                    ordView.Attributes["readonly"] = "true";
                    ordinanceTabs.Visible = true;
                    copyOrd.Visible = true;
                    ddStatusDiv.Visible = false;
                    statusDiv.Visible = true;
                    requiredFieldDescriptor.Visible = false;
                    vendorNumber.Attributes["placeholder"] = "N/A";
                    contractTerm.Attributes["placeholder"] = "N/A";
                    prevOrdinanceNums.Attributes["placeholder"] = "N/A";
                    codeProvision.Attributes["placeholder"] = "N/A";
                    newRevenueRowDiv.Visible = false;
                    newExpenditureRowDiv.Visible = false;
                    supportingDocumentation.Visible = false;
                    UploadDocBtn.Visible = false;
                    submitSection.Visible = false;
                    if (rpRevenueTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpRevenueTable.Items)
                        {
                            HtmlGenericControl removeRevRow = item.FindControl("removeRevRowDiv") as HtmlGenericControl;
                            TextBox revAmount = item.FindControl("revenueAmount") as TextBox;
                            removeRevRow.Visible = false;
                            revAmount.Attributes["placeholder"] = "N/A";
                        }
                    }
                    if (rpExpenditureTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpExpenditureTable.Items)
                        {
                            HtmlGenericControl removeExpRow = item.FindControl("removeExpRowDiv") as HtmlGenericControl;
                            TextBox expAmount = item.FindControl("expenditureAmount") as TextBox;
                            removeExpRow.Visible = false;
                            expAmount.Attributes["placeholder"] = "N/A";
                        }
                    }
                    foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                    {
                        LinkButton deleteFile = item.FindControl("deleteFile") as LinkButton;
                        deleteFile.Visible = false;
                    }

                    if (ord.ContractStartDate.Length < 1)
                    {
                        contractStartDate.TextMode = TextBoxMode.SingleLine;
                        contractStartDate.Attributes["placeholder"] = "N/A";
                    }
                    else
                    {
                        contractStartDate.TextMode = TextBoxMode.Date;
                    }

                    if (ord.ContractEndDate.Length < 1)
                    {
                        contractEndDate.TextMode = TextBoxMode.SingleLine;
                        contractEndDate.Attributes["placeholder"] = "N/A";
                    }
                    else
                    {
                        contractEndDate.TextMode = TextBoxMode.Date;
                    }

                    otherException.Enabled = true;
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;

                    ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                    statusLabel.InnerHtml = ord.StatusDescription;
                    switch (ord.StatusDescription)
                    {
                        case "New":
                            statusIcon.Attributes["class"] = "fas fa-sparkles text-primary";
                            statusLabel.Attributes["class"] = "text-primary";
                            copyOrd.Visible = false;
                            ordinanceNumberDiv.Visible = false;
                            break;
                        case "Pending":
                            statusIcon.Attributes["class"] = "fas fa-hourglass-clock text-warning-light";
                            statusLabel.Attributes["class"] = "text-warning-light";
                            copyOrd.Visible = true;
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Under Review":
                            statusIcon.Attributes["class"] = "fas fa-memo-circle-info text-info";
                            statusLabel.Attributes["class"] = "text-info";
                            copyOrd.Visible = true;
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Being Held":
                            statusIcon.Attributes["class"] = "fas fa-triangle-exclamation text-hazard";
                            statusLabel.Attributes["class"] = "text-hazard";
                            copyOrd.Visible = true;
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Drafted":
                            statusIcon.Attributes["class"] = "fas fa-badge-check text-success";
                            statusLabel.Attributes["class"] = "text-success";
                            copyOrd.Visible = true;
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Rejected":
                            statusIcon.Attributes["class"] = "fas fa-ban text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            copyOrd.Visible = true;
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Deleted":
                            statusIcon.Attributes["class"] = "fas fa-trash-xmark text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            copyOrd.Visible = true;
                            ordinanceNumberDiv.Visible = true;
                            break;
                    }

                    foreach (LinkButton item in emailBtns)
                    {
                        item.Visible = false;
                    }

                    foreach (TextBox item in sigTextBoxes) if (!item.Text.IsNullOrWhiteSpace())
                    {
                        switch (item.ClientID)
                        {
                            case "fundsCheckBySig":
                                fundsCheckByBtnDiv.Visible = false;
                                fundsCheckByInputGroup.Visible = true;
                                break;
                            case "directorSupervisorSig":
                                directorSupervisorBtnDiv.Visible = false;
                                directorSupervisorInputGroup.Visible = true;
                                break;
                            case "cPASig":
                                cPABtnDiv.Visible = false;
                                cPAInputGroup.Visible = true;
                                break;
                            case "obmDirectorSig":
                                obmDirectorBtnDiv.Visible = false;
                                obmDirectorInputGroup.Visible = true;
                                break;
                            case "mayorSig":
                                mayorBtnDiv.Visible = false;
                                mayorInputGroup.Visible = true;
                                break;
                        }
                    }
                    else
                    {
                        switch (item.ClientID)
                        {
                            case "fundsCheckBySig":
                                fundsCheckByBtnDiv.Visible = true;
                                fundsCheckByBtn.Text = "Awaiting Signature...";
                                fundsCheckByInputGroup.Visible = false;
                                break;
                            case "directorSupervisorSig":
                                directorSupervisorBtnDiv.Visible = true;
                                directorSupervisorBtn.Text = "Awaiting Signature...";
                                directorSupervisorInputGroup.Visible = false;
                                break;
                            case "cPASig":
                                cPABtnDiv.Visible = true;
                                cPABtn.Text = "Awaiting Signature...";
                                cPAInputGroup.Visible = false;
                                break;
                            case "obmDirectorSig":
                                obmDirectorBtnDiv.Visible = true;
                                obmDirectorBtn.Text = "Awaiting Signature...";
                                obmDirectorInputGroup.Visible = false;
                                break;
                            case "mayorSig":
                                mayorBtnDiv.Visible = true;
                                mayorBtn.Text = "Awaiting Signature...";
                                mayorInputGroup.Visible = false;
                                break;
                        }
                    }
                    break;
                case "edit":
                    ordView.Attributes["readonly"] = "false";
                    ordinanceTabs.Visible = false;
                    copyOrd.Visible = false;
                    ddStatusDiv.Visible = !userInfo.IsAdmin || userInfo.UserView ? false : true;
                    ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                    bool adminUser = (userInfo.IsAdmin || !userInfo.UserView) ? true : false;
                    if (ordStatus.StatusDescription != "New" && adminUser)
                    {
                        ddStatus.SelectedValue = ordStatus.StatusID.ToString();
                    }
                    else if (ordStatus.StatusDescription == "New" && adminUser)
                    {
                        hdnStatusID.Value = ordStatus.StatusID.ToString();
                    }
                    else
                    {
                        ddStatus.SelectedIndex = 0;
                    }
                    statusLabel.InnerHtml = ord.StatusDescription;
                    switch (ord.StatusDescription)
                    {
                        case "New":
                            statusIcon.Attributes["class"] = "fas fa-sparkles text-primary";
                            statusLabel.Attributes["class"] = "text-primary";
                            signatureSection.Visible = false;
                            break;
                        case "Pending":
                            statusIcon.Attributes["class"] = "fas fa-hourglass-clock text-warning-light";
                            statusLabel.Attributes["class"] = "text-warning-light";
                            signatureSection.Visible = true;
                            break;
                        case "Under Review":
                            statusIcon.Attributes["class"] = "fas fa-memo-circle-info text-info";
                            statusLabel.Attributes["class"] = "text-info";
                            signatureSection.Visible = true;
                            break;
                        case "Being Held":
                            statusIcon.Attributes["class"] = "fas fa-triangle-exclamation text-hazard";
                            statusLabel.Attributes["class"] = "text-hazard";
                            signatureSection.Visible = true;
                            break;
                        case "Drafted":
                            statusIcon.Attributes["class"] = "fas fa-badge-check text-success";
                            statusLabel.Attributes["class"] = "text-success";
                            signatureSection.Visible = true;
                            break;
                        case "Rejected":
                            statusIcon.Attributes["class"] = "fas fa-ban text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            signatureSection.Visible = true;
                            break;
                        case "Deleted":
                            statusIcon.Attributes["class"] = "fas fa-trash-xmark text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            signatureSection.Visible = true;
                            break;
                    }
                    hdnOrdStatusID.Value = ordStatus.OrdinanceStatusID.ToString();
                    statusDiv.Visible = !userInfo.IsAdmin || userInfo.UserView ? true : false;
                    requiredFieldDescriptor.Visible = true;
                    vendorNumber.Attributes["placeholder"] = "0123456789";
                    contractTerm.Attributes["placeholder"] = "Calculating Term...";
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
                            revAmount.Attributes["placeholder"] = "$10,000.00";
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
                            expAmount.Attributes["placeholder"] = "$10,000.00";
                            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeExpRow);
                        }
                    }
                    foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                    {
                        LinkButton deleteFile = item.FindControl("deleteFile") as LinkButton;
                        deleteFile.Visible = true;
                        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(deleteFile);
                    }

                    switch (scYes.Checked)
                    {
                        case true:
                            changeOrderNumber.Enabled = true;
                            additionalAmount.Enabled = true;
                            changeOrderNumber.Attributes.Add("required", "true");
                            additionalAmount.Attributes.Add("required", "true");
                            break;

                        case false:
                            changeOrderNumber.Enabled = false;
                            additionalAmount.Enabled = false;
                            changeOrderNumber.Attributes.Remove("required");
                            additionalAmount.Attributes.Remove("required");
                            break;
                    }
                    switch (purchaseMethod.SelectedItem.Value)
                    {
                        default:
                            otherException.Enabled = false;
                            otherException.Text = string.Empty;
                            otherException.Attributes.Remove("required");
                            break;
                        case "Other":
                        case "Exception":
                            otherException.Enabled = true;
                            otherException.Attributes.Add("required", "true");
                            break;
                    }
                    scopeChangeOptions.Visible = true;
                    otherExceptionDiv.Visible = true;

                    foreach (LinkButton item in emailBtns) if (!userInfo.IsAdmin || userInfo.UserView || Request.QueryString["id"] != null)
                    {
                        item.Visible = false;
                    }
                    else
                    {
                        item.Visible = true;
                    }
                    foreach (Button item in signBtns)
                    {
                        item.Text = "Sign";
                    }
                    foreach (TextBox item in sigTextBoxes) if (!item.Text.IsNullOrWhiteSpace())
                    {
                        switch (item.ClientID)
                        {
                            case "fundsCheckBySig":
                                fundsCheckByBtnDiv.Visible = false;
                                fundsCheckByInputGroup.Visible = true;
                                break;
                            case "directorSupervisorSig":
                                directorSupervisorBtnDiv.Visible = false;
                                directorSupervisorInputGroup.Visible = true;
                                break;
                            case "cPASig":
                                cPABtnDiv.Visible = false;
                                cPAInputGroup.Visible = true;
                                break;
                            case "obmDirectorSig":
                                obmDirectorBtnDiv.Visible = false;
                                obmDirectorInputGroup.Visible = true;
                                break;
                            case "mayorSig":
                                mayorBtnDiv.Visible = false;
                                mayorInputGroup.Visible = true;
                                break;
                        }
                    }
                    else
                    {
                        if (!userInfo.IsAdmin || userInfo.UserView || Request.QueryString["f"] != null)
                        {
                            switch (item.ClientID)
                            {
                                case "fundsCheckBySig":
                                    fundsCheckByBtnDiv.Visible = true;
                                    fundsCheckByBtn.Text = "Awaiting Signature...";
                                    fundsCheckByBtnDiv.Attributes["readonly"] = "true";
                                    fundsCheckByInputGroup.Visible = false;
                                    break;
                                case "directorSupervisorSig":
                                    directorSupervisorBtnDiv.Visible = true;
                                    directorSupervisorBtn.Text = "Awaiting Signature...";
                                    directorSupervisorBtnDiv.Attributes["readonly"] = "true";
                                    directorSupervisorInputGroup.Visible = false;
                                    break;
                                case "cPASig":
                                    cPABtnDiv.Visible = true;
                                    cPABtn.Text = "Awaiting Signature...";
                                    cPABtnDiv.Attributes["readonly"] = "true";
                                    cPAInputGroup.Visible = false;
                                    break;
                                case "obmDirectorSig":
                                    obmDirectorBtnDiv.Visible = true;
                                    obmDirectorBtn.Text = "Awaiting Signature...";
                                    obmDirectorBtnDiv.Attributes["readonly"] = "true";
                                    obmDirectorInputGroup.Visible = false;
                                    break;
                                case "mayorSig":
                                    mayorBtnDiv.Visible = true;
                                    mayorBtn.Text = "Awaiting Signature...";
                                    mayorBtnDiv.Attributes["readonly"] = "true";
                                    mayorInputGroup.Visible = false;
                                    break;
                            }
                        }
                        else
                        {
                            switch (item.ClientID)
                            {
                                case "fundsCheckBySig":
                                    fundsCheckByBtnDiv.Visible = true;
                                    fundsCheckByBtn.Text = "Sign";
                                    fundsCheckByBtnDiv.Attributes.Remove("readonly");
                                    fundsCheckByInputGroup.Visible = false;
                                    break;
                                case "directorSupervisorSig":
                                    directorSupervisorBtnDiv.Visible = true;
                                    directorSupervisorBtn.Text = "Sign";
                                    directorSupervisorBtnDiv.Attributes.Remove("readonly");
                                    directorSupervisorInputGroup.Visible = false;
                                    break;
                                case "cPASig":
                                    cPABtnDiv.Visible = true;
                                    cPABtn.Text = "Sign";
                                    cPABtnDiv.Attributes.Remove("readonly");
                                    cPAInputGroup.Visible = false;
                                    break;
                                case "obmDirectorSig":
                                    obmDirectorBtnDiv.Visible = true;
                                    obmDirectorBtn.Text = "Sign";
                                    obmDirectorBtnDiv.Attributes.Remove("readonly");
                                    obmDirectorInputGroup.Visible = false;
                                    break;
                                case "mayorSig":
                                    mayorBtnDiv.Visible = true;
                                    mayorBtn.Text = "Sign";
                                    mayorBtnDiv.Attributes.Remove("readonly");
                                    mayorInputGroup.Visible = false;
                                    break;
                            }
                        }
                    }

                    switch (ord.StatusDescription)
                    {
                        case "New":                            
                            ordinanceNumberDiv.Visible = false;
                            break;
                        case "Pending":                            
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Under Review":                            
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Being Held":                            
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Drafted":                            
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Rejected":                            
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Deleted":                            
                            ordinanceNumberDiv.Visible = true;
                            break;
                    }

                    Session["OriginalOrdinance"] = ord;
                    break;
                case "download":
                    ReportViewer viewer = new ReportViewer();
                    viewer.LocalReport.ReportPath = "./Reports/OrdinanceTracking/Ordinance.rdlc";
                    if (ord.ContractStartDate.Length > 0)
                    {
                        ord.ContractStartDate = Convert.ToDateTime(ord.ContractStartDate).ToString("MM/dd/yyyy");
                    }
                    if (ord.ContractEndDate.Length > 0)
                    {
                        ord.ContractEndDate = Convert.ToDateTime(ord.ContractEndDate).ToString("MM/dd/yyyy");
                    }
                    bool hideTables = true;
                    if (revItems.Count > 0 || expItems.Count > 0)
                    {
                        hideTables = false;
                    }
                    RevExpBool HideTables = new RevExpBool() { HideTables = hideTables};
                    List<string> sigTypeList = new List<string>()
            {
                "fundsCheckBy",
                "directorSupervisor",
                "cPA",
                "obmDirector",
                "mayor",
            };
                    foreach (string item in sigTypeList)
                    {
                        if (!ordSigs.Any(i => i.SignatureType.Equals(item)))
                        {
                            OrdinanceSignature blankSig = new OrdinanceSignature()
                            {
                                SignatureID = -1,
                                SortOrder = 0,
                                OrdinanceID = Convert.ToInt32(ordID),
                                SignatureType = item,
                                Signature = string.Empty,
                                DateSigned = DateTime.MinValue,
                                SignatureCertified = false,
                                LastUpdateBy = string.Empty,
                                LastUpdateDate = DateTime.Now
                            };
                            ordSigs.Add(blankSig);
                        }
                    }
                    foreach (OrdinanceSignature item in ordSigs)
                    {
                        switch (item.SignatureType)
                        {
                            case "fundsCheckBy":
                                item.SortOrder = 0;
                                break;
                            case "directorSupervisor":
                                item.SortOrder = 1;
                                break;
                            case "cPA":
                                item.SortOrder = 2;
                                break;
                            case "obmDirector":
                                item.SortOrder = 3;
                                break;
                            case "mayor":
                                item.SortOrder = 4;
                                break;
                        }
                    }

                    IEnumerable<Ordinance> ordData = new[] { ord };
                    IEnumerable<RevExpBool> revExpBoolData = new[] { HideTables };
                    ReportDataSource ordinanceData = new ReportDataSource() { Name = "dsOrdinance", Value = ordData };
                    ReportDataSource revExpTableBoolData = new ReportDataSource() { Name = "dsRevExpBool", Value = revExpBoolData };
                    ReportDataSource ordinanceRevAccountingData = new ReportDataSource() { Name = "dsRevAccounting", Value = revItems,  };
                    ReportDataSource ordinanceExpAccountingData = new ReportDataSource() { Name = "dsExpAccounting", Value = expItems };
                    ReportDataSource ordinanceStatusData = new ReportDataSource() { Name = "dsOrdinanceStatus" };
                    ReportDataSource ordinanceSignaturesData = new ReportDataSource() { Name = "dsSignatures", Value = ordSigs.OrderBy(i => i.SortOrder) };

                    viewer.LocalReport.DataSources.Add(ordinanceData);
                    viewer.LocalReport.DataSources.Add(revExpTableBoolData);
                    viewer.LocalReport.DataSources.Add(ordinanceRevAccountingData);
                    viewer.LocalReport.DataSources.Add(ordinanceExpAccountingData);
                    viewer.LocalReport.DataSources.Add(ordinanceStatusData);
                    viewer.LocalReport.DataSources.Add(ordinanceSignaturesData);

                    viewer.LocalReport.Refresh();

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    string delivery = HttpContext.Current.IsDebuggingEnabled ? "inline" : "attachment";

                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", $"{delivery}; filename=Ordinance_{ord.OrdinanceID}.pdf");
                    Response.BinaryWrite(bytes); // create the file                    
                    Context.ApplicationInstance.CompleteRequest();
                    break;
            }

            
            foreach (OrdinanceSignature item in ordSigs)
            {
                switch (item.SignatureType)
                {
                    case "fundsCheckBy":
                        fundsCheckByBtnDiv.Visible = false;
                        fundsCheckByInputGroup.Visible = true;
                        fundsCheckEmailBtn.Visible = false;
                        fundsCheckBySig.Text = item.Signature;
                        fundsCheckByDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "directorSupervisor":
                        directorSupervisorBtnDiv.Visible = false;
                        directorSupervisorInputGroup.Visible = true;
                        directorSupervisorEmailBtn.Visible = false;
                        directorSupervisorSig.Text = item.Signature;
                        directorSupervisorDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "cPA":
                        cPABtnDiv.Visible = false;
                        cPAInputGroup.Visible = true;
                        cPAEmailBtn.Visible = false;
                        cPASig.Text = item.Signature;
                        cPADate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "obmDirector":
                        obmDirectorBtnDiv.Visible = false;
                        obmDirectorInputGroup.Visible = true;
                        obmDirectorEmailBtn.Visible = false;
                        obmDirectorSig.Text = item.Signature;
                        obmDirectorDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "mayor":
                        mayorBtnDiv.Visible = false;
                        mayorInputGroup.Visible = true;
                        mayorEmailBtn.Visible = false;
                        mayorSig.Text = item.Signature;
                        mayorDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                }
            }

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "FadeOutOrdTable", "OrdTableFadeOut();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CurrencyFormatting", "CurrencyFormatting();", true);

            ordView.Visible = true;
            ordTable.Visible = false;
        }
        protected void rpAccountingTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string tableDesc = e.CommandArgument.ToString();
            List<Accounting> prvList = new List<Accounting>();
            List<Accounting> accountingList = new List<Accounting>();

            List<Accounting> removeAccs = new List<Accounting>();
            List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();

            switch (e.CommandName)
            {
                case "delete":

                    switch (tableDesc)
                    {
                        case "ordRevTable":
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                Accounting accountingItem = new Accounting();
                                var revItem = rpRevenueTable.Items[i];
                                HiddenField revHdnIDField = (HiddenField)revItem.FindControl("hdnRevID");
                                TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                                TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                                TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                                TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                                TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                                TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                                accountingItem.AccountingID = Convert.ToInt32(revHdnIDField.Value);
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = revFundCode.Text;
                                accountingItem.DepartmentCode = revAgencyCode.Text;
                                accountingItem.UnitCode = revOrgCode.Text;
                                accountingItem.ActivityCode = revActivityCode.Text;
                                accountingItem.ObjectCode = revObjectCode.Text;
                                if (revAmount.Text.Length == 0)
                                {

                                    accountingItem.Amount = CurrencyToDecimal("-1");
                                }
                                else
                                {
                                    accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                                }
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField revHdnIndex = (HiddenField)e.Item.FindControl("hdnRevIndex");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }


                            HiddenField revHdnID = (HiddenField)e.Item.FindControl("hdnRevID");

                            if (Session["RemoveAccs"] != null)
                            {
                                removeAccs = Session["RemoveAccs"] as List<Accounting>;
                            }
                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(revHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(revHdnID.Value);
                                Accounting acc = Factory.Instance.GetByID<Accounting>(accID, "sp_GetLkAccountingByAccountingID", "AccountingID");
                                removeAccs.Add(acc);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveAccs"] = removeAccs;
                            Session["RemoveOrdAccs"] = removeOrdAccs;

                            accountingList.RemoveAt(Convert.ToInt32(revHdnIndex.Value));
                            Session[tableDesc] = accountingList;
                            rpRevenueTable.DataSource = accountingList;
                            rpRevenueTable.DataBind();
                            break;
                        case "ordExpTable":
                            for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                            {
                                Accounting accountingItem = new Accounting();
                                var expItem = rpExpenditureTable.Items[i];
                                HiddenField expHdnIDField = (HiddenField)expItem.FindControl("hdnExpID");
                                TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                                TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                                TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                                TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                                TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                                TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                                accountingItem.AccountingID = Convert.ToInt32(expHdnIDField.Value);
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = expFundCode.Text;
                                accountingItem.DepartmentCode = expAgencyCode.Text;
                                accountingItem.UnitCode = expOrgCode.Text;
                                accountingItem.ActivityCode = expActivityCode.Text;
                                accountingItem.ObjectCode = expObjectCode.Text;
                                if (expAmount.Text.Length == 0)
                                {

                                    accountingItem.Amount = CurrencyToDecimal("-1");
                                }
                                else
                                {
                                    accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                                }
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField expHdnIndex = (HiddenField)e.Item.FindControl("hdnExpIndex");

                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }

                            HiddenField expHdnID = (HiddenField)e.Item.FindControl("hdnExpID");

                            if (Session["RemoveAccs"] != null)
                            {
                                removeAccs = Session["RemoveAccs"] as List<Accounting>;
                            }
                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(expHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(expHdnID.Value);
                                Accounting acc = Factory.Instance.GetByID<Accounting>(accID, "sp_GetLkAccountingByAccountingID", "AccountingID");
                                removeAccs.Add(acc);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveAccs"] = removeAccs;
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
        protected void rpSupportingDocumentation_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            HiddenField hdnDocID = (HiddenField)e.Item.FindControl("hdnDocID");
            HiddenField hdnDocIndex = (HiddenField)e.Item.FindControl("hdnDocIndex");
            List<OrdinanceDocument> ordDocList = Session["ordDocs"] as List<OrdinanceDocument>;
            OrdinanceDocument ordDocItem = ordDocList[Convert.ToInt32(hdnDocIndex.Value)];

            switch (e.CommandName)
            {
                case "download":
                    string delivery = HttpContext.Current.IsDebuggingEnabled ? "inline" : "attachment";
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Length", ordDocItem.DocumentData.Length.ToString());
                    Response.AddHeader("Content-type", MimeMapping.GetMimeMapping(ordDocItem.DocumentName));
                    Response.AddHeader("Content-Disposition", $"{delivery}; filename=" + ordDocItem.DocumentName);
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
                    removeDocs.Add(ordDocItem);
                    Session["RemoveDocs"] = removeDocs;
                    ordDocList.Remove(ordDocItem);
                    Session["ordDocs"] = ordDocList;
                    rpSupportingDocumentation.DataSource = ordDocList;
                    rpSupportingDocumentation.DataBind();
                    break;
            }
        }
        protected void rpAudit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HiddenField hdnAuditItem = (HiddenField)e.Item.FindControl("hdnAuditItem");
            int auditID = Convert.ToInt32(hdnAuditItem.Value);
            Repeater rpAuditDesc = (Repeater)e.Item.FindControl("rpAuditDesc");

            List<OrdinanceAudit> ordAuditList = Session["ordAudit"] as List<OrdinanceAudit>;
            OrdinanceAudit ordAudit = ordAuditList.First(i => i.AuditID.Equals(auditID));

            string[] descArray = ordAudit.Description.Split('|');
            List<string> descList = new List<string>();

            for (int i = 0; i < descArray.Length; i++)
            {
                if (!descArray[i].SubstringUpToFirst(':').Contains("Emergency Passage Justification") && !descArray[i].SubstringUpToFirst(':').Contains("Staff Analysis") && !descArray[i].SubstringUpToFirst(':').Contains("Suggested Title"))
                {
                    descList.Add(descArray[i]);
                }
                else
                {
                    string[] longTextArr = descArray[i].Split('\\');
                    string longText = string.Empty;
                    switch (!longTextArr[1].IsNullOrWhiteSpace())
                    {
                        case true:
                            longText = $"<p class='m-0'>{longTextArr[0]}</p> <div class='d-flex change-bg mw-100 lh-1p5'> <div class='w-50 pe-2'>{longTextArr[1]}</div> {longTextArr[2]} <div class='w-50 ps-2'>{longTextArr[3]}</div> </div>";
                            break;
                        case false:
                            longText = $"<p class='m-0'>{longTextArr[0]}</p> <div class='d-flex change-bg w-50 lh-1p5'> {longTextArr[2]} <div class='w-100 ps-2'>{longTextArr[3]}</div> </div>";
                            break;
                    }
                    descList.Add(longText);
                }
            }

            rpAuditDesc.DataSource = descList;
            rpAuditDesc.DataBind();
        }



        // ACCOUNTING TABLES //
        protected void NewAccountingRow(string tableDesc)
        {
            List<Accounting> prvList = new List<Accounting>();
            List<Accounting> accountingList = new List<Accounting>();
            Accounting newAccountingItem = new Accounting();
            newAccountingItem.Amount = CurrencyToDecimal("-1");

            switch (tableDesc)
            {
                case "ordRevTable":
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                        {
                            Accounting accountingItem = GetAccountingItem("revenue", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = (List<Accounting>)Session[tableDesc];
                    }
                    accountingList.Add(newAccountingItem);
                    Session[tableDesc] = accountingList;
                    rpRevenueTable.DataSource = accountingList;
                    rpRevenueTable.DataBind();
                    break;
                case "ordExpTable":
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                        {
                            Accounting accountingItem = GetAccountingItem("expenditure", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = (List<Accounting>)Session[tableDesc];
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
        protected Accounting GetAccountingItem(string tableDesc, int itemIndex)
        {
            Accounting accountingItem = new Accounting();
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
                    accountingItem.AccountingID = Convert.ToInt32(revHdnID.Value);
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
                    if (revAmount.Text.Length == 0)
                    {

                        accountingItem.Amount = CurrencyToDecimal("-1");
                    }
                    else
                    {
                        accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                    }
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
                    accountingItem.AccountingID = Convert.ToInt32(expHdnID.Value);
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
                    if (expAmount.Text.Length == 0)
                    {

                        accountingItem.Amount = CurrencyToDecimal("-1");
                    }
                    else
                    {
                        accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                    }
                    break;
            }
            return accountingItem;
        }
        


        // SUBMITS //
        protected void SaveFactSheet_Click(object sender, EventArgs e)
        {            
            Ordinance ordinance = new Ordinance();

            ordinance.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            ordinance.OrdinanceNumber = ordinanceNumber.Text;
            ordinance.RequestID = 0;
            ordinance.RequestDepartment = requestDepartment.SelectedItem.Text;
            ordinance.RequestContact = requestContact.Text;
            ordinance.RequestPhone = $"{requestPhone.Text}{requestExt.Text}";
            ordinance.RequestEmail = requestEmail.Text.ToLower();
            ordinance.FirstReadDate = Convert.ToDateTime(firstReadDate.Text);
            ordinance.EmergencyPassage = epYes.Checked;
            ordinance.EmergencyPassageReason = epJustification.Text ?? string.Empty;
            ordinance.OrdinanceFiscalImpact = CurrencyToDecimal(fiscalImpact.Text);
            ordinance.OrdinanceTitle = suggestedTitle.Text;
            ordinance.ContractVendorID = 0;
            ordinance.ContractVendorName = vendorName.Text;
            ordinance.ContractVendorNumber = vendorNumber.Text;
            ordinance.ContractStartDate = contractStartDate.Text;
            ordinance.ContractEndDate = contractEndDate.Text;
            ordinance.ContractTerm = contractTerm.Value;
            ordinance.ContractAmount = CurrencyToDecimal(contractAmount.Text);
            ordinance.ScopeChange = scYes.Checked;
            ordinance.ChangeOrderNumber = changeOrderNumber.Text ?? string.Empty;
            if (scYes.Checked)
            {
                ordinance.AdditionalAmount = CurrencyToDecimal(additionalAmount.Text);
            }
            else
            {
                ordinance.AdditionalAmount = CurrencyToDecimal("-1");
            }
            ordinance.ContractMethod = purchaseMethod.SelectedValue;
            ordinance.OtherException = otherException.Text ?? string.Empty;
            ordinance.PreviousOrdinanceNumbers = prevOrdinanceNums.Text;
            ordinance.CodeProvision = codeProvision.Text;
            ordinance.PAApprovalRequired = paApprovalRequiredYes.Checked;
            ordinance.PAApprovalIncluded = paApprovalAttachedYes.Checked;
            ordinance.OrdinanceAnalysis = staffAnalysis.Text;
            ordinance.LastUpdateBy = _user.Login;
            ordinance.LastUpdateDate = DateTime.Now;
            ordinance.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
            ordinance.ExpirationDate = DateTime.MaxValue;

            int retVal = Factory.Instance.Update(ordinance, "sp_UpdateOrdinance", 1);

            OrdinanceStatus ordStatus = new OrdinanceStatus();
            ordStatus.OrdinanceStatusID = Convert.ToInt32(hdnOrdStatusID.Value);
            ordStatus.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            try
            {
                ordStatus.StatusID = Convert.ToInt32(ddStatus.SelectedValue);
            }
            catch (Exception)
            {
                ordStatus.StatusID = Convert.ToInt32(hdnStatusID.Value);
            }
            ordStatus.LastUpdateBy = _user.Login;
            ordStatus.LastUpdateDate = DateTime.Now;
            ordStatus.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
            ordStatus.ExpirationDate = DateTime.MaxValue;
            ordinance.StatusDescription = ddStatus.SelectedItem.ToString();
            int statusVal = Factory.Instance.Update(ordStatus, "sp_UpdateOrdinance_Status", 1);

            int addDocsVal = new int();
            int addUploadedDocsVal = new int();
            List<OrdinanceDocument> ordDocs = Session["addOrdDocs"] as List<OrdinanceDocument>;
            if (Session["addOrdDocs"] != null)
            {
                foreach (OrdinanceDocument ordDoc in ordDocs)
                {
                    ordDoc.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                    addUploadedDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document");
                    //int ret = 1;
                    if (addUploadedDocsVal < 1)
                    {
                        break;
                    }
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
                    addDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document");
                    //int ret = 1;
                    if (addDocsVal < 1)
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
            if (Session["RemoveDocs"] != null)
            {
                removeDocs = Session["RemoveDocs"] as List<OrdinanceDocument>;
            }
            if (removeDocs.Count > 0)
            {
                foreach (OrdinanceDocument item in removeDocs)
                {
                    removeDocVal = Factory.Instance.Expire(item, "sp_UpdateOrdinance_Document");
                    if (removeDocVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                removeDocVal = 1;
            }

            int removeAccsVal = new int();
            int removeOrdAccsVal = new int();
            List<Accounting> removeAccs = new List<Accounting>();
            List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();
            if (Session["RemoveAccs"] != null)
            {
                removeAccs = Session["RemoveAccs"] as List<Accounting>;
            }
            if (Session["RemoveOrdAccs"] != null)
            {
                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
            }
            if (removeAccs.Count > 0)
            {
                foreach (Accounting item in removeAccs)
                {
                    item.LastUpdateBy = _user.Login;
                    item.LastUpdateDate = DateTime.Now;
                    item.EffectiveDate = DateTime.Now;
                    removeAccsVal = Factory.Instance.Expire(item, "sp_UpdatelkAccounting");

                    if (removeAccsVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                removeAccsVal = 1;
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
                        Accounting accountingItem = GetAccountingItem("revenue", i);
                        if (accountingItem.AccountingID > 0)
                        {
                            updateRevAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdatelkAccounting");
                        }
                        else
                        {
                            int ret = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                            if (ret > 0)
                            {
                                OrdinanceAccounting oaItem = new OrdinanceAccounting();
                                oaItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                                oaItem.AccountingID = ret;
                                oaItem.LastUpdateBy = _user.Login;
                                oaItem.LastUpdateDate = DateTime.Now;
                                oaItem.EffectiveDate = DateTime.Now;
                                oaItem.ExpirationDate = DateTime.MaxValue;
                                updateRevAccsVal = Factory.Instance.Insert(oaItem, "sp_InsertOrdinance_Accounting");
                            }
                            else
                            {
                                updateRevAccsVal = 0;
                                break;
                            }
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
                        Accounting accountingItem = GetAccountingItem("expenditure", i);
                        if (accountingItem.AccountingID > 0)
                        {
                            updateExpAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdatelkAccounting");
                        }
                        else
                         {
                            int ret = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                            if (ret > 0)
                            {
                                OrdinanceAccounting oaItem = new OrdinanceAccounting();
                                oaItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                                oaItem.AccountingID = ret;
                                oaItem.LastUpdateBy = _user.Login;
                                oaItem.LastUpdateDate = DateTime.Now;
                                oaItem.EffectiveDate = DateTime.Now;
                                oaItem.ExpirationDate = DateTime.MaxValue;
                                updateExpAccsVal = Factory.Instance.Insert(oaItem, "sp_InsertOrdinance_Accounting");
                            }
                            else
                            {
                                updateExpAccsVal = 0;
                                break;
                            }
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

            int insertSignatureVal = new int();
            if (Session["insertSigList"] != null)
            {
                List<OrdinanceSignature> insertSigList = (List<OrdinanceSignature>)Session["insertSigList"];
                foreach (OrdinanceSignature item in insertSigList)
                {
                    insertSignatureVal = Factory.Instance.Insert(item, "sp_InsertOrdinance_Signature", 2);
                }
            }
            else
            {
                insertSignatureVal = 1;
            }

            List<string> addEmailList = new List<string>()
            {
                _user.Email.ToLower(),
                ordinance.RequestEmail.ToLower()
            };
            foreach (string item in addEmailList)
            {
                Email.Instance.AddEmailAddress(emailList, item);
            }
            string formType = "Ordinance Fact Sheet";
            string href = $"apptest/Themis/Ordinances?id={hdnOrdID.Value}&v=view";
            string ordinanceNumInfo = !ordinanceNumber.Text.IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {ordinance.OrdinanceNumber}</span></p>" : string.Empty;

            Email newEmail = new Email();

            newEmail.EmailSubject = $"{formType} UPDATED";
            newEmail.EmailTitle = $"{formType} UPDATED";
            newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been UPDATED by <b>{_user.FirstName} {_user.LastName}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {ordinance.OrdinanceID}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {requestDepartment.SelectedItem.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {requestContact.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {ordinance.RequestPhone}</span></p><p><span>Status: {ordinance.StatusDescription}</span></p><br /><p><span>Please click the button below to review the document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";

            List<int> submitVals = new List<int>( new int[] {
                retVal,
                statusVal,
                removeDocVal,
                addDocsVal,
                addUploadedDocsVal,
                removeAccsVal,
                removeOrdAccsVal,
                updateRevAccsVal,
                updateExpAccsVal,
                insertSignatureVal
            });

            if (submitVals.All(i => i > 0))
            {
                if(Session["OriginalOrdinance"] != null)
                {
                    Ordinance oldOrd = Session["OriginalOrdinance"] as Ordinance;
                    PropertyInfo[] properties = typeof(Ordinance).GetProperties();
                    List<string> baseData = new List<string>()
                    {
                        "LastUpdateBy",
                        "LastUpdateDate",
                        "EffectiveDate",
                        "ExpirationDate",
                    };
                    foreach (PropertyInfo property in properties.Where(i => !i.GetValue(ordinance).Equals(i.GetValue(oldOrd)) && !baseData.Any(b => b.Contains(i.Name))))
                    {
                        if (property.GetValue(oldOrd) == null || property.GetValue(oldOrd).ToString() == string.Empty)
                        {
                            Debug.WriteLine($"{property.Name}: + {property.GetValue(ordinance)}");
                        }
                        else if (property.GetValue(ordinance) == null || property.GetValue(ordinance).ToString() == string.Empty)
                        {
                            Debug.WriteLine($"{property.Name}: - {property.GetValue(oldOrd)}");
                        }
                        else
                        {
                            Debug.WriteLine($"{property.Name}: {property.GetValue(oldOrd)} -> {property.GetValue(ordinance)}");
                        }
                            
                    }
                }
                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                Session["ToastMessage"] = "Form Saved!";
                Email.Instance.SendEmail(newEmail, emailList);
                Response.Redirect("./Ordinances");
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while saving!";
            }
        }
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            int ordID = Convert.ToInt32(hdnOrdID.Value);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            OrdinanceStatus ordStatus = new OrdinanceStatus();
            ordStatus.OrdinanceStatusID = Convert.ToInt32(hdnOrdStatusID.Value);
            ordStatus.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            ordStatus.StatusID = 7;
            ordStatus.LastUpdateBy = _user.Login;
            ordStatus.LastUpdateDate = DateTime.Now;
            ordStatus.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
            ordStatus.ExpirationDate = DateTime.MaxValue;
            int retVal = Factory.Instance.Expire<Ordinance>(ord, "sp_UpdateOrdinance", 1);
            if (retVal > 0)
            {
                int statusVal = Factory.Instance.Update(ordStatus, "sp_UpdateOrdinance_Status", 1);
                if (statusVal > 0)
                {
                    Session["SubmitStatus"] = "success";
                    Session["ToastColor"] = "text-bg-success";
                    Session["ToastMessage"] = "Entry Deleted!";
                    Response.Redirect("./Ordinances");
                }
                else
                {
                    Session["SubmitStatus"] = "error";
                    Session["ToastColor"] = "text-bg-danger";
                    Session["ToastMessage"] = "Something went wrong while deleting!";
                }
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
                Response.Redirect("./Ordinances");
            }
            else
            {
                ordTable.Visible = true;
                List<TextBox> sigTextBoxes = new List<TextBox>()
                {
                    fundsCheckBySig,
                    directorSupervisorSig,
                    cPASig,
                    obmDirectorSig,
                    mayorSig
                };
                foreach (TextBox item in sigTextBoxes)
                {
                    item.Text = string.Empty;
                }
                SaveFactSheet.CssClass = SaveFactSheet.CssClass.Replace(" emphasize", string.Empty);
                ordView.Visible = false;
            }
        }
        protected void copyOrd_Click(object sender, EventArgs e)
        {
            Response.Redirect($"./NewFactSheet?id={hdnOrdID.Value}");
        }
        

        public void GetTestAuditData()
        {
            List<OrdinanceAudit> testAudit = new List<OrdinanceAudit>()
            {
                new OrdinanceAudit
                {
                    AuditID = 1,
                    OrdinanceID = 0,
                    DateModified = Convert.ToDateTime("04/15/2025"),
                    ModifiedBy = "Kyle Bolinger",
                    ModificationType = "Updated",
                    Description = $"Suggested Title: \\ \\{addSymbol} \\AN ORDINANCE AUTHORIZING THE EXTENSION OF CONTRACT# PW21-21 WITH BLH COMPUTERS, INC. FOR COLLECTION, RECYCLING, AND DISPOSAL OF ELECTRONIC WASTE, IN AN AMOUNT NOT TO EXCEED $50,000.00 FOR THE OFFICE OF PUBLIC WORKS|Vendor Name: <span class='change-bg'>VI LLC {editSymbol} LRS</span>|Method of Purchase: <span class='change-bg'>Low Bid {editSymbol} Other</span>|Other/Exception: <span class='change-bg'>{addSymbol} RPF-100</span>"
                },
                new OrdinanceAudit
                {
                    AuditID = 2,
                    OrdinanceID = 0,
                    DateModified = Convert.ToDateTime("04/01/2025"),
                    ModifiedBy = "Chip McCrunch",
                    ModificationType = "Updated",
                    Description = $"First Read Date: <span class='change-bg'>03/25/2025 {editSymbol} 03/26/2025</span>|Requesting Contact: <span class='change-bg'>Chip McCrunch {editSymbol} Kyle Bolinger</span>|Ext: <span class='change-bg'>x5684 {editSymbol} x2811</span>|Fiscal Impact: <span class='change-bg'>$10,000.00 {editSymbol} $100,000.00</span>|Emergency Passage Justification: \\Vestibulum enim enim, vestibulum id consequat vitae, imperdiet at nisi. Duis rhoncus massa sit amet mi porta, bibendum dignissim lorem pretium. Suspendisse ultricies iaculis libero, sit amet rhoncus quam dictum sit amet. Praesent maximus vehicula tortor, bibendum auctor libero blandit ut. Quisque diam mi, finibus quis diam in, elementum finibus arcu. Donec congue rhoncus mauris. Suspendisse sit amet cursus augue. Sed aliquet arcu ac ante vehicula, a blandit ligula laoreet. Nunc vel diam auctor, aliquet nisl eget, venenatis sapien. Maecenas non leo ut felis maximus convallis eu sed nibh. Vestibulum nisl tortor, tempor quis ex ut, lobortis accumsan orci. Aenean scelerisque dictum nisi, at mattis nunc eleifend consectetur. Fusce semper metus sit amet dui mollis consectetur. Pellentesque lorem ipsum, iaculis quis lacinia et, ultricies eget nunc. \\{editSymbol} \\Vestibulum enim enim, vestibulum id consequat vitae, imperdiet at nisi. Duis rhoncus massa sit amet mi porta, bibendum dignissim lorem pretium. Suspendisse ultricies iaculis libero, sit amet rhoncus quam dictum sit amet. Praesent maximus vehicula tortor, bibendum auctor libero blandit ut. Quisque diam mi, finibus quis diam in, elementum finibus arcu. Donec congue rhoncus mauris. Suspendisse sit amet cursus augue. Sed aliquet arcu ac ante vehicula, a blandit ligula laoreet.|Code Provision: <span class='change-bg'>{removeSymbol} 56519813</span>"
                }
            };
            Session["ordAudit"] = testAudit;

            rpAudit.DataSource = testAudit;
            rpAudit.DataBind();
        }

        protected void btnSignDoc_Click(object sender, EventArgs e)
        {
            switch (sigType.Value)
            {
                case "fundsCheckBy":
                    fundsCheckByBtnDiv.Visible = false;
                    fundsCheckByInputGroup.Visible = true;
                    fundsCheckEmailBtn.Visible = false;
                    fundsCheckBySig.Text = sigName.Text;
                    fundsCheckByDate.Text = sigDate.Text;
                    break;
                case "directorSupervisor":
                    directorSupervisorBtnDiv.Visible = false;
                    directorSupervisorInputGroup.Visible = true;
                    directorSupervisorEmailBtn.Visible = false;
                    directorSupervisorSig.Text = sigName.Text;
                    directorSupervisorDate.Text = sigDate.Text;
                    break;
                case "cPA":
                    cPABtnDiv.Visible = false;
                    cPAInputGroup.Visible = true;
                    cPAEmailBtn.Visible = false;
                    cPASig.Text = sigName.Text;
                    cPADate.Text = sigDate.Text;
                    break;
                case "obmDirector":
                    obmDirectorBtnDiv.Visible = false;
                    obmDirectorInputGroup.Visible = true;
                    obmDirectorEmailBtn.Visible = false;
                    obmDirectorSig.Text = sigName.Text;
                    obmDirectorDate.Text = sigDate.Text;
                    break;
                case "mayor":
                    mayorBtnDiv.Visible = false;
                    mayorInputGroup.Visible = true;
                    mayorEmailBtn.Visible = false;
                    mayorSig.Text = sigName.Text;
                    mayorDate.Text = sigDate.Text;
                    break;
            }

            OrdinanceSignature signature = new OrdinanceSignature();
            signature.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            signature.SignatureType = sigType.Value;
            signature.Signature = sigName.Text;
            signature.DateSigned = Convert.ToDateTime(sigDate.Text);
            signature.SignatureCertified = certifySig.Checked;
            signature.LastUpdateBy = _user.Login;
            signature.LastUpdateDate = DateTime.Now;

            List<OrdinanceSignature> insertSigList = (List<OrdinanceSignature>)Session["insertSigList"] ?? new List<OrdinanceSignature>();
            insertSigList.Add(signature);
            Session["insertSigList"] = insertSigList;
            SaveFactSheet.CssClass += " emphasize";
        }
    }
}