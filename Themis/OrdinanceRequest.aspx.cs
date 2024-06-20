using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Themis
{
    public partial class OrdinanceRequest : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //Debug.WriteLine($"\n{dept_dd.SelectedItem.Value}");
            if (!Page.IsPostBack)
            {
                GetAllDepartments();
            }
            switch (dept_dd.SelectedItem.Value)
            {
                case "":
                    dept_dd.CssClass = "form-control gray-text";
                    break;
                default:
                    dept_dd.CssClass = "form-control";
                    break;
            }
        }

        protected void GetAllDepartments()
        {
            //dept_dd.Items.Insert(1, new ListItem("Select...", ""));
            dept_dd.Items.Insert(1, new ListItem("Budget and Management", "5"));
            dept_dd.Items.Insert(2, new ListItem("City Clerk", "13"));
            dept_dd.Items.Insert(3, new ListItem("City Council", "7"));
            dept_dd.Items.Insert(4, new ListItem("City Treasurer", "12"));
            dept_dd.Items.Insert(5, new ListItem("Community Relations", "16"));
            dept_dd.Items.Insert(6, new ListItem("Convention and Visitor's Bureau", "14"));
            dept_dd.Items.Insert(7, new ListItem("Corporation Counsel", "6"));
            dept_dd.Items.Insert(8, new ListItem("Fire Department", "4"));
            dept_dd.Items.Insert(9, new ListItem("Human Resources", "8"));
            dept_dd.Items.Insert(10, new ListItem("Lincoln Library", "15"));
            dept_dd.Items.Insert(11, new ListItem("Office of The Mayor", "10"));
            dept_dd.Items.Insert(12, new ListItem("Planning and Economic Development", "1"));
            dept_dd.Items.Insert(13, new ListItem("Police Department", "11"));
            dept_dd.Items.Insert(14, new ListItem("Public Utilities", "3"));
            dept_dd.Items.Insert(15, new ListItem("Public Works", "9"));

        }

        protected void dept_dd_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentClassAttr;
            switch (dept_dd.SelectedItem.Value)
            {
                case "":
                    currentClassAttr = div_div.Attributes["class"];
                    div_div.Attributes.Add("class", $"{currentClassAttr} disabled-control");
                    div_dd.Enabled = false;
                    break;
                default:
                    if (div_div.Attributes["class"].Contains("disabled-control"))
                    {
                        string[] currentClassAttrList = div_div.Attributes["class"].Split(' '); ;
                        string disabledClassAttr = currentClassAttrList[2];
                        currentClassAttr = div_div.Attributes["class"].Replace($" {disabledClassAttr}", "");
                        div_div.Attributes.Remove("class");
                        div_div.Attributes.Add("class", currentClassAttr);
                        div_dd.Enabled = true;
                    }
                    break;
            }
            
        }

        protected void div_dd_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}