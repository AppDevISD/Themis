using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DataLibrary
{
    public class TablePagination
    {
        public StateBag ViewState = new StateBag();
        public HtmlButton FirstBtn = new HtmlButton();
        public HtmlButton PreviousBtn = new HtmlButton();
        public HtmlButton NextBtn = new HtmlButton();
        public HtmlButton LastBtn = new HtmlButton();
        public Repeater TableRepeater = new Repeater();
        public Panel TablePanel = new Panel();
        public Label PageLabel = new Label();
        public int ItemsPerPg = new int();
        public static TablePagination _TablePagination = null;

        public static TablePagination Instance
        {
            get
            {
                if (_TablePagination == null)
                {
                    _TablePagination = new TablePagination();
                }
                return _TablePagination;
            }
        }
        public static void SetViewState(StateBag state, int ItemsPerPage)
        {
            Instance.ViewState = state;
            Instance.ItemsPerPg = ItemsPerPage;
        }
        public static void GetControls(HtmlButton firstButton, HtmlButton previousButton, HtmlButton nextButton, HtmlButton lastButton, Repeater tableRepeater, Panel tablePanel, Label pageLabel)
        {
            Instance.FirstBtn = firstButton;
            Instance.PreviousBtn = previousButton;
            Instance.NextBtn = nextButton;
            Instance.LastBtn = lastButton;
            Instance.TableRepeater = tableRepeater;
            Instance.TablePanel = tablePanel;
            Instance.PageLabel = pageLabel;
        }
        public static int SearchPgNumP
        {
            get
            {
                if (Instance.ViewState["PgNumP"] != null)
                    return Convert.ToInt32(Instance.ViewState["PgNumP"]);
                else return 1;
            }

            set
            {
                Instance.ViewState["PgNumP"] = value;
            }
        }
        public static int SearchPageCountP
        {
            get
            {
                if (Instance.ViewState["PageCountP"] != null)
                    return Convert.ToInt32(Instance.ViewState["PageCountP"]);
                else return 0;
            }
            set
            {
                Instance.ViewState["PageCountP"] = value;
            }
        }

        public static void PageButtonClick<T>(List<T> DataList, string command)
        {
            switch (command)
            {
                case "first":
                    SearchPgNumP = 1;
                    break;
                case "previous":
                    SearchPgNumP -= 1;
                    break;
                case "next":
                    SearchPgNumP += 1;
                    break;
                case "last":
                    SearchPgNumP = SearchPageCountP;
                    break;
            }
            BindDataRepeaterPagination("no", DataList);
        }

        public static void BindDataRepeaterPagination<T>(string isNewSearch, List<T> _list)
        {
            PagedDataSource pDSSearch = new PagedDataSource();
            pDSSearch.DataSource = _list;
            pDSSearch.AllowPaging = true;
            pDSSearch.PageSize = Instance.ItemsPerPg;
            if (isNewSearch == "yes")
            {
                SearchPgNumP = 1;
            }
            pDSSearch.CurrentPageIndex = SearchPgNumP;
            SearchPageCountP = pDSSearch.PageCount;
            Instance.PageLabel.Text = SearchPgNumP.ToString() + " of " + SearchPageCountP.ToString();
            if (_list.Count <= 0)
            {
                Instance.TablePanel.Visible = false; //false
            }
            else
            {
                if (pDSSearch.PageCount == 1)
                {
                    Instance.TablePanel.Visible = false; //false
                }
                else
                {
                    Instance.TablePanel.Visible = true;
                }
                pDSSearch.CurrentPageIndex = SearchPgNumP - 1;
                Instance.FirstBtn.Disabled = pDSSearch.IsFirstPage;
                Instance.LastBtn.Disabled = pDSSearch.IsLastPage;
                Instance.NextBtn.Disabled = pDSSearch.IsLastPage;
                Instance.PreviousBtn.Disabled = pDSSearch.IsFirstPage;
                Instance.TableRepeater.DataSource = pDSSearch;
                Instance.TableRepeater.DataBind();
            }
        }
    }
}
