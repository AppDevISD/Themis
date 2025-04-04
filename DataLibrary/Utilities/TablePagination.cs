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
using System.Security.AccessControl;
using System.Reflection;

namespace DataLibrary
{
    public class TablePagination
    {
        public StateBag ViewState = new StateBag();
        public LinkButton FirstBtn = new LinkButton();
        public LinkButton PreviousBtn = new LinkButton();
        public LinkButton NextBtn = new LinkButton();
        public LinkButton LastBtn = new LinkButton();
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
        public static void GetControls(LinkButton firstButton, LinkButton previousButton, LinkButton nextButton, LinkButton lastButton, Repeater tableRepeater, Panel tablePanel, Label pageLabel)
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

        public static Dictionary<string, object> SortButtonClick<T>(List<T> DataList, string command, string argument)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            PropertyInfo property = typeof(T).GetProperty(command);            
            switch (argument)
            {
                case "asc":
                    DataList = (from n in DataList orderby property.GetValue(n, null) descending
                                select n).ToList();
                    ret.Add("dir", "desc");
                    ret.Add("arrow", "down");
                    ret.Add("curDir", "asc");
                    break;
                case "desc":
                    DataList = (from n in DataList
                                orderby property.GetValue(n, null) ascending
                                select n).ToList();
                    ret.Add("dir", "asc");
                    ret.Add("arrow", "up");
                    ret.Add("curDir", "desc");
                    break;
            }
            
            BindDataRepeaterPagination("no", DataList);
            ret.Add("list", DataList);
            ret.Add("curCmd", command);
            return ret;
        }

        public static List<Ordinance> FilterList(List<Ordinance> DataList, string command, string argument)
        {
            string ret = string.Empty;
            List<Ordinance> newList = new List<Ordinance>();
            switch (!argument.Contains("Select"))
            {
                case true:
                    foreach (Ordinance item in DataList.Where(o => o.RequestDepartment.Equals(argument)))
                    {
                        newList.Add(item);
                    }
                    DataList = newList;
                    break;
                case false:
                    foreach (Ordinance item in DataList)
                    {
                        newList.Add(item);
                    }
                    DataList = newList;
                    break;
            }
            BindDataRepeaterPagination("no", DataList);
            return newList;
        }

        public static void BindDataRepeaterPagination<T>(string isNewSearch, List<T> _list)
        {
            PagedDataSource pDSSearch = new PagedDataSource();
            pDSSearch.DataSource = _list;
            pDSSearch.AllowPaging = true;
            pDSSearch.PageSize = Instance.ItemsPerPg;
            if (isNewSearch == "yes" || _list.Count <= Instance.ItemsPerPg)
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
                Instance.FirstBtn.Enabled = !pDSSearch.IsFirstPage;
                Instance.LastBtn.Enabled = !pDSSearch.IsLastPage;
                Instance.NextBtn.Enabled = !pDSSearch.IsLastPage;
                Instance.PreviousBtn.Enabled = !pDSSearch.IsFirstPage;
                Instance.TableRepeater.DataSource = pDSSearch;
                Instance.TableRepeater.DataBind();
            }
        }
    }
}
