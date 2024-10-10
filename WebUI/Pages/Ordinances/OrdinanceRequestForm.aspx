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
			<div class="form-section">
				<div class="row mb-2">
					<div class="col-3 col-sm-2 col-md-3">
						<div class="form-group mb-3">
							<label for="contractNumber">Contract Number</label>
							<input runat="server" id="contractNumber" type="text" class="form-control" placeholder="John Doe" autocomplete="off">
						</div>
					</div>
					<div class="col-6 col-sm-3 col-md-6">
						<div class="form-group mb-3">
							<label for="contractTitle">Title <span class="required-field">*</span></label>
							<input runat="server" id="contractTitle" type="text" class="form-control" placeholder="John Doe" autocomplete="off" required>
						</div>
					</div>
					<div class="col-2 col-sm-2 col-md-2">
						<div class="form-group mb-3">
							<label for="requestDate">Date <span class="required-field">*</span></label>
							<input runat="server" ID="requestDate" type="date" class="form-control" required value="" />
						</div>
					</div>
				</div>
				<div class="row mb-2">
					<div class="col-6 col-sm-3 col-md-6">
						<div class="form-group mb-3">
							<label for="originatorName">Originator <span class="required-field">*</span></label>
							<input runat="server" id="originatorName" type="text" class="form-control" placeholder="John Doe" autocomplete="off" required>
						</div>
					</div>
					<div class="col-6 col-sm-3 col-md-6">
						<div class="form-group mb-3">
							<label for="divisionHeadName">Division Head <span class="required-field">*</span></label>
							<input runat="server" id="divisionHeadName" type="text" class="form-control" placeholder="John Doe" autocomplete="off" required>
						</div>
					</div>
				</div>
				<div class="row mb-2">
					
				</div>
			</div>
		</div>
	</div>
</asp:Content>