<%@ Page Title="Ordinance Request Form" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrdinanceRequestForm.aspx.cs" Inherits="WebUI.OrdinanceRequestForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<section class="container" style="height: 225px; box-shadow: 5px 5px 5px 0px #0000007b; position: relative; z-index: 10;">
		<div class="overlay dark-7">
			<div class="display-table">
				<div class="display-table-cell align-middle">
					<div class="container text-center">
						<h1 class="nomargin size-50 weight-300" style="color: white;"><i class="fas fa-gavel"></i>&nbsp;Ordinance Request Form</h1>
					</div>
				</div>
			</div>
		</div>
	</section>
	<div class="container form-page">
		<div class="px-2 py-4">
			<p class="text-justify">
				Et tellus suspendisse suscipit orci sit amet sem venenatis nec lobortis sem suscipit nullam nec imperdiet velit mauris eu nisi a felis imperdiet porta at ac nulla vivamus faucibus felis nec dolor pretium eget pellentesque dolor suscipit maecenas vitae enim arcu, at tincidunt nunc pellentesque eleifend vulputate lacus, vel semper sem ornare sit amet proin sem sapien, auctor vel faucibus id, aliquet vitae ipsum etiam auctor ultricies ante, at dapibus urna viverra sed nullam mi arcu, tempor vitae interdum a.
			</p>
			<p class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i> = Required Field</p>
			<%-- CONTACT & CONTRACT --%>
			<div class="form-section">
				<%-- CONTACT --%>
				<div class="row mb-3">
					<div class="col-5 col-sm-3 col-md-5">
						<div class="form-group">
							<label for="originatorName">Originator <span class="required-field">*</span></label>
							<asp:TextBox runat="server" ID="originatorName" CssClass="form-control" TextMode="SingleLine" placeholder="John Doe" AutoCompleteType="DisplayName" required="true"></asp:TextBox>
						</div>
					</div>
					<div class="col-5 col-sm-3 col-md-5">
						<div class="form-group">
							<label for="divisionHeadName">Division Head <span class="required-field">*</span></label>
							<asp:TextBox runat="server" ID="divisionHeadName" CssClass="form-control" TextMode="SingleLine" placeholder="Jane Doe" AutoCompleteType="DisplayName" required="true"></asp:TextBox>
						</div>
					</div>
					<div class="col-2 col-sm-2 col-md-2">
						<div class="form-group">
							<label for="requestDate">Date <span class="required-field">*</span></label>
							<asp:TextBox runat="server" ID="requestDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
						</div>
					</div>
				</div>
				<%-- CONTRACT --%>
				<div class="row mb-3">
					<div class="col-3 col-sm-2 col-md-3">
						<div class="form-group">
							<label for="contractNumber">Contract Number</label>
							<asp:TextBox runat="server" ID="TextBox1" CssClass="form-control" TextMode="SingleLine" placeholder="0123456789" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
					<div class="col-7 col-sm-4 col-md-7">
						<div class="form-group">
							<label for="contractTitle">Title <span class="required-field">*</span></label>
							<asp:TextBox runat="server" ID="contractTitle" CssClass="form-control" TextMode="SingleLine" placeholder="ex: Equipment Replacement Upgrade" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
					<div class="col-2 col-sm-2 col-md-2">
						<div class="form-group">
							<label for="engineerEstimate">Engineer Estimate</label>
							<asp:TextBox runat="server" ID="engineerEstimate" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
				</div>
				<div class="row mb-3">
					<div class="col-12">
						<div class="form-group">
							<label for="contractDescription">Description & Comments <span class="required-field">*</span></label>
							<%--<textarea runat="server" id="contractDescription" type="text" rows="8" class="form-control" autocomplete="off" required></textarea>--%>
							<asp:TextBox runat="server" ID="contractDescription" CssClass="form-control" TextMode="Multiline" Rows="8" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>
			<%-- VENDOR --%>
			<div class="form-section">
				<div class="row mb-3">
					<div class="col-3 col-sm-2 col-md-3">
						<div class="form-group">
							<label for="vendorNumber">Vendor Number</label>
							<input runat="server" id="vendorNumber" type="text" class="form-control" placeholder="0123456789" autocomplete="off" />
						</div>
					</div>
					<div class="col-7 col-sm-3 col-md-7">
						<div class="form-group">
							<label for="vendorName">Vendor Name</label>
							<input runat="server" id="vendorName" type="text" class="form-control" placeholder="Vendor McVenderson & Associates" autocomplete="off">
						</div>
					</div>
					<div class="col-2 col-sm-2 col-md-2">
						<div class="form-group">
							<label for="contractAmount">Amount</label>
							<input runat="server" id="contractAmount" type="text" data-type="currency" class="form-control" placeholder="$0.00" autocomplete="off">
						</div>
					</div>
				</div>
			</div>
			<%-- PURCHASE --%>
			<div class="form-section">
				<div class="row mb-3">
					<div class="col-4 col-sm-2 col-md-4">
						<div class="form-group">
							<label for="bidPeriod">Bid Period</label>
							<div id="bidPeriod" class="input-group">
								<asp:TextBox runat="server" ID="bidPeriodStart" CssClass="form-control" TextMode="Date"></asp:TextBox>
								<div class="input-group-append">
									<span class="input-group-text" style="background-color: transparent; border: none; color: var(--text-color)"><i class="fas fa-minus"></i></span>
								</div>
								<asp:TextBox runat="server" ID="bidPeriodEnd" CssClass="form-control" TextMode="Date"></asp:TextBox>
							</div>
						</div>
					</div>
					<div class="col-4 col-sm-2 col-md-4">
						<div class="form-group">
							<label for="purchaseMethod">Method of Purchase <span class="required-field">*</span></label>
							<asp:DropDownList ID="purchaseMethod" runat="server" OnSelectedIndexChanged="PurchaseMethodSelectedIndexChanged" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
						</div>
					</div>
					<div class="col-4 col-sm-2 col-md-4">
						<div id="otherMethodDiv" class="form-group disabled-control" runat="server">
							<label for="otherMethod">Other <span class="required-field">*</span></label>
							<asp:TextBox runat="server" ID="otherMethod" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" required="true" Enabled="false"></asp:TextBox>
						</div>
					</div>
				</div>
				<div class="row mb-3">
					<div class="col-4 col-sm-2 col-md-4">
						<div class="form-group">
							<label for="ordinanceTotal">Ordinance Total</label>
							<asp:TextBox runat="server" ID="ordinanceTotal" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
					<div class="col-4 col-sm-2 col-md-4">
						<div class="form-group">
							<label for="materialTotal">Material Total</label>
							<asp:TextBox runat="server" ID="materialTotal" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
					<div class="col-4 col-sm-2 col-md-4">
						<div class="form-group">
							<label for="laborTotal">Labor Total</label>
							<asp:TextBox runat="server" ID="laborTotal" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>