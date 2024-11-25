using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebUI
{
    public partial class TableTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetStartupActives();
                Template tf = new Template();
                List<Template> tf_list = new List<Template>();
                tf_list = Factory.Instance.GetAllTemplateForms();

                if (tf_list.Count > 0)
                {
                    BindDataRepeaterSearch("yes", tf_list);
                }

                Session["tf_list"] = tf_list;
                int numItems = tf_list.Count;
            }
        }
        protected void SetStartupActives()
        {
            errorAlert.Visible = false;
        }
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            Debug.WriteLine(deleteID.Value);
            int FormID = Convert.ToInt32(deleteID.Value);
            int retVal = Factory.Instance.DeleteTemplateForm(FormID);
            if (retVal > 0)
            {
                List<Template> tf_list = new List<Template>();
                tf_list = Session["tf_list"] as List<Template>;
                tf_list = Factory.Instance.GetAllTemplateForms();
                rpTemplateForm.DataSource = tf_list;
                rpTemplateForm.DataBind();
                Session["tf_list"] = tf_list;
                Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
            }
            else
            {
                errorAlert.Visible = true;
                errorMsg.Text = "Could not delete entry. Something went wrong!";
            }
        }
        protected void mdlCancelBtn_ServerClick(object sender, EventArgs e)
        {
            //deleteModal.Hide();
        }

        public int SearchPgNumP
        {
            get
            {
                if (ViewState["PgNumP"] != null)
                    return Convert.ToInt32(ViewState["PgNumP"]);
                else return 1;
            }

            set
            {
                ViewState["PgNumP"] = value;
            }
        }
        public int SearchPageCountP
        {
            get
            {
                if (ViewState["PageCountP"] != null)
                    return Convert.ToInt32(ViewState["PageCountP"]);
                else return 0;
            }
            set
            {
                ViewState["PageCountP"] = value;
            }
        }
        protected void lnkFirstSearchP_Click(object sender, EventArgs e)
        {
            Template tf = new Template();
            List<Template> tf_list = new List<Template>();
            tf_list = (List<Template>)Session["tf_list"];
            SearchPgNumP = 1;
            BindDataRepeaterSearch("no", tf_list);
        }
        protected void lnkPreviousSearchP_Click(object sender, EventArgs e)
        {
            Template tf = new Template();
            List<Template> tf_list = new List<Template>();
            tf_list = (List<Template>)Session["tf_list"];
            SearchPgNumP -= 1;
            BindDataRepeaterSearch("no", tf_list);
        }
        protected void lnkNextSearchP_Click(object sender, EventArgs e)
        {
            Template tf = new Template();
            List<Template> tf_list = new List<Template>();
            tf_list = (List<Template>)Session["tf_list"];
            SearchPgNumP += 1;
            BindDataRepeaterSearch("no", tf_list);
        }
        protected void lnkLastSearchP_Click(object sender, EventArgs e)
        {
            Template tf = new Template();
            List<Template> tf_list = new List<Template>();
            tf_list = (List<Template>)Session["tf_list"];
            SearchPgNumP = SearchPageCountP;
            BindDataRepeaterSearch("no", tf_list);
        }
        protected void BindDataRepeaterSearch(string isNewSearch, List<Template> _list)
        {
            PagedDataSource pDSSearch = new PagedDataSource();
            pDSSearch.DataSource = _list;
            pDSSearch.AllowPaging = true;
            pDSSearch.PageSize = 10;
            if (isNewSearch == "yes")
            {
                SearchPgNumP = 1;
            }
            pDSSearch.CurrentPageIndex = SearchPgNumP;
            SearchPageCountP = pDSSearch.PageCount;
            lblCurrentPageBottomSearchP.Text = SearchPgNumP.ToString() + " of " + SearchPageCountP.ToString();
            if (_list.Count <= 0)
            {
                pnlPagingP.Visible = false; //false
            }
            else
            {
                if (pDSSearch.PageCount == 1)
                {
                    pnlPagingP.Visible = false; //false
                }
                else
                {
                    pnlPagingP.Visible = true;
                }
                pDSSearch.CurrentPageIndex = SearchPgNumP - 1;
                lnkFirstSearchP.Disabled = pDSSearch.IsFirstPage;
                lnkLastSearchP.Disabled = pDSSearch.IsLastPage;
                lnkNextSearchP.Disabled = pDSSearch.IsLastPage;
                lnkPreviousSearchP.Disabled = pDSSearch.IsFirstPage;
                rpTemplateForm.DataSource = pDSSearch;
                rpTemplateForm.DataBind();
            }
        }
        protected void CloseAlert_ServerClick(object sender, EventArgs e)
        {
            errorAlert.Visible = false;
            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }
    }
}