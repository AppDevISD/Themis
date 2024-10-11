using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI
{
    public partial class OrdinanceRequestForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetAllPurchaseMethods();
                requestDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }


        protected void GetAllPurchaseMethods()
        {
            purchaseMethod.Items.Insert(0, new ListItem("Select Purchase Method...", "N/A"));
            purchaseMethod.Items.Insert(1, new ListItem("Low Bid", "1"));
            purchaseMethod.Items.Insert(2, new ListItem("Low Bid Meeting Specs", "2"));
            purchaseMethod.Items.Insert(3, new ListItem("Low Evaluated Bid", "3"));
            purchaseMethod.Items.Insert(4, new ListItem("Other", "4"));
        }

        protected void PurchaseMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            string currentClassAttr;
            switch (purchaseMethod.SelectedItem.Value)
            {
                default:
                    currentClassAttr = otherMethodDiv.Attributes["class"];
                    otherMethodDiv.Attributes.Add("class", $"{currentClassAttr} disabled-control");
                    otherMethod.Enabled = false;
                    otherMethod.Text = string.Empty;
                    break;
                case "4":
                    if (otherMethodDiv.Attributes["class"].Contains("disabled-control"))
                    {
                        currentClassAttr = otherMethodDiv.Attributes["class"].Replace($" disabled-control", "");
                        otherMethodDiv.Attributes.Remove("class");
                        otherMethodDiv.Attributes.Add("class", currentClassAttr);
                        otherMethod.Enabled = true;
                    }
                    break;
            }
        }

    }
}