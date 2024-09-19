using DataLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI
{
    public partial class NewFactSheet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetAllDepartments();
                GetAllPurchaseMethods();
            }
            switch (department.SelectedItem.Value)
            {
                case "N/A":
                    department.CssClass = "form-control gray-text";
                    break;
                default:
                    department.CssClass = "form-control";
                    break;
            }
        }

        protected void GetAllDepartments()
        {
            department.Items.Insert(0, new ListItem("Select Department...", "N/A"));
            department.Items.Insert(1, new ListItem("Budget and Management", "5"));
            department.Items.Insert(2, new ListItem("City Clerk", "13"));
            department.Items.Insert(3, new ListItem("City Council", "7"));
            department.Items.Insert(4, new ListItem("City Treasurer", "12"));
            department.Items.Insert(5, new ListItem("Community Relations", "16"));
            department.Items.Insert(6, new ListItem("Convention and Visitor's Bureau", "14"));
            department.Items.Insert(7, new ListItem("Corporation Counsel", "6"));
            department.Items.Insert(8, new ListItem("Fire Department", "4"));
            department.Items.Insert(9, new ListItem("Human Resources", "8"));
            department.Items.Insert(10, new ListItem("Lincoln Library", "15"));
            department.Items.Insert(11, new ListItem("Office of The Mayor", "10"));
            department.Items.Insert(12, new ListItem("Planning and Economic Development", "1"));
            department.Items.Insert(13, new ListItem("Police Department", "11"));
            department.Items.Insert(14, new ListItem("Public Utilities", "3"));
            department.Items.Insert(15, new ListItem("Public Works", "9"));
        }

        protected void DepartmentSelectedIndexChanged(object sender, EventArgs e)
        {
            List<Division> lst = new List<Division>();
            switch (department.SelectedValue)
            {
                case "N/A":
                    lst = Factory.Instance.LoadDivisionsByDept(0);
                    break;

                default:
                    lst = Factory.Instance.LoadDivisionsByDept(Convert.ToInt32(department.SelectedValue));
                    break;
            }

            string currentClassAttr;
            switch (department.SelectedItem.Value)
            {
                case "N/A":
                    currentClassAttr = divisionDiv.Attributes["class"];
                    divisionDiv.Attributes.Add("class", $"{currentClassAttr} disabled-control");
                    division.Enabled = false;
                    break;
                default:
                    if (divisionDiv.Attributes["class"].Contains("disabled-control") && lst.Count > 0)
                    {
                        string[] currentClassAttrList = divisionDiv.Attributes["class"].Split(' ');
                        string disabledClassAttr = currentClassAttrList[2];
                        currentClassAttr = divisionDiv.Attributes["class"].Replace($" {disabledClassAttr}", "");
                        divisionDiv.Attributes.Remove("class");
                        divisionDiv.Attributes.Add("class", currentClassAttr);
                        division.Enabled = true;
                    }
                    else if (lst.Count <= 0)
                    {
                        currentClassAttr = divisionDiv.Attributes["class"];
                        divisionDiv.Attributes.Add("class", $"{currentClassAttr} disabled-control");
                        division.Enabled = false;
                    }
                    break;
            }

            division.DataSource = lst;
            division.DataTextField = "div_name";
            division.DataValueField = "div_code";
            division.DataBind();
            division.Items.Insert(0, "Select Division...");
            division.Items[0].Value = "N/A";
            division.Focus();
        }

        protected void EPGroupCheckedChanged(object sender, EventArgs e)
        {
            switch (epYes.Checked)
            {
                case true:
                    epExplanation.Visible = true;
                    epExplanation.Enabled = true;
                    epLabel.Visible = true;
                    break;

                case false:
                    epExplanation.Visible = false;
                    epExplanation.Enabled = false;
                    epLabel.Visible = false;
                    break;
            }
        }

        protected void ScopeGroupCheckedChanged(object sender, EventArgs e)
        {
            string currentChangeOrderDivAttr;
            string currentAdditionalAmountDivAttr;
            switch (scopeYes.Checked)
            {
                case true:
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;

                    if (changeOrderDiv.Attributes["class"].Contains("disabled-control"))
                    {
                        string[] currentClassAttrList = changeOrderDiv.Attributes["class"].Split(' ');
                        string disabledClassAttr = currentClassAttrList[2];
                        currentChangeOrderDivAttr = changeOrderDiv.Attributes["class"].Replace($" {disabledClassAttr}", "");
                        changeOrderDiv.Attributes.Remove("class");
                        changeOrderDiv.Attributes.Add("class", currentChangeOrderDivAttr);
                    }
                    if (additionalAmountDiv.Attributes["class"].Contains("disabled-control"))
                    {
                        string[] currentClassAttrList = additionalAmountDiv.Attributes["class"].Split(' ');
                        string disabledClassAttr = currentClassAttrList[2];
                        currentAdditionalAmountDivAttr = additionalAmountDiv.Attributes["class"].Replace($" {disabledClassAttr}", "");
                        additionalAmountDiv.Attributes.Remove("class");
                        additionalAmountDiv.Attributes.Add("class", currentAdditionalAmountDivAttr);
                    }
                    break;

                case false:
                    changeOrderNumber.Enabled = false;
                    additionalAmount.Enabled = false;
                    currentChangeOrderDivAttr = changeOrderDiv.Attributes["class"];
                    changeOrderDiv.Attributes.Add("class", $"{currentChangeOrderDivAttr} disabled-control");
                    currentAdditionalAmountDivAttr = additionalAmountDiv.Attributes["class"];
                    additionalAmountDiv.Attributes.Add("class", $"{currentAdditionalAmountDivAttr} disabled-control");
                    break;
            }
        }

        protected void GetAllPurchaseMethods()
        {
            purchaseMethod.Items.Insert(0, new ListItem("Select Purchase Method...", "N/A"));
            purchaseMethod.Items.Insert(1, new ListItem("Low Bid", "1"));
            purchaseMethod.Items.Insert(2, new ListItem("Low Bid Meeting Specs", "2"));
            purchaseMethod.Items.Insert(3, new ListItem("Low Evaluated Bid", "3"));
            purchaseMethod.Items.Insert(4, new ListItem("Other", "4"));
            purchaseMethod.Items.Insert(5, new ListItem("Exception", "5"));
        }

        protected void PurchaseMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            string currentClassAttr;
            switch (purchaseMethod.SelectedItem.Value)
            {
                default:
                    currentClassAttr = otherExceptionDiv.Attributes["class"];
                    otherExceptionDiv.Attributes.Add("class", $"{currentClassAttr} disabled-control");
                    otherException.Enabled = false;
                    break;
                case "4":
                case "5":
                    if (otherExceptionDiv.Attributes["class"].Contains("disabled-control"))
                    {
                        string[] currentClassAttrList = otherExceptionDiv.Attributes["class"].Split(' ');
                        string disabledClassAttr = currentClassAttrList[2];
                        currentClassAttr = otherExceptionDiv.Attributes["class"].Replace($" {disabledClassAttr}", "");
                        otherExceptionDiv.Attributes.Remove("class");
                        otherExceptionDiv.Attributes.Add("class", currentClassAttr);
                        otherException.Enabled = true;
                    }
                    break;
            }
        }
        
    }
}