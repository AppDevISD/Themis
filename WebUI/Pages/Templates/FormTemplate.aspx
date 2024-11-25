<%@ Page Title="Form Template" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FormTemplate.aspx.cs" Inherits="WebUI.FormTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<section class="container form-header bg-body text-center">
		<div class="row h-100 align-items-center">
			<h1><span class="fas fa-file-lines"></span>&nbsp;Form Template</h1>
		</div>
	</section>
	<div class="container form-page bg-body-tertiary">
		<div class="px-2 py-4">
			<p class="text-justify">
				This is a template for a form. It provides many examples of fields that could be implemented in a form, including javascript for masking phone numbers and currency. To add required field styling, add 'required="true"' to the field element.
			</p>
			<p class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i>&nbsp;= Required Field</p>

			<div class="form-section">
				<div class="row mb-3">
					<div class="col-md-4">
						<div class="form-group">
							<label for="userName">User Name</label>
							<asp:TextBox runat="server" ID="userName" CssClass="form-control" TextMode="SingleLine" placeholder="John Doe" AutoCompleteType="DisplayName" required="true"></asp:TextBox>
						</div>
					</div>
					<div class="col-md-4">
						<div class="form-group">
							<label for="formNumber">Form Number</label>
							<asp:TextBox runat="server" ID="formNumber" CssClass="form-control" TextMode="SingleLine" placeholder="0A1B2C3" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
					<div class="col-md-4">
						<div class="form-group">
							<label for="number">Number</label>
							<asp:TextBox runat="server" ID="number" CssClass="form-control" TextMode="Number" placeholder="0123456789" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
				</div>
				<div class="row mb-3">
					<div class="col-md-12">
						<div class="form-group">
							<label for="textArea">Text Area</label>
							<asp:TextBox runat="server" ID="textArea" CssClass="form-control" TextMode="Multiline" Rows="8" AutoCompleteType="Disabled"></asp:TextBox>
						</div>
					</div>
				</div>
			</div>

			<div class="form-section">
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
			</div>


			<div class="form-section">
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
			</div>

			<div class="form-section">
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
			</div>

			<div class="form-section">
				<div class="row mb-3">
					<div class="col-md-3">
						<asp:Button runat="server" ID="SubmitNoFunForm" type="submit" CssClass="btn btn-primary" Text="Submit" Width="100%" OnClick="SubmitForm_Click" OnClientClick="showToast();" />
					</div>
					<div class="col-md-5 text-center float-end">

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
		const getStoredTheme = () => localStorage.getItem('showToast');
		document.addEventListener('DOMContentLoaded', function () {
			if (getStoredTheme() == 'show') {
				$('#submitToast').toast('show');
				localStorage.setItem('showToast', '');
			}
		});
		function showToast() {
			localStorage.setItem('showToast', 'show');
		}
	</script>
</asp:Content>
