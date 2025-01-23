using DataLibrary;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Timers;
using static DataLibrary.TablePagination;

namespace WebUI
{
    public partial class Ordinances : System.Web.UI.Page
    {
        public string toastColor;
        public string toastMessage;        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                SetStartupActives();
                SetPagination(rpOrdinanceTable, 10);
                GetStartupData();
            }
            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                LinkButton editButton = item.FindControl("editOrd") as LinkButton;
                LinkButton viewButton = item.FindControl("viewOrd") as LinkButton;
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(editButton);
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(viewButton);
            }
            SubmitStatus();
        }
        protected void SetStartupActives()
        {
            ordView.Visible = false;
        }
        protected void SetPagination(Repeater rpTable, int ItemsPerPage)
        {
            SetViewState(ViewState, ItemsPerPage);
            GetControls(lnkFirstSearchP, lnkPreviousSearchP, lnkNextSearchP, lnkLastSearchP, rpTable, pnlPagingP, lblCurrentPageBottomSearchP);
        }
        protected void GetStartupData()
        {
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
            if (ord_list.Count > 0)
            {
                BindDataRepeaterPagination("yes", ord_list);
            }

            Session["ord_list"] = ord_list;
        }
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            int FormID = Convert.ToInt32(deleteID.Value);
            int retVal = Factory.Instance.Delete<Ordinance>(FormID, "Ordinance");
            if (retVal > 0)
            {
                List<Ordinance> ord_list = new List<Ordinance>();
                ord_list = Session["ord_list"] as List<Ordinance>;
                ord_list = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                rpOrdinanceTable.DataSource = ord_list;
                rpOrdinanceTable.DataBind();
                Session["ord_list"] = ord_list;
                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                Session["ToastMessage"] = "Entry Deleted!";
                Response.Redirect("./Ordinances");
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while submitting!";
            }
        }
        protected void paginationBtn_Click(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = (List<Ordinance>)Session["ord_list"];
            HtmlButton button = (HtmlButton)sender;
            string commandName = button.Attributes["data-command"];
            PageButtonClick(ord_list, commandName);
        }
        protected void SubmitStatus()
        {
            if (Session["SubmitStatus"] != null || (string)Session["SubmitStatus"] == "success")
            {
                toastColor = (string)Session["ToastColor"];
                toastMessage = (string)Session["ToastMessage"];
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while submitting!";
                toastColor = (string)Session["ToastColor"];
                toastMessage = (string)Session["ToastMessage"];
            }
        }

        protected void rpOrdinanceTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            
            switch (e.CommandName)
            {
                case "view":
                    ordView.Attributes["readonly"] = "true";
                    ordTable.Attributes["class"] = ordTable.Attributes["class"].Replace("show", "fade-out");
                    ordTable.Visible = false;
                    ordView.Visible = true;
                    ordView.Attributes["class"] = ordView.Attributes["class"].Replace("fade-out", "fade-in show");
                    break;
                case "edit":
                    ordView.Attributes["readonly"] = "false";
                    ordTable.Attributes["class"] = ordTable.Attributes["class"].Replace("show", "fade-out");
                    ordTable.Visible = false;
                    ordView.Visible = true;
                    ordView.Attributes["class"] = ordView.Attributes["class"].Replace("fade-out", "fade-in show");
                    break;
            }
        }


        protected void SetFormReadOnly(bool readOnly)
        {
            //requestContact.ReadOnly
        }
    }
}