<%@ Page Title="Fact Sheet Drafts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FactSheetDrafts.aspx.cs" Inherits="WebUI.FactSheetDrafts" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server"> 
	<%-- PAGE CONTENT --%>
	<section>
		<asp:UpdatePanel runat="server" ID="pnlDraftsTable" UpdateMode="Always" class="overlap-panels">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="backBtn" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkFirstSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkPreviousSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkNextSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkLastSearchP" EventName="Click" />

				<asp:AsyncPostBackTrigger ControlID="btnFilterSearchTitle" EventName="Click" />

				<asp:AsyncPostBackTrigger ControlID="epYes" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="epNo" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="scYes" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="scNo" EventName="CheckedChanged" />

				<asp:AsyncPostBackTrigger ControlID="requestDepartment" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="requestDivision" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="purchaseMethod" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="signatureEmailAddress" EventName="TextChanged" />

				<asp:AsyncPostBackTrigger ControlID="sortDate" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortTitle" EventName="Click" />

				<asp:AsyncPostBackTrigger ControlID="AddRequestEmailAddress" EventName="Click" />

				<asp:PostBackTrigger ControlID="UploadDocBtn" />
				<asp:PostBackTrigger ControlID="SubmitFactSheet" />
				<asp:PostBackTrigger ControlID="SaveFactSheet" />

				<asp:AsyncPostBackTrigger ControlID="lnkInactivityRefresh" EventName="Click" />
			</Triggers>

			<ContentTemplate>
				<div runat="server" id="draftsTable" class="card">
					<div class="card-header bg-body-secondary d-flex justify-content-between">
						<h3><i class="fas fa-pen-ruler"></i>&nbsp;Drafts</h3>
						<asp:LinkButton runat="server" ID="BtnNewFactSheet" CssClass="btn btn-success float-end" OnClick="BtnNewFactSheet_Click" OnClientClick="showLoadingModal();"><span class="fas fa-plus"></span>&nbsp;New Fact Sheet</asp:LinkButton>
					</div>
					<div class="card-body bg-body-tertiary">
						<%-- FILTERS & SORTING --%>
						<div class="row mb-4">
							<div class="col-md-3" id="filterSearchTitleDiv">
								<div class="form-group">
									<label for="filterSearchTitleGroup">Search Title</label>
									<div id="filterSearchTitleGroup" class="input-group">
										<asp:TextBox runat="server" ID="filterSearchTitle" CssClass="form-control" TextMode="SingleLine" placeholder="Search..." AutoCompleteType="Disabled" data-enter-btn="btnFilterSearchTitle"></asp:TextBox>
										<asp:LinkButton runat="server" ID="btnFilterSearchTitle" CssClass="btn input-group-text" OnClick="Filter_SelectedIndexChanged" OnClientClick="showLoadingModal();" TabIndex="-1"><span class="fas fa-magnifying-glass"></span></asp:LinkButton>
									</div>
								</div>
							</div>
						</div>

						<%-- TABLE --%>
						<div runat="server" id="formTableDiv">
							<table id="FormTable" class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
								<thead>
									<tr>
										<th style="width: 15%; text-align: center"><asp:LinkButton runat="server" ID="sortDate" data-command="LastUpdateDate" data-text="Last Updated" OnClick="SortBtn_Click" class="btn btn-sort" TabIndex="-1" data-disable-btn="aspIconBtn" data-disable-btn-symbol="true"><strong>Last Updated<span runat="server" class='float-end lh-1p5 fas fa-arrow-down'></span></strong></asp:LinkButton></th>
										<th style="width: 100%; text-align: center"><asp:LinkButton runat="server" ID="sortTitle" data-command="OrdinanceTitle" data-text="Title" OnClick="SortBtn_Click" class="btn btn-sort" TabIndex="-1" data-disable-btn="aspIconBtn" data-disable-btn-symbol="true"><strong>Title<span class="float-end lh-1p5 me-1"></span></strong></asp:LinkButton></th>
										<th style="width: 6%; text-align: center"><strong>Action</strong></th>
									</tr>
								</thead>
								<asp:Repeater runat="server" ID="rpDraftsTable" OnItemCommand="rpDraftsTable_ItemCommand">
									<HeaderTemplate></HeaderTemplate>
									<ItemTemplate>
										<tr>
											<td class="align-middle">
												<asp:HiddenField runat="server" ID="hdnID" Value='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' />
												<asp:Label ID="draftsTableDate" Text='<%# DataBinder.Eval(Container.DataItem, "LastUpdateDate", "{0:MM/dd/yyyy - h:mm:ss tt}") %>' runat="server" />
											</td>
											<td class="align-middle text-start mw-0 text-truncate" data-overflow-tooltip="true" data-placement="top" title='<%# DataBinder.Eval(Container.DataItem, "OrdinanceTitle") %>'>
												<asp:Label ID="draftsTableTitle" Text='<%# DataBinder.Eval(Container.DataItem, "OrdinanceTitle") %>' runat="server" />
											</td>
											<td class="align-middle d-flex justify-content-center px-0">
												<asp:LinkButton runat="server" ID="editOrd" CommandName="edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' CssClass="formActionBtn border-end px-2" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="Edit" OnClientClick="showLoadingModal();" CausesValidation="false" TabIndex="-1"><i class="fas fa-pen-to-square text-warning-light"></i></asp:LinkButton>
												<a href="javascript:void(0);" onclick='<%# $"SetDeleteModal({DataBinder.Eval(Container.DataItem, "OrdinanceID")});" %>' class="formActionBtn px-2" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="Delete" TabIndex="-1"><i class="fas fa-trash-can text-danger"></i></a>
											</td>
										</tr>
									</ItemTemplate>
									<FooterTemplate>
								
									</FooterTemplate>
								</asp:Repeater>
							</table>
						</div>
						<div runat="server" id="lblNoItems" class="row text-center" style="margin-top: 12.5%;">
							<div class="col-md-12">
								<h4 runat="server" id="lblNoItemsTxt" class="text-danger">There are no items to show for the current search</h4>
							</div>
						</div>
					</div>
					<div runat="server" id="pnlFooter" class="card-footer p-0">
						<asp:Panel ID="pnlPagingP" CssClass="panel m-0" runat="server" Visible="false">
							<table class="table m-0" runat="server">
								<tr>
									<td class="text-left">
										<asp:LinkButton ID="lnkFirstSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="first" data-list="draftsTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1"><i class="fas fa-angles-left"></i>&nbsp;First</asp:LinkButton>
									</td>
									<td class="text-center">
										<asp:LinkButton ID="lnkPreviousSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="previous" data-list="draftsTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1"><i class="fas fa-angle-left"></i>&nbsp;Previous</asp:LinkButton>
									</td>
									<td class="text-center">
										<div style="margin-top: 5px">
											<asp:Label Style="font-weight: bold; font-size: 18px" ID="lblCurrentPageBottomSearchP" runat="server"></asp:Label>
										</div>
									</td>
									<td class="text-center">
										<asp:LinkButton ID="lnkNextSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="next" data-list="draftsTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1">Next&nbsp;<i class="fas fa-angle-right"></i></asp:LinkButton>
									</td>
									<td class="text-end">
										<asp:LinkButton ID="lnkLastSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="last" data-list="draftsTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1">Last&nbsp;<i class="fas fa-angles-right"></i></asp:LinkButton>
									</td>
								</tr>
							</table>
						</asp:Panel>
					</div>
				</div>

				<div runat="server" id="ordView" class="readonly-color custom-tab-div w-100" data-form="true">
					<asp:HiddenField runat="server" ID="hdnOrdID" />
					<asp:HiddenField runat="server" ID="hdnEffectiveDate" />
					<asp:HiddenField runat="server" ID="hdnEmail" />
					<asp:HiddenField runat="server" ID="hdnOrdStatusID" />
					<asp:HiddenField runat="server" ID="hdnStatusID" />
					<div runat="server" id="factSheetPane">
						<%-- FORM HEADER --%>
						<section class="container form-header bg-body-secondary text-center position-relative">
							<div class="row h-100 align-items-center">
								<h1><span class="fas fa-pen-ruler"></span>&nbsp;Fact Sheet</h1>
							</div>
							<asp:LinkButton runat="server" ID="backBtn" CssClass="btn bg-danger backBtn" OnClick="backBtn_Click" OnClientClick="showLoadingModal();" TabIndex="-1"><span class="fas fa-xmark text-light"></span></asp:LinkButton>
						</section>

						<%-- FORM BODY --%>
						<div class="container form-page bg-body-tertiary">

							<div class="px-2 py-4">
								<%-- REQUIRED FIELD DESCRIPTOR --%>
								<p class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i>&nbsp;= Required Field</p>

								<%-- FIRST SECTION --%>
								<div class="form-section">
									<%-- FIRST ROW --%>
									<div class="row mb-3">
										<%-- DEPARTMENT --%>
										<div class="col-md-5">
											<div class="form-group">
												<label for="requestDepartment">Requesting Department</label>
												<asp:DropDownList ID="requestDepartment" runat="server" AutoPostBack="true" CssClass="form-select" data-required="true" OnSelectedIndexChanged="requestDepartment_SelectedIndexChanged"></asp:DropDownList>
												<asp:RequiredFieldValidator runat="server" ID="requestDepartmentValid" ControlToValidate="requestDepartment" ErrorMessage="Please Select a Department" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>

										<%-- DIVISION --%>
										<div runat="server" id="requestDivisionDiv" class="col-md-5">
											<div class="form-group">
												<label for="requestDivision">Requesting Division</label>
												<asp:DropDownList ID="requestDivision" runat="server" AutoPostBack="true" CssClass="form-select" data-required="true"></asp:DropDownList>
												<asp:RequiredFieldValidator runat="server" ID="requestDivisionValid" ControlToValidate="requestDivision" ErrorMessage="Please Select a Division" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>

										<%-- 1ST READ DATE --%>
										<div class="col-md-2">
											<div class="form-group">
												<label for="firstReadDate">Date of 1<sup>st</sup> Reading</label>
												<div class="input-group">
													<asp:TextBox runat="server" ID="firstReadDate" CssClass="form-control" TextMode="Date" data-required="true"></asp:TextBox>
													<button runat="server" id="firstReadDatePicker" type="button" class="btn input-group-text" data-calendar-btn="firstReadDate" tabindex="-1"><span class="fas fa-calendar-days"></span></button>
												</div>
												<asp:RequiredFieldValidator runat="server" ID="firstReadDateValid" ControlToValidate="firstReadDate" ErrorMessage="Please Select a Date" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>
									</div>

									<%-- SECOND ROW --%>
									<div class="row mb-3">
										<%-- CONTACT --%>
										<div class="col-md-5">
											<div class="form-group">
												<label for="requestContact">Requesting Contact</label>
												<asp:TextBox runat="server" ID="requestContact" CssClass="form-control" TextMode="SingleLine" placeholder="John Doe" AutoCompleteType="DisplayName" data-required="true"></asp:TextBox>
												<asp:RequiredFieldValidator runat="server" ID="requestContactValid" ControlToValidate="requestContact" ErrorMessage="Please Enter a Name" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>

										<%-- EMAIL --%>
										<div class="col-md-3">
											<div class="form-group">
												<label for="requestEmail">Email</label>
												<asp:TextBox runat="server" ID="requestEmail" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-required="true"></asp:TextBox>
												<asp:RequiredFieldValidator runat="server" ID="requestEmailValid" ControlToValidate="requestEmail" ErrorMessage="Please Enter an Email" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>

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
												<asp:TextBox runat="server" ID="requestPhone" CssClass="form-control" TextMode="Phone" data-type="telephone" placeholder="(555) 555-5555" AutoCompleteType="Disabled" data-required="true"></asp:TextBox>

												<%-- EXTENSION --%>
												<asp:TextBox runat="server" ID="requestExt" CssClass="form-control ext-split" TextMode="SingleLine" data-type="extension" placeholder="x1234" AutoCompleteType="Disabled" data-required="true"></asp:TextBox>
											</div>
											<div class="d-flex justify-content-between">
												<asp:RequiredFieldValidator runat="server" ID="requestPhoneValid" ControlToValidate="requestPhone" ErrorMessage="Please Enter a Phone Number" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
												<asp:RequiredFieldValidator runat="server" ID="requestExtValid" ControlToValidate="requestExt" ErrorMessage="Please Enter an Extension" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback text-end"></asp:RequiredFieldValidator>
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
												<div class="radioListDiv" id="epList">
													<%-- YES --%>
													<div class="form-check form-check-inline">
														<label for="epYes">Yes</label>
														<asp:RadioButton runat="server" ID="epYes" CssClass="form-check-input" GroupName="epList" OnCheckedChanged="EPCheckedChanged" AutoPostBack="true" TabIndex="-1" />
													</div>

													<%-- NO --%>
													<div class="form-check form-check-inline">
														<label for="epNo">No</label>
														<asp:RadioButton runat="server" ID="epNo" CssClass="form-check-input" GroupName="epList" OnCheckedChanged="EPCheckedChanged" AutoPostBack="true" TabIndex="-1" />
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
												<asp:TextBox runat="server" ID="epJustification" CssClass="form-control" TextMode="Multiline" Rows="4" AutoCompleteType="Disabled"></asp:TextBox>
												<asp:RequiredFieldValidator runat="server" ID="epJustificationValid" ControlToValidate="epJustification" ErrorMessage="Please Explain Justification" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
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
												<asp:TextBox runat="server" ID="fiscalImpact" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled" ></asp:TextBox>
											</div>
										</div>
									</div>

									<%-- SECOND ROW --%>
									<div class="row mb-3">
										<%-- SUGGESTED TITLE --%>
										<div class="col-md-12">
											<label for="suggestedTitle">Suggested Title</label>
											<asp:TextBox runat="server" ID="suggestedTitle" CssClass="form-control" TextMode="Multiline" Rows="4" AutoCompleteType="Disabled" data-required="true"></asp:TextBox>
											<asp:RequiredFieldValidator runat="server" ID="suggestedTitleValid" ControlToValidate="suggestedTitle" ErrorMessage="Please Enter a Title" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2" ></asp:RequiredFieldValidator>
										</div>
									</div>
								</div>

								<%-- FOURTH SECTION --%>
								<div class="form-section">
									<%-- FIRST ROW --%>
									<div class="row mb-5">
										<%-- VENDOR NAME --%>
										<div class="col-md-10">
											<div class="form-group">
												<label for="vendorName">Vendor Name</label>
												<asp:TextBox runat="server" ID="vendorName" CssClass="form-control" TextMode="SingleLine" placeholder="Vendor Incorporated LLC" AutoCompleteType="Company"></asp:TextBox>
											</div>
										</div>

										<%-- VENDOR NUMBER --%>
										<div class="col-md-2">
											<div class="form-group">
												<label for="vendorNumber">Vendor Number</label>
												<asp:TextBox runat="server" ID="vendorNumber" CssClass="form-control" TextMode="SingleLine" placeholder="0123456789" AutoCompleteType="Disabled"></asp:TextBox>
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
													<asp:TextBox runat="server" ID="contractStartDate" CssClass="form-control" TextMode="Date" data-type="datePeriodStart"></asp:TextBox>
													<button runat="server" id="contractStartDatePicker" type="button" class="btn input-group-text" data-calendar-btn="contractStartDate" tabindex="-1"><span class="fas fa-calendar-days"></span></button>

													<%-- SEPARATOR --%>
													<div class="input-group-append">
														<span class="input-group-text date-period-separator"><i class="fas fa-minus"></i></span>
													</div>

													<%-- END --%>
													<asp:TextBox runat="server" ID="contractEndDate" CssClass="form-control" TextMode="Date" data-type="datePeriodEnd"></asp:TextBox>
													<button runat="server" id="contractEndDatePicker" type="button" class="btn input-group-text" data-calendar-btn="contractEndDate" tabindex="-1"><span class="fas fa-calendar-days"></span></button>
												</div>
											</div>
										</div>

										<%-- DATE TERM --%>
										<div class="col-md-3">
											<div class="form-group">
												<label for="dateTerm">Date Term</label>
												<input runat="server" id="contractTerm" type="text" data-type="dateTerm" class="form-control locked-field" autocomplete="off" readonly="readonly" value="" placeholder="Calculating Term..." required tabindex="-1">
											</div>
										</div>

										<%-- CONTRACT AMOUNT --%>
										<div class="col-md-3">
											<div class="form-group">
												<label for="contractAmount">Contract Amount</label>
												<asp:TextBox runat="server" ID="contractAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled"></asp:TextBox>
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
												<div class="radioListDiv" id="scopeChangeList">
													<%-- YES --%>
													<div class="form-check form-check-inline">
														<label for="scYes">Yes</label>
														<asp:RadioButton runat="server" ID="scYes" CssClass="form-check-input" GroupName="scopeChangeList" OnCheckedChanged="SCCheckedChanged" AutoPostBack="true" TabIndex="-1" />
													</div>

													<%-- NO --%>
													<div class="form-check form-check-inline">
														<label for="scNo">No</label>
														<asp:RadioButton runat="server" ID="scNo" CssClass="form-check-input" GroupName="scopeChangeList" OnCheckedChanged="SCCheckedChanged" AutoPostBack="true" TabIndex="-1" />
													</div>
												</div>
											</div>
										</div>
									</div>

									<%-- SECOND ROW --%>
									<div runat="server" id="scopeChangeOptions" class="row mb-3">
										<%-- CHANGE ORDER NUMBER --%>
										<div class="col-md-4">
											<label for="changeOrderNumber">Change Order Number</label>
											<asp:TextBox runat="server" ID="changeOrderNumber" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled"></asp:TextBox>
											<asp:RequiredFieldValidator runat="server" ID="changeOrderNumberValid" ControlToValidate="changeOrderNumber" ErrorMessage="Please Enter a Change Order Number" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
										</div>

										<%-- ADDITIONAL AMOUNT --%>
										<div class="col-md-2">
											<div class="form-group">
												<label for="additionalAmount">Additional Amount</label>
												<asp:TextBox runat="server" ID="additionalAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" AutoCompleteType="Disabled"></asp:TextBox>
												<asp:RequiredFieldValidator runat="server" ID="additionalAmountValid" ControlToValidate="additionalAmount" ErrorMessage="Please Enter an Amount" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>
									</div>
								</div>

								<%-- SIXTH SECTION --%>
								<div class="form-section">
									<%-- FIRST ROW --%>
									<div class="row mb-5">
										<%-- PURCHASE METHOD --%>
										<div class="col-md-5">
											<div class="form-group">
												<label for="purchaseMethod">Method of Purchase</label>
												<asp:DropDownList ID="purchaseMethod" runat="server" OnSelectedIndexChanged="PurchaseMethodSelectedIndexChanged" AutoPostBack="true" CssClass="form-select" data-required="true"></asp:DropDownList>
												<asp:RequiredFieldValidator runat="server" ID="purchaseMethodValid" ControlToValidate="purchaseMethod" ErrorMessage="Please Select a Method" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>

										<%-- OTHER / EXCEPTION --%>
										<div class="col-md-4">
											<div runat="server" id="otherExceptionDiv" class="form-group">
												<label for="otherException">Other/Exception</label>
												<asp:TextBox runat="server" ID="otherException" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled"></asp:TextBox>
												<asp:RequiredFieldValidator runat="server" ID="otherExceptionValid" ControlToValidate="otherException" ErrorMessage="Please Enter a Method" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>
									</div>

									<%-- SECOND ROW --%>
									<div class="row mb-3">
										<%-- PREVIOUS ORDINANCE NUMBERS --%>
										<div class="col-md-4">
											<div class="form-group">
												<label for="prevOrdinanceNums">Previous Ordinance Numbers</label>
												<asp:TextBox runat="server" ID="prevOrdinanceNums" CssClass="form-control" TextMode="SingleLine" placeholder="123-45-6789" AutoCompleteType="Disabled"></asp:TextBox>
											</div>
										</div>

										<%-- CODE PROVISION --%>
										<div class="col-md-4">
											<div class="form-group">
												<label for="codeProvision">Code Provision</label>
												<asp:TextBox runat="server" ID="codeProvision" CssClass="form-control" TextMode="SingleLine" placeholder="0123456789" AutoCompleteType="Disabled"></asp:TextBox>
											</div>
										</div>
									</div>
								</div>

								<%-- SEVENTH SECTION --%>
								<div class="form-section">
									<%-- FIRST ROW --%>
									<div class="row mb-3">
										<%-- PURCHASING AGENT APPROVAL REQUIRED --%>
										<div class="col-md-12">
											<div class="form-group">
												<label for="paApprovalRequiredList">Is Purchasing Agent Approval Required?</label>
												<div class="radioListDiv" id="paApprovalRequiredList">
													<%-- YES --%>
													<div class="form-check form-check-inline">
														<label for="paApprovalRequiredYes">Yes</label>
														<asp:RadioButton runat="server" ID="paApprovalRequiredYes" CssClass="form-check-input" GroupName="paApprovalRequiredList" TabIndex="-1" />
													</div>

													<%-- NO --%>
													<div class="form-check form-check-inline">
														<label for="paApprovalRequiredNo">No</label>
														<asp:RadioButton runat="server" ID="paApprovalRequiredNo" CssClass="form-check-input" GroupName="paApprovalRequiredList" TabIndex="-1" />
													</div>
												</div>
											</div>
										</div>
									</div>

									<%-- SECOND ROW --%>
									<div class="row mb-3">
										<%-- PURCHASING AGENT APPROVAL ATTACHED --%>
										<div class="col-md-12">
											<div class="form-group">
												<label for="paApprovalAttachedList">Is Purchasing Agent Approval Attached?</label>
												<div class="radioListDiv" id="paApprovalAttachedList">
													<%-- YES --%>
													<div class="form-check form-check-inline">
														<label for="paApprovalAttachedYes">Yes</label>
														<asp:RadioButton runat="server" ID="paApprovalAttachedYes" CssClass="form-check-input" GroupName="paApprovalAttachedList" TabIndex="-1" />
													</div>

													<%-- NO --%>
													<div class="form-check form-check-inline">
														<label for="paApprovalAttachedNo">No</label>
														<asp:RadioButton runat="server" ID="paApprovalAttachedNo" CssClass="form-check-input" GroupName="paApprovalAttachedList" TabIndex="-1" />
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>

								<%-- EIGHTH SECTION --%>
								<div class="form-section">
									<%-- FIRST ROW --%>
									<div class="row mb-3">
										<%-- REVENUE --%>
										<div class="col-md-6hf form-table">
											<label for="revenueTable">Revenue</label>
											<%-- REVENUE TABLE --%>
											<table id="revenueTable" class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
												<%-- TABLE HEAD --%>
												<thead>
													<tr>
														<th style="width: 13%; text-align: center">Fund</th>
														<th style="width: 15%; text-align: center">Agency</th>
														<th style="width: 15%; text-align: center">Org</th>
														<th style="width: 16%; text-align: center">Activity</th>
														<th style="width: 15%; text-align: center">Object</th>
														<th style="width: 18%; text-align: center">Amount</th>
													</tr>
												</thead>

												<%-- TABLE BODY --%>
												<tbody>
													<%-- REVENUE TABLE REPEATER --%>
													<asp:Repeater runat="server" ID="rpRevenueTable" OnItemCommand="rpAccountingTable_ItemCommand" OnItemCreated="rpRevExpTable_ItemCreated">
														<ItemTemplate>
															<tr class="upperCaseField">
																<td runat="server" id="revenueFundCodeCell" style="vertical-align: middle">
																	<asp:HiddenField runat="server" ID="hdnRevID" Value='<%# DataBinder.Eval(Container.DataItem, "OrdinanceAccountingID") %>' />
																	<asp:HiddenField runat="server" ID="hdnRevIndex" Value='<%# Container.ItemIndex %>' />
																	<asp:TextBox runat="server" ID="revenueFundCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "FundCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="revenueAgencyCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="revenueAgencyCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "DepartmentCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="revenueOrgCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="revenueOrgCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "UnitCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="revenueActivityCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="revenueActivityCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ActivityCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="revenueObjectCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="revenueObjectCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ObjectCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td class="position-relative" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="revenueAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled" Text='<%# (Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Amount")) >= 0)?DataBinder.Eval(Container.DataItem, "Amount"):string.Empty%>'></asp:TextBox>

																	<div runat="server" id="removeRevRowDiv">
																		<asp:Button runat="server" ID="removeRevenueRow" CssClass="btn row-delete" UseSubmitBehavior="false" CommandName="delete" CommandArgument="revenue" Text="&#xf068;" data-disable-btn="aspIconBtn" TabIndex="-1" />
																	</div>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</tbody>
											</table>

											<%-- ADD REVENUE ROW BUTTON --%>
											<div runat="server" id="newRevenueRowDiv" class="text-center w-100">
												<asp:Button runat="server" ID="newRevenueRow" CssClass="btn btn-success w-100 row-add" OnClick="newAccountingRow_ServerClick" UseSubmitBehavior="false" CommandName="revenue" Text="Add Row" data-disable-btn="aspBtn" data-disable-btn-text="Adding Row" TabIndex="-1" />
											</div>
										</div>

										<%-- BLANK SPACE --%>
										<div class="col-md-1hf"></div>

										<%-- EXPENDITURE --%>
										<div class="col-md-6hf form-table">
											<label for="expenditureTable">Expenditure</label>
											<%-- EXPENDITURE TABLE --%>
											<table id="expenditureTable" class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
												<%-- TABLE HEAD --%>
												<thead>
													<tr>
														<th style="width: 13%; text-align: center">Fund</th>
														<th style="width: 15%; text-align: center">Agency</th>
														<th style="width: 15%; text-align: center">Org</th>
														<th style="width: 16%; text-align: center">Activity</th>
														<th style="width: 15%; text-align: center">Object</th>
														<th style="width: 18%; text-align: center">Amount</th>
													</tr>
												</thead>

												<%-- TABLE BODY --%>
												<tbody>
													<%-- EXPENDITURE TABLE REPEATER --%>
													<asp:Repeater runat="server" ID="rpExpenditureTable" OnItemCommand="rpAccountingTable_ItemCommand" OnItemCreated="rpRevExpTable_ItemCreated">
														<ItemTemplate>
															<tr class="upperCaseField">
																<td runat="server" id="expenditureFundCodeCell" style="vertical-align: middle">
																	<asp:HiddenField runat="server" ID="hdnExpID" Value='<%# DataBinder.Eval(Container.DataItem, "OrdinanceAccountingID") %>' />
																	<asp:HiddenField runat="server" ID="hdnExpIndex" Value='<%# Container.ItemIndex %>' />
																	<asp:TextBox runat="server" ID="expenditureFundCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "FundCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="expenditureAgencyCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="expenditureAgencyCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "DepartmentCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="expenditureOrgCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="expenditureOrgCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "UnitCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="expenditureActivityCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="expenditureActivityCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ActivityCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td runat="server" id="expenditureObjectCodeCell" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="expenditureObjectCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ObjectCode") %>' data-required="true" ClientIDMode="AutoID"></asp:TextBox>
																</td>
																<td class="position-relative" style="vertical-align: middle">
																	<asp:TextBox runat="server" ID="expenditureAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$0.00" AutoCompleteType="Disabled" Text='<%# (Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Amount")) >= 0)?DataBinder.Eval(Container.DataItem, "Amount"):string.Empty%>'></asp:TextBox>

																	<div runat="server" id="removeExpRowDiv">
																		<asp:Button runat="server" ID="removeExpenditureRow" CssClass="btn row-delete" UseSubmitBehavior="false" CommandName="delete" CommandArgument="expenditure" Text="&#xf068;" data-disable-btn="aspIconBtn" TabIndex="-1" />
																	</div>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</tbody>
											</table>

											<%-- ADD EXPENDITURE ROW BUTTON --%>
											<div runat="server" id="newExpenditureRowDiv" class="text-center w-100">
												<asp:Button runat="server" ID="newExpenditureRow" CssClass="btn btn-success w-100 row-add" OnClick="newAccountingRow_ServerClick" UseSubmitBehavior="false" CommandName="expenditure" Text="Add Row" data-disable-btn="aspBtn" data-disable-btn-text="Adding Row" TabIndex="-1" />
											</div>
										</div>

										<%-- BLANK SPACE --%>
										<div class="col-md-1hf"></div>
									</div>
								</div>

								<%-- NINTH SECTION --%>
								<div class="form-section">
									<%-- FIRST ROW --%>
									<div class="row mb-5">
										<%-- STAFF ANALYSIS --%>
										<div class="col-md-12">
											<div class="form-group">
												<label for="staffAnalysis">Staff Analysis</label>
												<asp:TextBox runat="server" ID="staffAnalysis" CssClass="form-control" TextMode="Multiline" Rows="18" AutoCompleteType="Disabled" data-required="true"></asp:TextBox>
												<asp:RequiredFieldValidator runat="server" ID="staffAnalysisValid" ControlToValidate="staffAnalysis" ErrorMessage="Please Enter an Analysis" ValidationGroup="factSheetMain" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2"></asp:RequiredFieldValidator>
											</div>
										</div>
									</div>

									<%-- SECOND ROW --%>
									<div runat="server" id="supportingDocumentationDiv" class="row mb-3">
										<%-- SUPPORTING DOCUMENTATION --%>
										<div class="col-md-8">
											<div class="form-group">
												<label for="supportingDocumentationGroup">Supporting Documentation (Ex: Contract, Agreement, Change Order, Bid Book)</label>
												<ul class="list-group mt-1">
													<asp:Repeater runat="server" ID="rpSupportingDocumentation" OnItemCommand="rpSupportingDocumentation_ItemCommand">
														<ItemTemplate>
															<li class="list-group-item" style="line-height: 2.25;">
																<asp:HiddenField runat="server" ID="hdnDocID" Value='<%# DataBinder.Eval(Container.DataItem, "DocumentID") %>' />
																<asp:HiddenField runat="server" ID="hdnDocIndex" Value='<%# Container.ItemIndex %>' />
																<%# DataBinder.Eval(Container.DataItem, "DocumentName") %>
																<div class="d-flex float-end">
																	<asp:LinkButton runat="server" ID="supportingDocDownload" CssClass="btn btn-primary" CommandName="download" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DocumentName") %>' Style="margin-right: 5px;" TabIndex="-1"><span class="fas fa-download"></span></asp:LinkButton>
																	<asp:LinkButton runat="server" ID="deleteFile" CssClass="btn btn-danger" CommandName="delete" Style="margin-left: 5px;" data-disable-btn="aspIconBtn" TabIndex="-1"><span class="fas fa-trash-can"></span></asp:LinkButton>
																</div>
															</li>
														</ItemTemplate>
													</asp:Repeater>
												</ul>
												<div id="supportingDocumentationGroup" class="d-flex mb-2">
													<asp:FileUpload runat="server" ID="supportingDocumentation" CssClass="form-control mt-3" AllowMultiple="true" onchange="SetUploadActive(this.id, 'uploadBtn');" onfocus="showFileWaiting();" TabIndex="-1" />
													<button id="uploadBtn" class="btn btn-success mt-3 ms-3 w-25" onclick="clickAspBtn('UploadDocBtn');" type="button" data-disable-btn="htmlIconBtn" data-disable-btn-icon="fa-upload" data-disable-btn-text="Uploading" disabled TabIndex="-1"><span>Upload</span></button>
													<asp:Button runat="server" ID="UploadDocBtn" UseSubmitBehavior="false" Width="25%" OnClick="UploadDocBtn_Click" hidden="hidden" TabIndex="-1" />
												</div>
												<div id="fileWaiting" class="mt-2" hidden>
													<strong class="text-warning fa-fade"><span class="fa-solid fa-hourglass-end fa-flip"></span>&nbsp;Waiting for file...</strong>
												</div>
											</div>
										</div>
									</div>
								</div>

								<%-- TENTH SECTION --%>
								<div runat="server" id="signatureSection" class="form-section">
									<%-- FIRST ROW --%>
									<div runat="server" id="emailListDiv" class="row mb-3">
										<div class="col-md-6">
											<div class="form-group">
												<label for="emailListContainer">Director/Supervisor Email(s) <span class="text-warning">* <span style="font-size: 11px;">Required if Submitting</span></span></label>
												<div id="emailListContainer" class="card mb-3 bg-body" style="min-height: 100px !important;">
													<div class="card-body" >
														<asp:Repeater runat="server" ID="rpEmailList" OnItemCommand="rpEmailList_ItemCommand" OnItemCreated="rpEmailList_ItemCreated">
															<ItemTemplate>
																<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
																	<%# Container.DataItem %>
																	<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="remove" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark" data-disable-btn="aspIconBtn"></span></asp:LinkButton>
																</div>
															</ItemTemplate>
														</asp:Repeater>
													</div>
													<div class="card-footer bg-body-tertiary">
														<div class="row">
															<div class="col-md-12">
																<div class="input-group">
																	<span class="input-group-text fas fa-address-book"></span>
																	<asp:TextBox runat="server" ID="signatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" data-enter-btn="AddRequestEmailAddress"></asp:TextBox>
																	<asp:Button runat="server" ID="AddRequestEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" OnClick="AddRequestEmailAddress_Click" disabled="disabled" Text='&#xf055;' data-disable-btn="aspIconBtn" TabIndex="-1"/>
																</div>
															</div>
														</div>
													</div>
												</div>
												<asp:TextBox runat="server" ID="directorSupervisorEmailAddresses" hidden="true" TabIndex="-1"></asp:TextBox>
												<asp:RequiredFieldValidator runat="server" ID="emailListContainerValid" ControlToValidate="directorSupervisorEmailAddresses" ErrorMessage="Please Enter an Email" ValidationGroup="directorSupervisorValidGroup" SetFocusOnError="false" Display="None" CssClass="text-danger invalid-feedback ps-2" EnableViewState="true"></asp:RequiredFieldValidator>
											</div>
										</div>
									</div>
								</div>

								<%-- SUBMIT SECTION --%>
								<div runat="server" id="submitSection" class="form-section">
									<%-- FIRST ROW --%>
									<div class="row mt-3 mb-3 text-center">
										<%-- SUBMIT BUTTON --%>
										<div class="col-md-6">
											<button id="submitBtn" class="btn btn-success float-end w-50" onclick="validateFactSheetDraft('submitBtn', 'factSheetMain,directorSupervisorValidGroup');" type="button" data-disable-btn="htmlBtn" data-disable-btn-text="Submitting" TabIndex="-1"><span>Submit</span></button>
											<asp:Button runat="server" ID="SubmitFactSheet" UseSubmitBehavior="false" OnClick="SaveFactSheet_Click" CommandName="submit" hidden="true" TabIndex="-1" />
										</div>
										<%-- SAVE BUTTON --%>
										<div class="col-md-6">
											<button id="saveBtn" class="btn btn-primary float-start w-50" onclick="validateFactSheetDraft('saveBtn', 'factSheetMain');" type="button" data-disable-btn="htmlIconBtn" data-disable-btn-icon="fa-floppy-disk" data-disable-btn-text="Saving" TabIndex="-1">Save Draft</button>
											<asp:Button runat="server" ID="SaveFactSheet" UseSubmitBehavior="false" OnClick="SaveFactSheet_Click" CommandName="save" hidden="true" TabIndex="-1" />
										</div>
									</div>

									<%-- SECOND ROW --%>
									<div class="row mb-3 text-center">
										<%-- DELETE BUTTON --%>
										<div class="col-md-12">
											<asp:Button runat="server" ID="DeleteFactSheet" UseSubmitBehavior="false" CssClass="btn btn-danger" Width="51.1725%" Text="Delete" data-toggle="modal" data-target="#deleteModal" TabIndex="-1" />
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>		
	</section>

	<!-- DELETE MODAL -->
	<div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel">
		<div class="modal-dialog" role="document">
			<div class="modal-content">
				<div class="modal-header">
					<h4 class="modal-title" id="deleteModalLabel">Delete</h4>
				</div>
				<div class="modal-body bg-body-tertiary">
					<asp:Label runat="server" ID="deleteLabel" Style="font-size: 18px; font-weight: bold" CssClass="text-danger" Text="Are you sure you want to delete this ordinance fact sheet? (This cannot be undone)" />
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal" tabindex="-1">Cancel</button>
					<asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" CausesValidation="false" UseSubmitBehavior="false" Visible="true" OnClick="mdlDeleteSubmit_ServerClick" OnClientClick="ShowSubmitToast();" data-disable-btn="htmlBtn" data-disable-btn-text="Deleting" TabIndex="-1" />
					<asp:HiddenField runat="server" ID="hdnDeleteID" />
				</div>
			</div>
		</div>
	</div>


	<%-- JAVASCRIPT --%>
	<script>
		InitialLoad();

		function InitialLoad() {
			SetTooltips();
			cancelFilePick('<%= supportingDocumentation.ClientID %>');
			addSignatureEmails([{ addressID: '<%= signatureEmailAddress.ClientID %>', btnID: '<%= AddRequestEmailAddress.ClientID %>' }]);
			DisableDDInitialOption([
				{ id: '<%= requestDepartment.ClientID %>', opacity: "75" },
				{ id: '<%= requestDivision.ClientID %>', opacity: "35" },
				{ id: '<%= purchaseMethod.ClientID %>', opacity: "75" },
			]);
		}

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			SetTooltips();
			cancelFilePick('<%= supportingDocumentation.ClientID %>');
			addSignatureEmails([{ addressID: '<%= signatureEmailAddress.ClientID %>', btnID: '<%= AddRequestEmailAddress.ClientID %>' }]);
			DisableDDInitialOption([
				{ id: '<%= requestDepartment.ClientID %>', opacity: "75" },
				{ id: '<%= requestDivision.ClientID %>', opacity: "35" },
				{ id: '<%= purchaseMethod.ClientID %>', opacity: "75" },
			]);
			HideAllTooltips();
		});

		function validateFactSheetDraft(btnID, validationGroups) {
			if (Page_ClientValidate(validationGroups)) {
				isValid = true;
				var btn;
				switch (btnID) {
					case "submitBtn":
						btn = document.getElementById('<%= SubmitFactSheet.ClientID %>');
						break;
					case "saveBtn":
						btn = document.getElementById('<%= SaveFactSheet.ClientID %>');
						break;
				}
				btn.click();
				ShowSubmitToast();
			}
			else {
				isValid = false;
				Page_BlockSubmit = false;
				ValidationFormatting(Page_Validators);
				currentValidation = validationGroups.split(",");
				$('#submitToast').removeClass('text-bg-danger');
				$('#submitToast').removeClass('text-bg-success');
				$('#submitToast').addClass('text-bg-danger');
				$('#toastMessage').html('Complete all required fields to proceed!');
				$('#submitToast').toast('show');
				return false;
			}
			return true;
		}
	</script>
</asp:Content>