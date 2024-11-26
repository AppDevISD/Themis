<%@ Page Title="New Fact Sheet" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewFactSheet.aspx.cs" Inherits="WebUI.NewFactSheet" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<section class="container form-header bg-body text-center">
		<div class="row h-100 align-items-center">
			<h1><span class="fas fa-file-circle-plus"></span>&nbsp;New Fact Sheet</h1>
		</div>
	</section>
	<div class="container form-page bg-body-tertiary">
		<div class="px-2 py-4">
			<p class="text-justify">
			</p>
			<p class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i>&nbsp;= Required Field</p>

			<%-- FIRST SECTION --%>
			<div class="form-section">
				<%-- FIRST ROW --%>
				<div class="row mb-3">
					<%-- DEPARTMENT --%>
					<div class="col-md-6">
						<div class="form-group">
							<label for="requestDepartment">Requesting Department</label>
							<asp:DropDownList ID="requestDepartment" runat="server" AutoPostBack="true" CssClass="form-select" required="true" ValidateRequestMode="Enabled"></asp:DropDownList>
						</div>
					</div>

					<%-- BLANK SPACE --%>
					<div class="col-md-4"></div>

					<%-- 1ST READ DATE --%>
					<div class="col-md-2">
						<div class="form-group">
							<label for="firstReadDate">Date of 1<sup>st</sup> Reading</label>
							<asp:TextBox runat="server" ID="firstReadDate" CssClass="form-control" TextMode="Date" required="true"></asp:TextBox>
						</div>
					</div>
				</div>

				<%-- SECOND ROW --%>
				<div class="row mb-3">
					<%-- CONTACT --%>
					<div class="col-md-6">
						<div class="form-group">
							<label for="requestContact">Requesting Contact</label>
							<asp:TextBox runat="server" ID="requestContact" CssClass="form-control" TextMode="SingleLine" placeholder="John Doe" AutoCompleteType="DisplayName" required="true"></asp:TextBox>
						</div>
					</div>

					<%-- BLANK SPACE --%>
					<div class="col-md-2"></div>

					<%-- PHONE NUMBER / EXTENSION --%>
					<div class="col-md-4">
						<%-- LABELS --%>
						<div class="input-group w-100">
							<label for="requestPhone" style="flex: 1 1 auto !important">Phone Number</label>
							<label for="requestExt" style="flex: 0.32 1 auto !important">Ext</label>
						</div>

						<%-- INPUTS --%>
						<div class="input-group">
							<%-- PHONE NUMBER --%>
							<asp:TextBox runat="server" ID="requestPhone" CssClass="form-control" TextMode="Phone" data-type="telephone" placeholder="(555) 555-5555" AutoCompleteType="Disabled" required="true"></asp:TextBox>

							<%-- EXTENSION --%>
							<asp:TextBox runat="server" ID="requestExt" CssClass="form-control ext-split" TextMode="SingleLine" data-type="extension" placeholder="x1234" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>

			<%-- SECOND SECTION --%>
			<div class="form-section">
				<%-- FIRST ROW --%>
				<div class="row mb-3">
					<%-- EMERGENCY PASSAGE --%>
					<div class="col-md-12">
						<div class="form-group">
							<label for="epList">Emergency Passage</label>
							<div class="radioListDiv" id="epList" required="true">
								<div class="form-check form-check-inline">
									<label for="epYes">Yes</label>
									<asp:RadioButton runat="server" ID="epYes" CssClass="form-check-input" GroupName="epList" OnCheckedChanged="EPCheckedChanged" AutoPostBack="true" />
								</div>
								<div class="form-check form-check-inline">
									<label for="epNo">No</label>
									<asp:RadioButton runat="server" ID="epNo" CssClass="form-check-input" GroupName="epList" OnCheckedChanged="EPCheckedChanged" AutoPostBack="true" Checked="true" />
								</div>
							</div>
						</div>
					</div>
				</div>

				<%-- SECOND ROW --%>
				<div class="row mb-3" runat="server" id="epJustificationGroup">
					<%-- JUSTIFICATION --%>
					<div class="col-md-12">
						<div class="form-group">
							<label for="epJustification">If Yes, Explain Justification - See Attached Document</label>
							<asp:TextBox runat="server" ID="epJustification" CssClass="form-control" TextMode="Multiline" Rows="8" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>

			<%-- THIRD SECTION --%>
			<div class="form-section">
				<%-- FIRST ROW --%>
				<div class="row mb-3">
					<%-- FISCAL IMPACT --%>
					<div class="col-md-2">
						<div class="form-group">
							<label for="fiscalImpact">Fiscal Impact</label>
							<asp:TextBox runat="server" ID="fiscalImpact" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$100,000.00" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
				</div>

				<%-- SECOND ROW --%>
				<div class="row mb-3">
					<%-- SUGGESTED TITLE --%>
					<div class="col-md-12">
						<label for="suggestedTitle">Suggested Title</label>
						<asp:TextBox runat="server" ID="suggestedTitle" CssClass="form-control" TextMode="Multiline" Rows="12" AutoCompleteType="Disabled" required="true"></asp:TextBox>
					</div>
				</div>
			</div>

			<%-- FOURTH SECTION --%>
			<div class="form-section">
				<%-- FIRST ROW --%>
				<div class="row mb-3">
					<%-- VENDOR NAME --%>
					<div class="col-md-10">
						<div class="form-group">
							<label for="vendorName">Vendor Name</label>
							<asp:TextBox runat="server" ID="vendorName" CssClass="form-control" TextMode="SingleLine" placeholder="Vendor Incorporated LLC" AutoCompleteType="Company" required="true"></asp:TextBox>
						</div>
					</div>

					<%-- VENDOR NUMBER --%>
					<div class="col-md-2">
						<div class="form-group">
							<label for="vendorNumber">Vendor Number</label>
							<asp:TextBox runat="server" ID="vendorNumber" CssClass="form-control" TextMode="SingleLine" placeholder="0123456789" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
				</div>

				<%-- SECOND ROW --%>
				<div class="row mb-3">
					<%-- DATE PERIOD --%>
					<div class="col-md-6">
						<div class="form-group">
							<label for="datePeriod">Date Period</label>
							<div id="datePeriod" class="input-group">
								<%-- START --%>
								<asp:TextBox runat="server" ID="datePeriodStart" CssClass="form-control" TextMode="Date" data-type="datePeriodStart"></asp:TextBox>

								<%-- SEPARATOR --%>
								<div class="input-group-append">
									<span class="input-group-text date-period-separator"><i class="fas fa-minus"></i></span>
								</div>

								<%-- END --%>
								<asp:TextBox runat="server" ID="datePeriodEnd" CssClass="form-control" TextMode="Date" data-type="datePeriodEnd"></asp:TextBox>
							</div>
						</div>
					</div>

					<%-- DATE TERM --%>
					<div class="col-md-3">
						<div class="form-group">
							<label for="dateTerm">Date Term</label>
							<input runat="server" id="dateTerm" type="text" data-type="dateTerm" class="form-control locked-field" autocomplete="off" readonly="readonly" value="" placeholder="Calculating Term..." required>
						</div>
					</div>

					<%-- CONTRACT AMOUNT --%>
					<div class="col-md-3">
						<div class="form-group">
							<label for="contractAmount">Contract Amount</label>
							<asp:TextBox runat="server" ID="contractAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$100,000.00" AutoCompleteType="Disabled" required="true"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>

			<%-- FIFTH SECTION --%>
			<div class="form-section">
				<%-- FIRST ROW --%>
				<div class="row mb-3">
					<%-- CHANGE IN SCOPE --%>
					<div class="col-md-12">
						<div class="form-group">
							<label for="scopeChangeList">Change In Scope</label>
							<div class="radioListDiv" id="scopeChangeList" required="true">
								<div class="form-check form-check-inline">
									<label for="scYes">Yes</label>
									<asp:RadioButton runat="server" ID="scYes" CssClass="form-check-input" GroupName="scopeChangeList" OnCheckedChanged="SCCheckedChanged" AutoPostBack="true" />
								</div>
								<div class="form-check form-check-inline">
									<label for="scNo">No</label>
									<asp:RadioButton runat="server" ID="scNo" CssClass="form-check-input" GroupName="scopeChangeList" OnCheckedChanged="SCCheckedChanged" AutoPostBack="true" Checked="true" />
								</div>
							</div>
						</div>
					</div>
				</div>

				<%-- SECOND ROW --%>
				<div class="row mb-3">
					<%-- CHANGE ORDER NUMBER --%>
					<div class="col-md-10">
						<label for="changeOrderNumber">Change Order Number</label>
						<asp:TextBox runat="server" ID="changeOrderNumber" CssClass="form-control" TextMode="SingleLine" placeholder="0123456789" AutoCompleteType="Disabled"></asp:TextBox>
					</div>
					
					<%-- ADDITIONAL AMOUNT --%>
					<div class="col-md-2">
						<div class="form-group">
							<label for="additionalAmount">Additional Amount</label>
							<asp:TextBox runat="server" ID="additionalAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$100,000.00" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>

			<%-- SIXTH SECTION --%>
			<div class="form-section">
				<%-- FIRST ROW --%>
				<div class="row mb-3">
					<%-- PURCHASE METHOD --%>
					<div class="col-md-4">
						<div class="form-group">
							<label for="purchaseMethod">Method of Purchase</label>
							<asp:DropDownList ID="purchaseMethod" runat="server" OnSelectedIndexChanged="PurchaseMethodSelectedIndexChanged" AutoPostBack="true" CssClass="form-select" required="true"></asp:DropDownList>
						</div>
					</div>

					<%-- OTHER / EXCEPTION --%>
					<div class="col-md-4">
						<div id="dropdownOtherDiv" class='form-group'>
							<label for="otherException">Other/Exception</label>
							<asp:TextBox runat="server" ID="otherException" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>

			<%--<div class="form-section">
				<div class="row mb-3">
					<div class="col-md-3">
						<div class="form-group">
							<label for="telephone">Telephone</label>
							<asp:TextBox runat="server" ID="telephone" CssClass="form-control" TextMode="Phone" data-type="telephone" placeholder="(555) 555-5555" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
					<div class="col-md-1">
						<div class="form-group">
							<label for="extension">Extension</label>
							<asp:TextBox runat="server" ID="extension" CssClass="form-control" TextMode="SingleLine" data-type="extension" placeholder="x1234" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
					<div class="col-md-2">
						<div class="form-group">
							<label for="currency">Currency</label>
							<asp:TextBox runat="server" ID="currency" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
				</div>
				<div class="row mb-3">
					<div class="col-md-3">
						<div class="form-group">
							<label for="date">Date</label>
							<asp:TextBox runat="server" ID="date" CssClass="form-control" TextMode="Date"></asp:TextBox>
						</div>
					</div>
					<div class="col-md-3">
						<div class="form-group">
							<label for="dateTime">Date Time</label>
							<asp:TextBox runat="server" ID="dateTime" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
						</div>
					</div>
					<div class="col-md-4">
						<div class="form-group">
							<label for="datePeriod">Date Period</label>
							<div id="datePeriod" class="input-group">
								<asp:TextBox runat="server" ID="datePeriodStart" CssClass="form-control" TextMode="Date" data-type="datePeriodStart"></asp:TextBox>
								<div class="input-group-append">
									<span class="input-group-text date-period-separator"><i class="fas fa-minus"></i></span>
								</div>
								<asp:TextBox runat="server" ID="datePeriodEnd" CssClass="form-control" TextMode="Date" data-type="datePeriodEnd"></asp:TextBox>
							</div>
						</div>
					</div>
					<div class="col-md-2">
						<div class="form-group">
							<label for="dateTerm">Date Term</label>
							<input runat="server" id="dateTerm" type="text" data-type="dateTerm" class="form-control locked-field" autocomplete="off" disabled="disabled" value="" placeholder="Calculating Term..." required>
						</div>
					</div>
				</div>
			</div>--%>

			<%--<div class="form-section">
				<div class="row mb-3">
					<div class="col-md-4">
						<div class="form-group">
							<label for="dropdown">Dropdown</label>
							<asp:DropDownList ID="dropdown" runat="server" OnSelectedIndexChanged="DropdownSelectedIndexChanged" AutoPostBack="true" CssClass="form-select"></asp:DropDownList>
						</div>
					</div>
					<div class="col-md-4">
						<div id="dropdownOtherDiv" class='form-group <%:dropdown.SelectedItem.Value =="Other"?"":"disabled-control"%>'>
							<label for="dropdownOther">Other</label>
							<asp:TextBox runat="server" ID="dropdownOther" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>--%>

			<%--<div class="form-section">
				<div class="row mb-3">
					<div class="col-md-2">
						<div class="form-group">
							<label for="single">Single Checkbox</label>
							<div class="checkboxListDiv" id="single">
								<div class="form-check form-check-inline">
									<label for="checkbox">Option</label>
									<asp:CheckBox runat="server" ID="checkbox" CssClass="form-check-input" />
								</div>
							</div>
						</div>
					</div>
					<div class="col-md-3">
						<div class="form-group">
							<label for="multiple">Multiple Checkboxes</label>
							<div class="checkboxListDiv" id="multiple">
								<div class="form-check form-check-inline">
									<label for="checkbox1">Option 1</label>
									<asp:CheckBox runat="server" ID="checkbox1" CssClass="form-check-input" />
								</div>
								<div class="form-check form-check-inline">
									<label for="checkbox2">Option 2</label>
									<asp:CheckBox runat="server" ID="checkbox2" CssClass="form-check-input" />
								</div>
							</div>
						</div>
					</div>
					<div class="col-md-3">
						<div class="form-group">
							<label for="radioList">Radios</label>
							<div class="radioListDiv" id="radioList">
								<div class="form-check form-check-inline">
									<label for="radio1">Option 1</label>
									<asp:RadioButton runat="server" ID="radio1" CssClass="form-check-input" GroupName="radioList1" />
								</div>
								<div class="form-check form-check-inline">
									<label for="radio2">Option 2</label>
									<asp:RadioButton runat="server" ID="radio2" CssClass="form-check-input" GroupName="radioList1" />
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>--%>

			<div class="form-section">
				<div class="row mt-3 mb-3 text-center">
					<div class="col-md-12">
						<asp:Button runat="server" ID="SubmitNoFunForm" type="submit" CssClass="btn btn-primary" Width="25%" Text="Submit" OnClick="SubmitForm_Click" OnClientClick="showToast();" />
					</div>
				</div>
			</div>
		</div>
	</div>
	<div class="toast-container position-fixed bottom-0 end-0 p-3">
		<div id="submitToast" class='toast <%:toastColor%> border-0 fade-slide-in' role="alert" aria-live="assertive" aria-atomic="true" data-delay="10000" data-animation="true">
			<div class="d-flex">
				<div class="toast-body"><%:toastMessage%></div>
				<button type="button" class="btn-close btn-close-white me-2 m-auto" data-dismiss="toast" aria-label="Close"></button>
			</div>
		</div>
	</div>

	<script>
		const getStoredToast = () => localStorage.getItem('showToast');
		document.addEventListener('DOMContentLoaded', function () {
			if (getStoredToast() == 'show') {
				$('#submitToast').toast('show');
				localStorage.setItem('showToast', '');
			}
		});
		function showToast() {
			localStorage.setItem('showToast', 'show');
		}
	</script>
</asp:Content>
