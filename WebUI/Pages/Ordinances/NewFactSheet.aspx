<%@ Page Title="New Fact Sheet" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewFactSheet.aspx.cs" Inherits="WebUI.NewFactSheet" MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="container" style="background: url('../../assets/images/backgrounds/scales-bg.png') no-repeat center center !important; height: 225px; box-shadow: 5px 5px 5px 0px #0000007b; position: relative; z-index: 10;">
		<div class="overlay dark-7">
			<div class="display-table">
				<div class="display-table-cell align-middle">
					<div class="container text-center">
						<h1 class="nomargin size-50 weight-300" style="color: white;"><i class="fas fa-gavel"></i>&nbsp;Ordinance Fact Sheet</h1>
					</div>
				</div>
			</div>
		</div>
	</section>
	<div class="container form-page">
       <div class="col-md-12">
           <div>
               <br />
               <p class="text-justify">
                   Et tellus suspendisse suscipit orci sit amet sem venenatis nec lobortis sem suscipit nullam nec imperdiet velit mauris eu nisi a felis imperdiet porta at ac nulla vivamus faucibus felis nec dolor pretium eget pellentesque dolor suscipit maecenas vitae enim arcu, at tincidunt nunc pellentesque eleifend vulputate lacus, vel semper sem ornare sit amet proin sem sapien, auctor vel faucibus id, aliquet vitae ipsum etiam auctor ultricies ante, at dapibus urna viverra sed nullam mi arcu, tempor vitae interdum a.
               </p>
               <p class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i> = Required Field</p>

               <div class="row border-top border-bottom pt-4 mt-1">
                   <div class="col-12 col-sm-6 col-md-5">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="department">Office Requesting <span class="required-field">*</span></label>
                            <asp:DropDownList ID="department" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DepartmentSelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <br />
                    </div>
                   <div class="col-12 col-sm-6 col-md-5">
                        <br />
                        <div id="divisionDiv" class="form-label-group mb-3 disabled-control" runat="server">
                            <label for="division" >Division</label>
                            <asp:DropDownList ID="division" runat="server" AutoPostBack="false" CssClass="form-control" Enabled="false">
                                <asp:ListItem selected="true">Select Division...</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <br />
                    </div>
                   <div class="col-6 col-sm-6 col-md-2">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="firstReadingDate">Date of 1st Reading <span class="required-field">*</span></label>
                            <input runat="server" id="firstReadingDate" type="date" class="form-control" autocomplete="off" required>
                        </div>
                        <br />
                    </div>
                   <div class="col-6 col-sm-4 col-md-8">
                       <br />
                       <div class="form-label-group mb-3">
                           <label for="contactName">Contact Name <span class="required-field">*</span></label>
                           <input runat="server" id="contactName" type="text" class="form-control" placeholder="John Doe" autocomplete="off" required>
                       </div>
                       <br />
                   </div>
                   <div class="col-6 col-sm-4 col-md-2">
                       <br />
                       <div class="form-label-group mb-3">
                           <label for="tel1">Phone Number <span class="required-field">*</span></label>
                           <input runat="server" id="tel1" type="tel" data-type="telephone" class="form-control" placeholder="(555) 555-5555" minlength="14" maxlength="14" autocomplete="off" required>
                       </div>
                       <br />
                   </div>
                   <div class="col-12 col-sm-1 col-md-1">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="ext1">Ext</label>
                            <input runat="server" id="ext1" type="text" data-type="extension" class="form-control" placeholder="x1234" minlength="5" maxlength="5" autocomplete="off">
                        </div>
                        <br />
                    </div>
                   <div class="col-12 col-sm-1 col-md-1">
                       <br />
                       <div class="form-label-group mb-3">
                           <label for="addNewContact">&nbsp;</label>
                           <asp:Button ID="addNewContact" runat="server" class="form-control green-btn-form" Text="+" AutoPostBack="true"  CausesValidation="false"></asp:Button> <%--ONCLICK--%>
                       </div>
                       <br />
                    </div>
               </div>
               <div class="row border-bottom">
                   <div class="col-12 col-sm-6 col-md-12">
                       <div class="form-label-group mb-3">
                           <label for="epGroup">Emergency Passage <span class="required-field">*</span></label>
                           <div id="epGroup">
                               <asp:RadioButton runat="server" ID="epYes" GroupName="epGroup" Text="Yes" CssClass="radio-btn" OnCheckedChanged="EPGroupCheckedChanged" AutoPostBack="true" />
                                <asp:RadioButton runat="server" ID="epNo" GroupName="epGroup" Text="No" CssClass="radio-btn" Checked="true" OnCheckedChanged="EPGroupCheckedChanged" AutoPostBack="true" />
                           </div>
                           <asp:Label runat="server" ID="epLabel" Visible="false"><label for="epExplanation" style="margin-top: 14px !important;">If Yes, Explain Justification - See Attached Document <span class="required-field">*</span></label></asp:Label>
                           <asp:TextBox ID="epExplanation" runat="server" TextMode="MultiLine" Rows="6" CssClass="form-control" Enabled="false" Visible="false" />
                       </div>
                       <br />
                   </div>
               </div>
               <div class="row border-bottom">
                    <div class="col-6 col-sm-4 col-md-3">
                        <div class="form-label-group mb-3">
                            <label for="fiscalImpact">Fiscal Impact <span class="required-field">*</span></label>
                            <input runat="server" id="fiscalImpact" type="text" data-type="currency" class="form-control" placeholder="$100,000.00" autocomplete="off" required>
                        </div>
                        <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-12">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="suggestedTitle">Suggested Title <span class="required-field">*</span></label>
                            <asp:TextBox ID="suggestedTitle" runat="server" TextMode="MultiLine" Rows="12" CssClass="form-control" />
                        </div>
                        <br />
                    </div>
               </div>
               <div class="row border-bottom">
                    <div class="col-6 col-sm-4 col-md-10">
                        <div class="form-label-group mb-3">
                            <label for="vendorName">Vendor Name <span class="required-field">*</span></label>
                            <input runat="server" id="vendorName" type="text" class="form-control" placeholder="Vendor McVenderson & Associates" autocomplete="off" required>
                        </div>
                        <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-2">
                        <div class="form-label-group mb-3">
                            <label for="vendorNumber">Vendor Number <span class="required-field">*</span></label>
                            <input runat="server" id="vendorNumber" type="text" class="form-control" placeholder="123456789" autocomplete="off" required>
                        </div>
                        <br />
                    </div>
                    <div class="col-6 col-sm-6 col-md-3">
                         <br />
                         <div class="form-label-group mb-3">
                             <label for="termStartDate">Start Date <span class="required-field">*</span></label>
                             <input runat="server" id="termStartDate" type="date" data-type="termStart" class="form-control" autocomplete="off" required>
                         </div>
                         <br />
                    </div>
                    <div class="col-6 col-sm-6 col-md-3">
                         <br />
                         <div class="form-label-group mb-3">
                             <label for="termEndDate">End Date <span class="required-field">*</span></label>
                             <input runat="server" id="termEndDate" type="date" data-type="termEnd" class="form-control" autocomplete="off" required>
                         </div>
                         <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-2">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="contractTerm">Contract Term</label>
                            <input runat="server" id="contractTerm" type="text" data-type="contractTerm" class="form-control locked-field" autocomplete="off" disabled="disabled" value="" placeholder="Calculating Term..." required>
                        </div>
                        <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-1">
                        <br />
                        <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-3">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="contractAmount">Contract Amount <span class="required-field">*</span></label>
                            <input runat="server" id="contractAmount" type="text" data-type="currency" class="form-control" placeholder="$100,000.00" autocomplete="off" required>
                        </div>
                        <br />
                    </div>
                </div>
               <div class="row border-bottom">
                   <div class="col-12 col-sm-6 col-md-12">
                       <div class="form-label-group mb-3">
                           <label for="scopeGroup">Change In Scope <span class="required-field">*</span></label>
                           <div id="scopeGroup">
                               <asp:RadioButton runat="server" ID="scopeYes" GroupName="scopeGroup" Text="Yes" CssClass="radio-btn" OnCheckedChanged="ScopeGroupCheckedChanged" AutoPostBack="true" />
                                <asp:RadioButton runat="server" ID="scopeNo" GroupName="scopeGroup" Text="No" CssClass="radio-btn" OnCheckedChanged="ScopeGroupCheckedChanged" Checked="true" AutoPostBack="true" />
                           </div>
                       </div>
                       <br />
                   </div>
                   <div class="col-12 col-sm-6 col-md-3">
                        <div id="changeOrderDiv" class="form-label-group mb-3 disabled-control" runat="server">
                            <label for="changeOrderNumber">Change Order Number <span class="required-field">*</span></label>
                            <asp:TextBox ID="changeOrderNumber" runat="server" TextMode="SingleLine" CssClass="form-control" AutoCompleteType="None" placeholder="123456789" Enabled="false" />
                        </div>
                        <br />
                    </div>
                   <div class="col-12 col-sm-6 col-md-3">
                        <div id="additionalAmountDiv" class="form-label-group mb-3 disabled-control" runat="server">
                            <label for="additionalAmount">Additional Amount <span class="required-field">*</span></label>
                            <asp:TextBox ID="additionalAmount" runat="server" data-type="currency" TextMode="SingleLine" CssClass="form-control" AutoCompleteType="None" placeholder="$100,000.00" Enabled="false" />
                        </div>
                        <br />
                    </div>
               </div>
               <div class="row border-bottom">
                    <div class="col-6 col-sm-4 col-md-6">
                        <div class="form-label-group mb-3">
                            <label for="purchaseMethod">Method of Purchase <span class="required-field">*</span></label>
                            <asp:DropDownList ID="purchaseMethod" runat="server" OnSelectedIndexChanged="PurchaseMethodSelectedIndexChanged" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-3">
                        <div id="otherExceptionDiv" class="form-label-group mb-3 disabled-control" runat="server">
                            <label for="otherException">Other/Exception <span class="required-field">*</span></label>
                            <asp:TextBox ID="otherException" runat="server" TextMode="SingleLine" CssClass="form-control" AutoCompleteType="None" Enabled="false" />
                        </div>
                        <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-4">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="previousOrdinanceNumbers">Previous Ordinance Numbers</label>
                            <input runat="server" id="previousOrdinanceNumbers" type="text" data-type="ordinanceNumbers" class="form-control" placeholder="123-45-6789" autocomplete="off">
                        </div>
                        <br />
                    </div>
                    <div class="col-6 col-sm-4 col-md-4">
                        <br />
                        <div class="form-label-group mb-3">
                            <label for="codeProvision">Code Provision <span class="required-field">*</span></label>
                            <input runat="server" id="codeProvision" type="text" class="form-control" placeholder="123456789" autocomplete="off">
                        </div>
                        <br />
                    </div>
                </div>
               <div class="row border-bottom">
                   <div class="col-6 col-sm-6 col-md-12">
                       <div class="form-label-group mb-3">
                           <label for="purchaseAARequiredGroup">Is Purchasing Agent Approval Required? <span class="required-field">*</span></label>
                           <div id="purchaseAARequiredGroup">
                               <asp:RadioButton runat="server" ID="purchaseAARequiredYes" GroupName="purchaseAARequiredGroup" Text="Yes" CssClass="radio-btn" />
                                <asp:RadioButton runat="server" ID="purchaseAARequiredNo" GroupName="purchaseAARequiredGroup" Text="No" CssClass="radio-btn" Checked="true" />
                           </div>
                       </div>
                       <br />
                    </div>
                   <div class="col-6 col-sm-6 col-md-12">
                       <div class="form-label-group mb-3">
                           <label for="purchaseAAAtachedGroup">Is Purchasing Agent Approval Attached? <span class="required-field">*</span></label>
                           <div id="purchaseAAAtachedGroup">
                                <asp:RadioButton runat="server" ID="purchaseAAAtachedYes" GroupName="purchaseAAAtachedGroup" Text="Yes" CssClass="radio-btn" />
                                <asp:RadioButton runat="server" ID="purchaseAAAtachedNo" GroupName="purchaseAAAtachedGroup" Text="No" CssClass="radio-btn" Checked="true" />
                           </div>
                       </div>
                       <br />
                   </div>
                </div>
               <div class="row border-bottom">
                    <div class="col-6 col-sm-4 col-md-12">
                        <div class="form-label-group mb-3">
                            <label for="supportingDocumentation">Supporting Documentation (Ex: Contract, Agreement, Change Order, Bid Book) <span class="required-field">*</span></label>
                            <asp:FileUpload runat="server" ID="supportingDocumentation" AllowMultiple="true" />
                        </div>
                        <br />
                    </div>
                   <br />
                    <div class="col-6 col-sm-4 col-md-12">
                        <div class="form-label-group mb-3">
                            <label for="staffAnalysis">Staff Analysis <span class="required-field">*</span></label>
                            <asp:TextBox ID="staffAnalysis" runat="server" TextMode="MultiLine" Rows="18" CssClass="form-control" />
                        </div>
                        <br />
                    </div>
               </div>
               <div class="row">
                   <div class="col-12">
                       <div class="alert alert-success noborder text-center weight-400 nomargin noradius" id="divSuccess" visible="false" runat="server" style="font-weight: 900 !important;">
                           Request Submitted!
                       </div>
                       <div class="pt-4 mt-1">
                           <br />
                           <asp:Button ID="TemplateFormSubmit" runat="server" type="submit" class="btn btn-primary btn-adjust" Text="Submit"></asp:Button> <%--ONCLICK--%>
                       </div>
                   </div>
               </div>
               <br />
               <br />
           </div>
       </div>
   </div>
</asp:Content>
