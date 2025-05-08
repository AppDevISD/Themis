<%@ Page Title="Ordinances" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ordinances.aspx.cs" Inherits="WebUI.Ordinances" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<%-- PAGE CONTENT --%>
	<section>
		<asp:UpdatePanel runat="server" ID="pnlOrdinanceTable" UpdateMode="Always" class="overlap-panels">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="backBtn" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkFirstSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkPreviousSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkNextSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkLastSearchP" EventName="Click" />

				<asp:AsyncPostBackTrigger ControlID="lnkAuditFirstSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkAuditPreviousSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkAuditNextSearchP" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lnkAuditLastSearchP" EventName="Click" />

				<asp:AsyncPostBackTrigger ControlID="epYes" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="epNo" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="scYes" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="scNo" EventName="CheckedChanged" />

				<asp:AsyncPostBackTrigger ControlID="purchaseMethod" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="ddStatus" EventName="SelectedIndexChanged" />

				<asp:AsyncPostBackTrigger ControlID="sortDate" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortTitle" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortDepartment" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortContact" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortStatus" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnSendSigEmail" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnSignDoc" EventName="Click" />

				<asp:PostBackTrigger ControlID="UploadDocBtn" />
				<asp:PostBackTrigger ControlID="SaveFactSheet" />

				<asp:AsyncPostBackTrigger ControlID="lnkInactivityRefresh" EventName="Click" />
			</Triggers>

			<ContentTemplate>
				<div runat="server" id="ordTable" class="card">
					<div class="card-header bg-body">
						<h3><i class="fas fa-book-section"></i>&nbsp;Ordinances</h3>
					</div>
					<div class="card-body bg-body-tertiary">
						<%-- FILTERS & SORTING --%>
						<div class="row mb-4">
							<div class="col-md-3" runat="server" id="filterDepartmentDiv">
								<div class="form-group">
									<label for="filterDepartment">Filter by Department</label>
									<asp:DropDownList ID="filterDepartment" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="department"></asp:DropDownList>
								</div>
							</div>
							<div class="col-md-3">
								<div class="form-group">
									<label for="filterStatus">Filter by Status</label>
									<asp:DropDownList ID="filterStatus" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="status"></asp:DropDownList>
								</div>
							</div>
						</div>

						<%-- TABLE --%>
						<div runat="server" id="formTableDiv">
							<table id="FormTable" class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
								<thead>
									<tr>
										<th style="width: 4%; text-align: center"><asp:LinkButton runat="server" ID="sortID" data-command="OrdinanceID" data-text="ID" OnClick="SortBtn_Click" class="btn btn-sort"><strong>ID<span runat="server" class='float-end lh-1p5'></span></strong></asp:LinkButton></th>
										<th style="width: 6%; text-align: center"><asp:LinkButton runat="server" ID="sortDate" data-command="EffectiveDate" data-text="Date" OnClick="SortBtn_Click" class="btn btn-sort"><strong>Date<span runat="server" class='float-end lh-1p5 fas fa-arrow-down'></span></strong></asp:LinkButton></th>
										<th style="width: 39%; text-align: center"><asp:LinkButton runat="server" ID="sortTitle" data-command="OrdinanceTitle" data-text="Title" OnClick="SortBtn_Click" class="btn btn-sort"><strong>Title<span class="float-end lh-1p5"></span></strong></asp:LinkButton></th>
										<th style="width: 20%; text-align: center"><asp:LinkButton runat="server" ID="sortDepartment" data-command="RequestDepartment" data-text="Department" OnClick="SortBtn_Click" class="btn btn-sort"><strong>Department<span class="float-end lh-1p5"></span></strong></asp:LinkButton></th>
										<th style="width: 15%; text-align: center"><asp:LinkButton runat="server" ID="sortContact" data-command="RequestContact" data-text="Contact" OnClick="SortBtn_Click" class="btn btn-sort"><strong>Contact<span class="float-end lh-1p5"></span></strong></asp:LinkButton></th>
										<th style="width: 10%; text-align: center"><asp:LinkButton runat="server" ID="sortStatus" data-command="StatusDescription" data-text="Status" OnClick="SortBtn_Click" class="btn btn-sort"><strong>Status<span class="float-end lh-1p5"></span></strong></asp:LinkButton></th>
										<th style="width: 6%; text-align: center"><strong>Action</strong></th>
									</tr>
								</thead>
								<asp:Repeater runat="server" ID="rpOrdinanceTable" OnItemCommand="rpOrdinanceTable_ItemCommand">
									<HeaderTemplate></HeaderTemplate>
									<ItemTemplate>
										<tr>
											<td class="align-middle">
												<asp:HiddenField runat="server" ID="hdnID" Value='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' />
												<asp:Label ID="ordID" Text='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' runat="server" />
											</td>
											<td class="align-middle">
												<asp:Label ID="ordTableDate" Text='<%# DataBinder.Eval(Container.DataItem, "EffectiveDate", "{0:MM/dd/yyyy}") %>' runat="server" />
											</td>
											<td class="align-middle text-start mw-0 text-truncate" data-overflow-tooltip="true" data-tooltip="tooltip" data-placement="right" title='<%# DataBinder.Eval(Container.DataItem, "OrdinanceTitle") %>'>
												<asp:Label ID="ordTableTitle" Text='<%# DataBinder.Eval(Container.DataItem, "OrdinanceTitle") %>' runat="server" />
											</td>
											<td class="align-middle">
												<asp:Label ID="ordTableDepartment" Text='<%# DataBinder.Eval(Container.DataItem, "RequestDepartment") %>' runat="server" />
											</td>
											<td class="align-middle">
												<asp:Label ID="ordTableContact" Text='<%# DataBinder.Eval(Container.DataItem, "RequestContact") %>' runat="server" />
											</td>
											<td class="align-middle">
												<asp:Label ID="ordTableStatus" Text='<%# DataBinder.Eval(Container.DataItem, "StatusDescription") %>' runat="server" />
											</td>
											<td class="align-middle d-flex justify-content-around">
												<asp:LinkButton runat="server" ID="editOrd" CommandName="edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' CssClass="ordActionBtn" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="Edit" Visible='<%# !DataBinder.Eval(Container.DataItem, "StatusDescription").Equals("Deleted") %>'><i class="fas fa-pen-to-square text-warning-light"></i></asp:LinkButton>
												<asp:LinkButton runat="server" ID="viewOrd" CommandName="view" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' CssClass="ordActionBtn" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="View"><i class="fas fa-magnifying-glass text-info"></i></asp:LinkButton>
												<asp:LinkButton runat="server" ID="downloadOrd" CommandName="download" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' CssClass="ordActionBtn" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="Download"><i class="fas fa-download text-primary"></i></asp:LinkButton>
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
								<h4 class="text-danger">There are no items to show for the current filter(s)</h4>
							</div>
						</div>
					</div>
					<div class="card-footer p-0">
						<asp:Panel ID="pnlPagingP" CssClass="panel m-0" runat="server" Visible="false">
							<table class="table m-0" runat="server">
								<tr>
									<td class="text-left">
										<asp:LinkButton ID="lnkFirstSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="first" data-list="ordTable" style="width: 150px;" causesvalidation="false"><i class="fas fa-angles-left"></i>&nbsp;First</asp:LinkButton>
									</td>
									<td class="text-center">
										<asp:LinkButton ID="lnkPreviousSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="previous" data-list="ordTable" style="width: 150px;" causesvalidation="false"><i class="fas fa-angle-left"></i>&nbsp;Previous</asp:LinkButton>
									</td>
									<td class="text-center">
										<div style="margin-top: 5px">
											<asp:Label Style="font-weight: bold; font-size: 18px" ID="lblCurrentPageBottomSearchP" runat="server"></asp:Label>
										</div>
									</td>
									<td class="text-center">
										<asp:LinkButton ID="lnkNextSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="next" data-list="ordTable" style="width: 150px;" causesvalidation="false">Next&nbsp;<i class="fas fa-angle-right"></i></asp:LinkButton>
									</td>
									<td class="text-end">
										<asp:LinkButton ID="lnkLastSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="last" data-list="ordTable" style="width: 150px;" causesvalidation="false">Last&nbsp;<i class="fas fa-angles-right"></i></asp:LinkButton>
									</td>
								</tr>
							</table>
						</asp:Panel>
					</div>
				</div>

				<div runat="server" id="ordView" readonly="false" class="readonly-color custom-tab-div w-100">
					<asp:HiddenField runat="server" ID="hdnOrdID" />
					<asp:HiddenField runat="server" ID="hdnEffectiveDate" />
					<asp:HiddenField runat="server" ID="hdnEmail" />
					<div runat="server" id="ordinanceTabs" class="nav nav-tabs" role="tablist">
						<button runat="server" id="factSheetTab" class="nav-link ordTabs active" data-toggle="tab" data-target="#factSheetPane" type="button" role="tab">Fact Sheet</button>
						<button runat="server" id="auditTab" class="nav-link ordTabs" data-toggle="tab" data-target="#auditPane" type="button" role="tab" onclick="FormatAudit();">History</button>
					</div>

					<div id="ordinanceTabsContent" class="tab-content p-0 border-0">
						<div runat="server" id="factSheetPane" class="tab-pane fade active show" role="tabpanel">
							<%-- FORM HEADER --%>
							<section class="container form-header bg-body text-center position-relative tab-border-header">
								<div class="row h-100 align-items-center">
									<h1><span class="fas fa-book-section"></span>&nbsp;Ordinance</h1>
								</div>
								<asp:Label runat="server" ID="lblOrdID" CssClass="text-primary-emphasis ordID">ID:</asp:Label>
								<asp:LinkButton runat="server" ID="backBtn" CssClass="btn bg-danger backBtn" OnClick="backBtn_Click"><span class="fas fa-xmark text-light"></span></asp:LinkButton>
								<asp:LinkButton runat="server" ID="copyOrd" CssClass="btn btn-primary copyBtn" OnClick="copyOrd_Click"><span class="fas fa-copy"></span>&nbsp;Copy</asp:LinkButton>

								<div class="statusDropDown text-start">
									<div runat="server" id="ddStatusDiv" class="form-group text-start w-75 me-auto">
										<label for="ddStatus">Status</label>
										<asp:DropDownList ID="ddStatus" runat="server" AutoPostBack="true" CssClass="form-select" required="true" ValidateRequestMode="Enabled" OnSelectedIndexChanged="ddStatus_SelectedIndexChanged"></asp:DropDownList>
										<asp:HiddenField runat="server" ID="hdnOrdStatusID" />
										<asp:HiddenField runat="server" ID="hdnStatusID" />
									</div>
									<div runat="server" id="statusDiv" class="d-flex fw-bold fs-4 justify-content-start">
										<label runat="server" id="statusLabel"></label>
										<label runat="server" id="statusIcon"></label>
									</div>
								</div>
							</section>

							<%-- FORM BODY --%>
							<div class="container form-page bg-body-tertiary tab-border-body">

								<div class="px-2 py-4">
									<%-- REQUIRED FIELD DESCRIPTOR --%>
									<div class="row">
										<p runat="server" id="requiredFieldDescriptor" class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i>&nbsp;= Required Field</p>
									</div>

									<%-- ORDINANCE NUMBER --%>
									<div runat="server" id="ordinanceNumberDiv" class="row mt-3">
										<div class="col-md-4">
											<div class="form-group">
													<label for="ordinanceNumber">Ordinance Number</label>
													<asp:TextBox runat="server" ID="ordinanceNumber" CssClass="form-control" TextMode="SingleLine" placeholder="123-45-6789" AutoCompleteType="Disabled"></asp:TextBox>
												</div>
										</div>
									</div>

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
											<div class="col-md-5">
												<div class="form-group">
													<label for="requestContact">Requesting Contact</label>
													<asp:TextBox runat="server" ID="requestContact" CssClass="form-control" TextMode="SingleLine" placeholder="John Doe" AutoCompleteType="DisplayName" required="true"></asp:TextBox>
												</div>
											</div>

											<%-- EMAIL --%>
											<div class="col-md-3">
												<div class="form-group">
													<label for="requestEmail">Email</label>
													<asp:TextBox runat="server" ID="requestEmail" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" required="true"></asp:TextBox>
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
													<div class="radioListDiv" id="epList">
														<%-- YES --%>
														<div class="form-check form-check-inline">
															<label for="epYes">Yes</label>
															<asp:RadioButton runat="server" ID="epYes" CssClass="form-check-input" GroupName="epList" OnCheckedChanged="EPCheckedChanged" AutoPostBack="true" />
														</div>

														<%-- NO --%>
														<div class="form-check form-check-inline">
															<label for="epNo">No</label>
															<asp:RadioButton runat="server" ID="epNo" CssClass="form-check-input" GroupName="epList" OnCheckedChanged="EPCheckedChanged" AutoPostBack="true" />
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

														<%-- SEPARATOR --%>
														<div class="input-group-append">
															<span class="input-group-text date-period-separator"><i class="fas fa-minus"></i></span>
														</div>

														<%-- END --%>
														<asp:TextBox runat="server" ID="contractEndDate" CssClass="form-control" TextMode="Date" data-type="datePeriodEnd"></asp:TextBox>
													</div>
												</div>
											</div>

											<%-- DATE TERM --%>
											<div class="col-md-3">
												<div class="form-group">
													<label for="dateTerm">Date Term</label>
													<input runat="server" id="contractTerm" type="text" data-type="dateTerm" class="form-control locked-field" autocomplete="off" readonly="readonly" value="" placeholder="Calculating Term..." required>
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
													<div class="radioListDiv" id="scopeChangeList">
														<%-- YES --%>
														<div class="form-check form-check-inline">
															<label for="scYes">Yes</label>
															<asp:RadioButton runat="server" ID="scYes" CssClass="form-check-input" GroupName="scopeChangeList" OnCheckedChanged="SCCheckedChanged" AutoPostBack="true" />
														</div>

														<%-- NO --%>
														<div class="form-check form-check-inline">
															<label for="scNo">No</label>
															<asp:RadioButton runat="server" ID="scNo" CssClass="form-check-input" GroupName="scopeChangeList" OnCheckedChanged="SCCheckedChanged" AutoPostBack="true" />
														</div>
													</div>
												</div>
											</div>
										</div>

										<%-- SECOND ROW --%>
										<div runat="server" id="scopeChangeOptions" class="row mb-3">
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
										<div class="row mb-5">
											<%-- PURCHASE METHOD --%>
											<div class="col-md-5">
												<div class="form-group">
													<label for="purchaseMethod">Method of Purchase</label>
													<asp:DropDownList ID="purchaseMethod" runat="server" OnSelectedIndexChanged="PurchaseMethodSelectedIndexChanged" AutoPostBack="true" CssClass="form-select" required="true"></asp:DropDownList>
												</div>
											</div>

											<%-- OTHER / EXCEPTION --%>
											<div class="col-md-4">
												<div runat="server" id="otherExceptionDiv" class="form-group">
													<label for="otherException">Other/Exception</label>
													<asp:TextBox runat="server" ID="otherException" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled"></asp:TextBox>
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
															<asp:RadioButton runat="server" ID="paApprovalRequiredYes" CssClass="form-check-input" GroupName="paApprovalRequiredList" />
														</div>

														<%-- NO --%>
														<div class="form-check form-check-inline">
															<label for="paApprovalRequiredNo">No</label>
															<asp:RadioButton runat="server" ID="paApprovalRequiredNo" CssClass="form-check-input" GroupName="paApprovalRequiredList" />
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
															<asp:RadioButton runat="server" ID="paApprovalAttachedYes" CssClass="form-check-input" GroupName="paApprovalAttachedList" />
														</div>

														<%-- NO --%>
														<div class="form-check form-check-inline">
															<label for="paApprovalAttachedNo">No</label>
															<asp:RadioButton runat="server" ID="paApprovalAttachedNo" CssClass="form-check-input" GroupName="paApprovalAttachedList" />
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
														<asp:Repeater runat="server" ID="rpRevenueTable" OnItemCommand="rpAccountingTable_ItemCommand">
															<ItemTemplate>
																<tr class="upperCaseField">
																	<td style="vertical-align: middle">
																		<asp:HiddenField runat="server" ID="hdnRevID" Value='<%# DataBinder.Eval(Container.DataItem, "AccountingID") %>' />
																		<asp:HiddenField runat="server" ID="hdnRevIndex" Value='<%# Container.ItemIndex %>' />
																		<%--<asp:DropDownList ID="revenueFundCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# fundCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "FundCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="revenueFundCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "FundCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="revenueAgencyCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# agencyCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "DepartmentCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="revenueAgencyCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "DepartmentCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="revenueOrgCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# orgCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "UnitCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="revenueOrgCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "UnitCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="revenueActivityCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# activityCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "ActivityCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="revenueActivityCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ActivityCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="revenueObjectCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# objectCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "ObjectCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="revenueObjectCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ObjectCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td class="position-relative" style="vertical-align: middle">
																		<asp:TextBox runat="server" ID="revenueAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$10,000.00" AutoCompleteType="Disabled" Text='<%# (Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Amount")) >= 0)?DataBinder.Eval(Container.DataItem, "Amount"):string.Empty%>'></asp:TextBox>

																		<div runat="server" id="removeRevRowDiv">
																			<asp:Button runat="server" ID="removeRevenueRow" CssClass="btn row-delete" UseSubmitBehavior="false" CommandName="delete" CommandArgument="ordRevTable" Text="&#xf068;" />
																		</div>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>

												<%-- ADD REVENUE ROW BUTTON --%>
												<div runat="server" id="newRevenueRowDiv" class="text-center w-100">
													<asp:Button runat="server" ID="newRevenueRow" CssClass="btn btn-success w-100 row-add" OnClick="newAccountingRow_ServerClick" UseSubmitBehavior="false" CommandName="ordRevTable" Text="Add Row" />
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
														<asp:Repeater runat="server" ID="rpExpenditureTable" OnItemCommand="rpAccountingTable_ItemCommand">
															<ItemTemplate>
																<tr class="upperCaseField">
																	<td style="vertical-align: middle">
																		<asp:HiddenField runat="server" ID="hdnExpID" Value='<%# DataBinder.Eval(Container.DataItem, "AccountingID") %>' />
																		<asp:HiddenField runat="server" ID="hdnExpIndex" Value='<%# Container.ItemIndex %>' />
																		<%--<asp:DropDownList ID="expenditureFundCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# fundCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "FundCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="expenditureFundCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "FundCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="expenditureAgencyCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# agencyCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "DepartmentCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="expenditureAgencyCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "DepartmentCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="expenditureOrgCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# orgCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "UnitCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="expenditureOrgCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "UnitCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="expenditureActivityCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# activityCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "ActivityCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="expenditureActivityCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ActivityCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td style="vertical-align: middle">
																		<%--<asp:DropDownList ID="expenditureObjectCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# objectCodes %>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "ObjectCode") %>'></asp:DropDownList>--%>
																		<asp:TextBox runat="server" ID="expenditureObjectCode" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" Text='<%# DataBinder.Eval(Container.DataItem, "ObjectCode") %>' required="true"></asp:TextBox>
																	</td>
																	<td class="position-relative" style="vertical-align: middle">
																		<asp:TextBox runat="server" ID="expenditureAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$10,000.00" AutoCompleteType="Disabled" Text='<%# (Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Amount")) >= 0)?DataBinder.Eval(Container.DataItem, "Amount"):string.Empty%>'></asp:TextBox>

																		<div runat="server" id="removeExpRowDiv">
																			<asp:Button runat="server" ID="removeExpenditureRow" CssClass="btn row-delete" UseSubmitBehavior="false" CommandName="delete" CommandArgument="ordExpTable" Text="&#xf068;" />
																		</div>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>

												<%-- ADD EXPENDITURE ROW BUTTON --%>
												<div runat="server" id="newExpenditureRowDiv" class="text-center w-100">
													<asp:Button runat="server" ID="newExpenditureRow" CssClass="btn btn-success w-100 row-add" OnClick="newAccountingRow_ServerClick" UseSubmitBehavior="false" CommandName="ordExpTable" Text="Add Row" />
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
													<asp:TextBox runat="server" ID="staffAnalysis" CssClass="form-control" TextMode="Multiline" Rows="18" AutoCompleteType="Disabled" required="true"></asp:TextBox>
												</div>
											</div>
										</div>

										<%-- SECOND ROW --%>
										<div runat="server" id="supportingDocumentationDiv" class="row mb-3">
											<%-- SUPPORTING DOCUMENTATION --%>
											<div class="col-md-6">
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
																		<asp:LinkButton runat="server" ID="supportingDocDownload" CssClass="btn btn-primary" CommandName="download" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DocumentName") %>' Style="margin-right: 5px;"><span class="fas fa-download"></span></asp:LinkButton>
																		<asp:LinkButton runat="server" ID="deleteFile" CssClass="btn btn-danger" CommandName="delete" Style="margin-left: 5px;"><span class="fas fa-trash-can"></span></asp:LinkButton>
																	</div>
																</li>
															</ItemTemplate>
														</asp:Repeater>
													</ul>
													<div id="supportingDocumentationGroup" class="d-flex">
														<asp:FileUpload runat="server" ID="supportingDocumentation" CssClass="form-control mt-3" AllowMultiple="true" onchange="SetUploadActive();" />
														<asp:Button runat="server" ID="UploadDocBtn" UseSubmitBehavior="false" CssClass="btn btn-success mt-3 ms-3" Width="25%" Text="Upload" OnClick="UploadDocBtn_Click" disabled="disabled" />
													</div>
												</div>
											</div>
										</div>
									</div>

									<%-- TENTH SECTION --%>
									<div runat="server" id="signatureSection" class="form-section">
										<%-- FIRST ROW --%>
										<div class="row mb-3">
											<%-- FUNDS CHECK BY --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="fundsCheckByGroup" class="mb-1">Funds Check By <asp:LinkButton runat="server" ID="fundsCheckEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" data-toggle="modal" data-target="#signatureEmailModal" OnClientClick="setEmailModal('fundsCheckByBtn', 'Funds Check By');"><span class="fas fa-envelope"></span>&nbsp;Send Request Email</asp:LinkButton></label>
													<div id="fundsCheckByGroup">
														<div runat="server" id="fundsCheckByBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="fundsCheckByBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('fundsCheckBy');" />
															</div>
														</div>
														<div runat="server" id="fundsCheckByInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="fundsCheckBySig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" required="true" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="fundsCheckByDate" CssClass="form-control" TextMode="Date" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-6 d-flex align-items-center">
																<asp:Label runat="server" ID="fundsCheckByCertified" CssClass="text-success m-0"><span class="fas fa-check"></span>&nbsp;Signature Certified!</asp:Label>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>

										<%-- SECOND ROW --%>
										<div class="row mb-3">
											<%-- DIRECTOR/SUPERVISOR --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="directorSupervisorGroup">Director/Supervisor <asp:LinkButton runat="server" ID="directorSupervisorEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" data-toggle="modal" data-target="#signatureEmailModal" OnClientClick="setEmailModal('directorSupervisorBtn', 'Director/Supervisor');"><span class="fas fa-envelope"></span>&nbsp;Send Request Email</asp:LinkButton></label>
													<div id="directorSupervisorGroup">
														<div runat="server" id="directorSupervisorBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="directorSupervisorBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('directorSupervisor');" />
															</div>
														</div>
														<div runat="server" id="directorSupervisorInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="directorSupervisorSig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" required="true" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="directorSupervisorDate" CssClass="form-control" TextMode="Date" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-6 d-flex align-items-center">
																<asp:Label runat="server" ID="directorSupervisorCertified" CssClass="text-success m-0"><span class="fas fa-check"></span>&nbsp;Signature Certified!</asp:Label>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>

										<%-- THIRD ROW --%>
										<div class="row mb-3">
											<%-- CITY PURCHASING AGENT --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="cPAGroup">City Purchasing Agent <asp:LinkButton runat="server" ID="cPAEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" data-toggle="modal" data-target="#signatureEmailModal" OnClientClick="setEmailModal('cPABtn', 'City Purchasing Agent');"><span class="fas fa-envelope"></span>&nbsp;Send Request Email</asp:LinkButton></label>
													<div id="cPAGroup">
														<div runat="server" id="cPABtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="cPABtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('cPA');" />
															</div>
														</div>
														<div runat="server" id="cPAInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="cPASig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" required="true" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="cPADate" CssClass="form-control" TextMode="Date" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-6 d-flex align-items-center">
																<asp:Label runat="server" ID="cPACertified" CssClass="text-success m-0"><span class="fas fa-check"></span>&nbsp;Signature Certified!</asp:Label>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>

										<%-- FOURTH ROW --%>
										<div class="row mb-3">
											<%-- OBM DIRECTOR --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="obmDirectorGroup">OBM Director <asp:LinkButton runat="server" ID="obmDirectorEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" data-toggle="modal" data-target="#signatureEmailModal" OnClientClick="setEmailModal('obmDirectorBtn', 'OBM Director');"><span class="fas fa-envelope"></span>&nbsp;Send Request Email</asp:LinkButton></label>
													<div id="obmDirectorGroup">
														<div runat="server" id="obmDirectorBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="obmDirectorBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('obmDirector');" />
															</div>
														</div>
														<div runat="server" id="obmDirectorInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="obmDirectorSig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" required="true" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="obmDirectorDate" CssClass="form-control" TextMode="Date" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-6 d-flex align-items-center">
																<asp:Label runat="server" ID="obmDirectorCertified" CssClass="text-success m-0"><span class="fas fa-check"></span>&nbsp;Signature Certified!</asp:Label>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>

										<%-- FIFTH ROW --%>
										<div class="row mb-3">
											<%-- MAYOR --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="mayorGroup">Mayor <asp:LinkButton runat="server" ID="mayorEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" data-toggle="modal" data-target="#signatureEmailModal" OnClientClick="setEmailModal('mayorBtn', 'Mayor');"><span class="fas fa-envelope"></span>&nbsp;Send Request Email</asp:LinkButton></label>
													<div id="mayorGroup">
														<div runat="server" id="mayorBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="mayorBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('mayor');" />
															</div>
														</div>
														<div runat="server" id="mayorInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="mayorSig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" required="true" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="mayorDate" CssClass="form-control" TextMode="Date" ReadOnly="true"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-6 d-flex align-items-center">
																<asp:Label runat="server" ID="mayorCertified" CssClass="text-success m-0"><span class="fas fa-check"></span>&nbsp;Signature Certified!</asp:Label>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>
									</div>

									<%-- SUBMIT SECTION --%>
									<div runat="server" id="submitSection" class="form-section">
										<%-- FIRST ROW --%>
										<div class="row mt-3 mb-3 text-center">
											<%-- SAVE BUTTON --%>
											<div class="col-md-6">
												<asp:Button runat="server" ID="SaveFactSheet" UseSubmitBehavior="true" CssClass="btn btn-primary float-end" Width="50%" Text="Save" OnClick="SaveFactSheet_Click" OnClientClick="ShowSubmitToast();" />
											</div>
											<%-- DELETE BUTTON --%>
											<div class="col-md-6">
												<asp:Button runat="server" ID="DeleteFactSheet" UseSubmitBehavior="false" CssClass="btn btn-danger float-start" Width="50%" Text="Delete" data-toggle="modal" data-target="#deleteModal" />
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
						<div runat="server" id="auditPane" class="tab-pane fade" role="tabpanel">
							<div class="card b-0">
								<div class="card-header bg-body">
									<h3><i class="fas fa-clock-rotate-left"></i>&nbsp;History</h3>
								</div>

								<div class="card-body bg-body-tertiary">
									<table id="historyTable" class="table table-bordered table-standard table-hover text-center" style="padding: 0px; margin: 0px">
										<thead>
											<tr>
												<th style="width: 100%; text-align: left">
													<strong>Date &mdash; Modified By &mdash; Modification Type</strong>
													<div class="float-end">
														<strong class="mx-2"><span class="fas fa-plus text-success"></span> Added</strong>
														<strong class="mx-2"><span class="fas fa-arrow-right-long text-warning-light"></span> Modified</strong>
														<strong class="mx-2"><span class="fas fa-minus text-danger"></span> Removed</strong>
													</div>
												</th>
											</tr>
										</thead>
										<tbody>
											<asp:Repeater runat="server" ID="rpAudit" OnItemDataBound="rpAudit_ItemDataBound">
												<ItemTemplate>
													<tr id='auditRow<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>'>
														<asp:HiddenField runat="server" ID="hdnAuditItem" Value='<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>' />
														<td class="align-middle text-start mw-0">
															<a href="javascript:void(0);" class="btn-accordion audit-link" data-toggle="collapse" data-target='<%# DataBinder.Eval(Container.DataItem, "UpdateType").Equals("UPDATED") ?  $"#auditItem{DataBinder.Eval(Container.DataItem, "OrdinanceAuditID")}" : string.Empty%>'>
																<p class="m-0"><%# DataBinder.Eval(Container.DataItem, "LastUpdateDate", "{0:MM/dd/yyyy}") %> &mdash; <%# DataBinder.Eval(Container.DataItem, "LastUpdateBy") %> &mdash; <%# DataBinder.Eval(Container.DataItem, "UpdateType") %><span runat="server" class='<%# DataBinder.Eval(Container.DataItem, "UpdateType").Equals("UPDATED") ? "float-end lh-1p5 fas fa-chevron-down" : string.Empty %>'></span></p>
															</a>
															<div id='auditItem<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>' class="collapse border-top mt-2 pt-3" data-parent='#auditRow<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>'>
																<p class="m-0">Changes:</p>
																<ul class="auditList">
																	<asp:Repeater runat="server" ID="rpAuditDesc">
																		<ItemTemplate>
																			<li><%# Container.DataItem %></li>
																		</ItemTemplate>
																	</asp:Repeater>
																</ul>
															</div>
														</td>
													</tr>
												</ItemTemplate>
											</asp:Repeater>
										</tbody>
									</table>
								</div>
								<div class="card-footer p-0">
									<asp:Panel ID="pnlAuditPagingP" CssClass="panel m-0" runat="server" Visible="false">
										<table class="table m-0" runat="server">
											<tr>
												<td class="text-left">
													<asp:LinkButton ID="lnkAuditFirstSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="first" data-list="auditTable" style="width: 150px;" causesvalidation="false"><i class="fas fa-angles-left"></i>&nbsp;First</asp:LinkButton>
												</td>
												<td class="text-center">
													<asp:LinkButton ID="lnkAuditPreviousSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="previous" data-list="auditTable" style="width: 150px;" causesvalidation="false"><i class="fas fa-angle-left"></i>&nbsp;Previous</asp:LinkButton>
												</td>
												<td class="text-center">
													<div style="margin-top: 5px">
														<asp:Label Style="font-weight: bold; font-size: 18px" ID="lblAuditCurrentPageBottomSearchP" runat="server"></asp:Label>
													</div>
												</td>
												<td class="text-center">
													<asp:LinkButton ID="lnkAuditNextSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="next" data-list="auditTable" style="width: 150px;" causesvalidation="false">Next&nbsp;<i class="fas fa-angle-right"></i></asp:LinkButton>
												</td>
												<td class="text-end">
													<asp:LinkButton ID="lnkAuditLastSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="last" data-list="auditTable" style="width: 150px;" causesvalidation="false">Last&nbsp;<i class="fas fa-angles-right"></i></asp:LinkButton>
												</td>
											</tr>
										</table>
									</asp:Panel>
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
				<div class="modal-body">
					<asp:Label runat="server" ID="deleteLabel" Style="font-size: 18px; font-weight: bold" CssClass="text-danger" Text="Are you sure you want to delete this ordinance fact sheet? (This cannot be undone)" />
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
					<asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" CausesValidation="false" UseSubmitBehavior="false" Visible="true" OnClick="mdlDeleteSubmit_ServerClick" OnClientClick="ShowSubmitToast();" />
				</div>
			</div>
		</div>
	</div>

	<!-- SIGNATURE MODAL -->
	<div class="modal fade" id="signatureModal" tabindex="-1" role="dialog" aria-labelledby="signatureModalLabel">
		<div class="modal-dialog" role="document" style="max-width: 750px;">
			<div class="modal-content">
				<div class="modal-header">
					<h4 class="modal-title" id="signatureModalLabel">Signature</h4>
				</div>
				<div class="modal-body bg-body-tertiary">
					<div class="row mb-5">
						<div class="col-md-8">
							<div class="input-group">
								<span class="input-group-text fas fa-signature"></span>
								<asp:TextBox runat="server" ID="sigName" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" placeholder="John Doe"></asp:TextBox>
							</div>
						</div>
						<div class="col-md-4">
							<div class="input-group">
								<span class="input-group-text fas fa-calendar-days"></span>
								<asp:TextBox runat="server" ID="sigDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
							</div>
						</div>
					</div>
					<div class="row">
						<div class="col-md-12">
							<div class="form-group">
								<div class="form-check form-check-inline">
									<label for="certifySig" class="ps-3">
										By checking this box and clicking the "Sign Document" button, I affirm that I am the individual named above, or that I am authorized to sign on behalf of that individual. I understand that this action constitutes the electronic equivalent of the signature of the named individual for the purposes of this document. I certify that, to the best of my knowledge, the information provided is accurate, and I understand that any false representation may result in disciplinary action.
									</label>
									<asp:CheckBox runat="server" ID="certifySig" CssClass="form-check-input" style="transform: scale(1.5); margin-left: -1em;"/>
								</div>
							</div>
						</div>
					</div>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
					<asp:Button ID="btnSignDoc" runat="server" Text="Sign Document" CssClass="btn btn-success" CausesValidation="false" UseSubmitBehavior="false" Visible="true" data-dismiss="modal" OnClick="btnSignDoc_Click" disabled="disabled" />
					<input runat="server" id="sigType" type="hidden" name="sigType"  />
				</div>
			</div>
		</div>
	</div>

	<!-- SIGNATURE EMAIL MODAL -->
	<div class="modal fade" id="signatureEmailModal" tabindex="-1" role="dialog" aria-labelledby="signatureEmailModalLabel">
		<div class="modal-dialog" role="document" style="max-width: 750px;">
			<div class="modal-content">
				<div class="modal-header">
					<h4 class="modal-title" id="signatureEmailModalLabel">Signature Request Email</h4>
				</div>
				<div class="modal-body bg-body-tertiary">
					<div class="row mb-2">
						<div class="col-md-8">
							<div class="input-group">
								<span class="input-group-text fas fa-address-book"></span>
								<asp:TextBox runat="server" ID="signatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com"></asp:TextBox>
							</div>
						</div>
					</div>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
					<asp:Button ID="btnSendSigEmail" runat="server" Text="Send" CssClass="btn btn-success" CausesValidation="false" UseSubmitBehavior="false" Visible="true" OnClick="btnSendSigEmail_Click" data-dismiss="modal" OnClientClick="ShowEmailToast();" />
					<input runat="server" id="sigBtnTarget" type="hidden" name="sigBtnTarget"  />
					<input runat="server" id="sigBtnLabel" type="hidden" name="sigBtnLabel"  />
				</div>
			</div>
		</div>
	</div>

	<%-- JAVASCRIPT --%>
	<script>
		FormatForms();
		SetTooltips();
		scrollToElement();

		var prm = Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			GetToastStatus();
			FormatForms();
			CurrencyFormatting();
			SetTooltips();
			DisableDDInitialOption();
			scrollToElement();
			HideAllTooltips();
		});

		function DisableDDInitialOption() {
			var ddStatus = document.getElementById('<%= ddStatus.ClientID %>');
			var ddDepartment = document.getElementById('<%= requestDepartment.ClientID %>');
			var ddMethod = document.getElementById('<%= purchaseMethod.ClientID %>');
			if (ddStatus != null) {
				if (ddStatus.options[0].selected) {
					ddStatus.style.color = "rgb(from var(--bs-body-color) r g b / 75%)"
				}
				else {
					ddStatus.style.color = "unset";
				}
				ddStatus.options[0].disabled = true;
			}
			if (ddDepartment != null) {
				if (ddDepartment.options[0].selected) {
					ddDepartment.style.color = "rgb(from var(--bs-body-color) r g b / 75%)"
				}
				else {
					ddDepartment.style.color = "unset";
				}
				ddDepartment.options[0].disabled = true;
			}
			if (ddMethod != null) {
				if (ddMethod.options[0].selected) {
					ddMethod.style.color = "rgb(from var(--bs-body-color) r g b / 75%)"
				}
				else {
					ddMethod.style.color = "unset";
				}
				ddMethod.options[0].disabled = true;
			}
		}

		function SetTooltips() {
			var tooltipTitles = $('[data-overflow-tooltip="true"]');
			$(tooltipTitles).each(function (i) {
				if (this.scrollWidth > this.offsetWidth) {
					$(this).tooltip();
				}				
			});
			$('[data-action-tooltip="true"]').tooltip();
		}

		function HideAllTooltips() {
			$('.tooltip.show').tooltip('hide');
		}

		function CurrencyFormatting() {
			$("[data-type='currency']").each(function () {
				formatCurrency($(this), "blur");
			});
		}

		function OrdinanceVisibility(fadeOut) {
			var dataString = JSON.stringify({ fadeOut: fadeOut });
			var valArray = [];
			$.ajax({
				type: "POST",
				async: false,
				url: "./Pages/OrdinanceTracking/Ordinances.aspx.cs/OrdVisibility",
				data: dataString,
				contentType: "application/json",
				dataType: "json"
			});
		}
		
		function OrdTableFadeOut() {
			var ordTable = document.getElementById('<%= ordTable.ClientID %>')
			var ordView = document.getElementById('<%= ordView.ClientID %>')
			
			$(ordTable).fadeOut(500);
			setTimeout(() => {
				$(ordView).fadeIn(500);
			}, 500);
			//setTimeout(() => {
			//	OrdinanceVisibility("table");
			//}, 1000);
		}
		function OrdTableFadeIn() {
			var ordTable = document.getElementById('<%= ordTable.ClientID %>')
			var ordView = document.getElementById('<%= ordView.ClientID %>')
			$(ordView).fadeOut(500);
			setTimeout(() => {
				$(ordTable).fadeIn(500);
			}, 500);
			//setTimeout(() => {
			//	OrdinanceVisibility("ord");
			//}, 1000);
		}

		function SetUploadActive() {
			const supportingDocumentation = document.getElementById('<%= supportingDocumentation.ClientID %>')
			var UploadDocBtn = document.getElementById('<%= UploadDocBtn.ClientID %>')
			if (supportingDocumentation.files.length > 0) {
				UploadDocBtn.disabled = false;
			}
			else {
				UploadDocBtn.disabled = true;
			}
			
		}

		function setEmailModal(btnID, btnLabel) {
			const sigBtnTarget = $('#<%= sigBtnTarget.ClientID %>');
			const sigBtnLabel = $('#<%= sigBtnLabel.ClientID %>');
			$(sigBtnTarget).attr("value", btnID);
			$(sigBtnLabel).attr("value", btnLabel);
			$('#<%= signatureEmailAddress.ClientID %>').val('');
		}

		function scrollToElement() {
			const urlParams = new URLSearchParams(window.location.search);
			if (urlParams.get('f')) {
				setTimeout(() => document.getElementById(urlParams.get('f')).scrollIntoView(), 500);
				document.getElementById(urlParams.get('f')).focus();
				//setTimeout(() => , 500);
			}
		}

		function ShowEmailToast() {
			$('#submitToast').removeClass("text-bg-danger");
			$('#submitToast').addClass("text-bg-success");

			$('#toastMessage').html("Email Sent!");
			ShowSubmitToast();
			GetToastStatus();
		}

		function setSigModal(typeOfSig) {
			const sigType = $('#<%= sigType.ClientID %>');
			$(sigType).attr("value", typeOfSig);
			$('#<%= sigName.ClientID %>').val('');
			$('#<%= sigDate.ClientID %>').val('');
			$('#<%= btnSignDoc.ClientID %>').prop('disabled', true);
			$('#<%= certifySig.ClientID %>').prop('checked', false);
		}

		function testMultiple() {
			console.log("Working");
		}

		function FormatAudit() {
			$("[data-type='Decimal']").each(function () {
				formatCurrencyDecimal($(this), "blur");
			});
		}

		$('#<%= sigName.ClientID %>').on('change keyup', function () {
			if ($('#<%= certifySig.ClientID %>').prop('checked') == true && $('#<%= sigName.ClientID %>').val().length > 0 && $('#<%= sigDate.ClientID %>').val().length > 0) {
				$('#<%= btnSignDoc.ClientID %>').prop('disabled', false);
			}
			else {
				$('#<%= btnSignDoc.ClientID %>').prop('disabled', true);
			}
		});
		$('#<%= sigDate.ClientID %>').on('change keyup', function () {
			if ($('#<%= certifySig.ClientID %>').prop('checked') == true && $('#<%= sigName.ClientID %>').val().length > 0 && $('#<%= sigDate.ClientID %>').val().length > 0) {
				$('#<%= btnSignDoc.ClientID %>').prop('disabled', false);
			}
			else {
				$('#<%= btnSignDoc.ClientID %>').prop('disabled', true);
			}
		});
		$('#<%= certifySig.ClientID %>').on('change', function () {
			if ($('#<%= certifySig.ClientID %>').prop('checked') == true && $('#<%= sigName.ClientID %>').val().length > 0 && $('#<%= sigDate.ClientID %>').val().length > 0) {
				$('#<%= btnSignDoc.ClientID %>').prop('disabled', false);
			}
			else {
				$('#<%= btnSignDoc.ClientID %>').prop('disabled', true);
			}
		});
	</script>
</asp:Content>