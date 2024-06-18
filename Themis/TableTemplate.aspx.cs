using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Themis
{
    public partial class TableTemplate : System.Web.UI.Page
    {
        public string pageTitle;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TemplateForm tf = new TemplateForm();
                List<TemplateForm> tf_list = new List<TemplateForm>();
                tf_list = tf.GetAll();

                if (tf_list.Count > 0)
                {
                    BindDataRepeaterSearch("yes", tf_list);
                }

                Session["tf_list"] = tf_list;
                int numItems = tf_list.Count;
            }
            pageTitle = Title;
            Header.Title = pageTitle;
            //string scriptBlock = $"let pageTitleElement = document.getElementsByTagName('title');\nconsole.log(pageTitleElement)\npageTitleElement.innerText = '{pageTitle}';";
            //ScriptManager.RegisterStartupScript(this, GetType(), "testPageTitle", scriptBlock, true);
        }

        protected void rpCustomFormTickets_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            HiddenField hdnID = (HiddenField)e.Item.FindControl("hdnID");
            int FormID = Convert.ToInt32(hdnID.Value);
            TemplateForm tf = new TemplateForm();
            tf.FormID = FormID;
            tf = tf.Get();

            switch (e.CommandName)
            {
                case "delete":
                    Session["tf_FormID"] = FormID;
                    deleteModal.Show();
                    break;

                default:
                    break;
            }
        }
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            int FormID = Convert.ToInt32(Session["tf_FormID"]); ;
            TemplateForm tf = new TemplateForm();
            tf.FormID = FormID;

            int flag = tf.Delete();

            if (flag == -32)
            {
                Debug.Write("\nNope Delete\n");
            }
            else
            {
                Debug.Write("\nYep Delete\n");
            }

            List<TemplateForm> tf_list = new List<TemplateForm>();
            tf_list = Session["tf_list"] as List<TemplateForm>;
            tf_list = tf.GetAll();
            rpCustomFormTickets.DataSource = tf_list;
            rpCustomFormTickets.DataBind();

            Session["tf_list"] = tf_list;
            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }
        protected void mdlCancelBtn_ServerClick(object sender, EventArgs e)
        {
            deleteModal.Hide();
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
            TemplateForm tf = new TemplateForm();
            List<TemplateForm> tf_list = new List<TemplateForm>();
            tf_list = (List<TemplateForm>)Session["tf_list"];
            SearchPgNumP = 1;
            BindDataRepeaterSearch("no", tf_list);
        }
        protected void lnkPreviousSearchP_Click(object sender, EventArgs e)
        {
            TemplateForm tf = new TemplateForm();
            List<TemplateForm> tf_list = new List<TemplateForm>();
            tf_list = (List<TemplateForm>)Session["tf_list"];
            SearchPgNumP -= 1;
            BindDataRepeaterSearch("no", tf_list);
        }
        protected void lnkNextSearchP_Click(object sender, EventArgs e)
        {
            TemplateForm tf = new TemplateForm();
            List<TemplateForm> tf_list = new List<TemplateForm>();
            tf_list = (List<TemplateForm>)Session["tf_list"];
            SearchPgNumP += 1;
            BindDataRepeaterSearch("no", tf_list);
        }
        protected void lnkLastSearchP_Click(object sender, EventArgs e)
        {
            TemplateForm tf = new TemplateForm();
            List<TemplateForm> tf_list = new List<TemplateForm>();
            tf_list = (List<TemplateForm>)Session["tf_list"];
            SearchPgNumP = SearchPageCountP;
            BindDataRepeaterSearch("no", tf_list);
        }

        protected void BindDataRepeaterSearch(string isNewSearch, List<TemplateForm> _list)
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
                lnkFirstSearchP.Enabled = !pDSSearch.IsFirstPage;
                lnkLastSearchP.Enabled = !pDSSearch.IsLastPage;
                lnkNextSearchP.Enabled = !pDSSearch.IsLastPage;
                lnkPreviousSearchP.Enabled = !pDSSearch.IsFirstPage;
                rpCustomFormTickets.DataSource = pDSSearch;
                rpCustomFormTickets.DataBind();
            }
        }
    }
}