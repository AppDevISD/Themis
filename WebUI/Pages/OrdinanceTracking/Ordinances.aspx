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

				<asp:AsyncPostBackTrigger ControlID="filterDepartment" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="filterDivision" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="filterStatus" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="btnFilterSearchTitle" EventName="Click" />

				<asp:AsyncPostBackTrigger ControlID="epYes" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="epNo" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="scYes" EventName="CheckedChanged" />
				<asp:AsyncPostBackTrigger ControlID="scNo" EventName="CheckedChanged" />

				<asp:AsyncPostBackTrigger ControlID="purchaseMethod" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="ddStatus" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="signatureEmailAddress" EventName="TextChanged" />

				<asp:AsyncPostBackTrigger ControlID="sortDate" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortTitle" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortDepartmentDivision" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="ddDeptDivision" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="sortContact" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="sortStatus" EventName="Click" />				
				<asp:AsyncPostBackTrigger ControlID="btnSendSigEmail" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnSignDoc" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnCancelRejection" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="fundsCheckEmailBtn" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="directorSupervisorEmailBtn" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="cPAEmailBtn" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="obmDirectorEmailBtn" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="mayorEmailBtn" EventName="Click" />
				
				<asp:PostBackTrigger ControlID="btnSendRejection" />

				<asp:PostBackTrigger ControlID="UploadDocBtn" />
				<asp:PostBackTrigger ControlID="SaveFactSheet" />

				<asp:AsyncPostBackTrigger ControlID="lnkInactivityRefresh" EventName="Click" />
			</Triggers>

			<ContentTemplate>
				<div runat="server" id="ordTable" class="card">
					<div class="card-header bg-body-secondary">
						<h3><i class="fas fa-book-section"></i>&nbsp;Ordinances</h3>
					</div>
					<div class="card-body bg-body-tertiary">
						<%-- FILTERS & SORTING --%>
						<div class="row mb-4">
							<div class="col-md-3" runat="server" id="filterDepartmentDiv">
								<div class="form-group">
									<label for="filterDepartment">Filter by Department</label>
									<asp:DropDownList ID="filterDepartment" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="department" onchange="showLoadingModal();" data-filter="true" ></asp:DropDownList>
								</div>
							</div>
							<div class="col-md-3" runat="server" id="filterDivisionDiv">
								<div class="form-group">
									<label for="filterDivision">Filter by Division</label>
									<asp:DropDownList ID="filterDivision" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="division" onchange="showLoadingModal();" data-filter="true" ></asp:DropDownList>
								</div>
							</div>
							<div class="col-md-3" id="filterStatusDiv">
								<div class="form-group">
									<label for="filterStatus">Filter by Status</label>
									<asp:DropDownList ID="filterStatus" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="Filter_SelectedIndexChanged" data-command="status" onchange="showLoadingModal();" data-filter="true" ></asp:DropDownList>
								</div>
							</div>
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
										<th style="width: 4%; text-align: center"><asp:LinkButton runat="server" ID="sortID" data-command="OrdinanceID" data-text="ID" OnClick="SortBtn_Click" class="btn btn-sort" TabIndex="-1"><strong>ID<span runat="server" class='float-end lh-1p5'></span></strong></asp:LinkButton></th>
										<th style="width: 6%; text-align: center"><asp:LinkButton runat="server" ID="sortDate" data-command="EffectiveDate" data-text="Date" OnClick="SortBtn_Click" class="btn btn-sort" TabIndex="-1"><strong>Date<span runat="server" class='float-end lh-1p5 fas fa-arrow-down'></span></strong></asp:LinkButton></th>
										<th style="width: 34%; text-align: center"><asp:LinkButton runat="server" ID="sortTitle" data-command="OrdinanceTitle" data-text="Title" OnClick="SortBtn_Click" class="btn btn-sort" TabIndex="-1"><strong>Title<span class="float-end lh-1p5 me-1"></span></strong></asp:LinkButton></th>
										<th style="width: 25%; text-align: center">
											<div style="position: relative;">
												<asp:LinkButton runat="server" ID="sortDepartmentDivision" data-command="RequestDepartment" data-text="Department" OnClick="SortBtn_Click" class="btn-sort btn-dd-sort" TabIndex="-1"><strong><span class="float-end lh-1p5"></span></strong></span></asp:LinkButton>
												<asp:DropDownList runat="server" ID="ddDeptDivision" CssClass="form-select dd-sort" OnSelectedIndexChanged="ddDeptDivision_SelectedIndexChanged" AutoPostBack="true" TabIndex="-1" >
													<asp:ListItem Text="Department" Value="RequestDepartment"></asp:ListItem>
													<asp:ListItem Text="Division" Value="RequestDivision"></asp:ListItem>
												</asp:DropDownList>
											</div>
										</th>
										
										<th style="width: 15%; text-align: center"><asp:LinkButton runat="server" ID="sortContact" data-command="RequestContact" data-text="Contact" OnClick="SortBtn_Click" class="btn btn-sort" TabIndex="-1"><strong>Contact<span class="float-end lh-1p5"></span></strong></asp:LinkButton></th>
										<th style="width: 10%; text-align: center"><asp:LinkButton runat="server" ID="sortStatus" data-command="StatusDescription" data-text="Status" OnClick="SortBtn_Click" class="btn btn-sort" TabIndex="-1"><strong>Status<span class="float-end lh-1p5"></span></strong></asp:LinkButton></th>
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
												<asp:Label ID="ordTableDivision" Text='<%# DataBinder.Eval(Container.DataItem, "RequestDivision") %>' runat="server" Visible="false" />
											</td>
											<td class="align-middle">
												<asp:Label ID="ordTableContact" Text='<%# DataBinder.Eval(Container.DataItem, "RequestContact") %>' runat="server" />
											</td>
											<td class="align-middle">
												<asp:Label ID="ordTableStatus" Text='<%# DataBinder.Eval(Container.DataItem, "StatusDescription") %>' runat="server" />
											</td>
											<td class="align-middle d-flex justify-content-center px-0">
												<asp:LinkButton runat="server" ID="editOrd" CommandName="edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' CssClass="ordActionBtn border-end px-2" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="Edit" Visible='<%# adminUnlockedOrd(DataBinder.Eval(Container.DataItem, "StatusDescription").ToString()) %>' OnClientClick="showLoadingModal();" TabIndex="-1"><i class="fas fa-pen-to-square text-warning-light"></i></asp:LinkButton>
												<asp:LinkButton runat="server" ID="viewOrd" CommandName="view" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' CssClass="ordActionBtn border-end px-2" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="View" OnClientClick="showLoadingModal();" TabIndex="-1"><i class="fas fa-magnifying-glass text-info"></i></asp:LinkButton>
												<asp:LinkButton runat="server" ID="downloadOrd" CommandName="download" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrdinanceID") %>' CssClass="ordActionBtn px-2" data-action-tooltip="true" data-tooltip="tooltip" data-placement="top" title="Download" TabIndex="-1"><i class="fas fa-download text-primary"></i></asp:LinkButton>
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
										<asp:LinkButton ID="lnkFirstSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="first" data-list="ordTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1"><i class="fas fa-angles-left"></i>&nbsp;First</asp:LinkButton>
									</td>
									<td class="text-center">
										<asp:LinkButton ID="lnkPreviousSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="previous" data-list="ordTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1"><i class="fas fa-angle-left"></i>&nbsp;Previous</asp:LinkButton>
									</td>
									<td class="text-center">
										<div style="margin-top: 5px">
											<asp:Label Style="font-weight: bold; font-size: 18px" ID="lblCurrentPageBottomSearchP" runat="server"></asp:Label>
										</div>
									</td>
									<td class="text-center">
										<asp:LinkButton ID="lnkNextSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="next" data-list="ordTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1">Next&nbsp;<i class="fas fa-angle-right"></i></asp:LinkButton>
									</td>
									<td class="text-end">
										<asp:LinkButton ID="lnkLastSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="last" data-list="ordTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1">Last&nbsp;<i class="fas fa-angles-right"></i></asp:LinkButton>
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
					<div runat="server" id="ordinanceTabs" class="nav nav-tabs border-0" role="tablist">
						<button runat="server" id="factSheetTab" class="nav-link ordTabs active" data-toggle="tab" data-target="#factSheetPane" type="button" role="tab" tabindex="-1">Fact Sheet</button>
						<button runat="server" id="auditTab" class="nav-link ordTabs" data-toggle="tab" data-target="#auditPane" type="button" role="tab" onclick="FormatAudit();" tabindex="-1">History</button>
					</div>

					<div id="ordinanceTabsContent" class="tab-content tab-card">
						<div runat="server" id="factSheetPane" class="tab-pane fade active show" role="tabpanel">
							<%-- FORM HEADER --%>
							<section class="container form-header bg-body-secondary text-center position-relative tab-border-header">
								<div class="row h-100 align-items-center">
									<h1><span class="fas fa-book-section"></span>&nbsp;Ordinance</h1>
								</div>
								<asp:Label runat="server" ID="lblOrdID" CssClass="text-primary-emphasis ordID">ID:</asp:Label>
								<asp:LinkButton runat="server" ID="backBtn" CssClass="btn bg-danger backBtn" OnClick="backBtn_Click" OnClientClick="showLoadingModal();" TabIndex="-1"><span class="fas fa-xmark text-light"></span></asp:LinkButton>
								<asp:LinkButton runat="server" ID="copyOrd" CssClass="btn btn-primary copyBtn" OnClick="copyOrd_Click" OnClientClick="showLoadingModal();" TabIndex="-1"><span class="fas fa-copy"></span>&nbsp;Copy</asp:LinkButton>

								<div class="statusDropDown text-start">
									<div runat="server" id="ddStatusDiv" class="form-group text-start w-75 me-auto">
										<label for="ddStatus">Status</label>
										<asp:DropDownList ID="ddStatus" runat="server" AutoPostBack="true" CssClass="form-select bg-body-tertiary" data-required="true" OnSelectedIndexChanged="ddStatus_SelectedIndexChanged"></asp:DropDownList>
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
										<div class="col-md-3">
											<div class="form-group">
												<label for="ordinanceNumber">Ordinance Number</label>
												<asp:TextBox runat="server" ID="ordinanceNumber" CssClass="form-control" TextMode="SingleLine" placeholder="123-45-6789" AutoCompleteType="Disabled"></asp:TextBox>
											</div>
										</div>
										<div class="col-md-3">
											<div class="form-group">
												<label for="agendaNumber">Agenda Number</label>
												<asp:TextBox runat="server" ID="agendaNumber" CssClass="form-control" TextMode="SingleLine" placeholder="123456789" AutoCompleteType="Disabled"></asp:TextBox>
											</div>
										</div>
									</div>

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
														<button runat="server" id="uploadBtn" class="btn btn-success mt-3 ms-3 w-25" onclick="clickAspBtn('UploadDocBtn');" type="button" data-disable-btn="htmlIconBtn" data-disable-btn-icon="fa-upload" data-disable-btn-text="Uploading" disabled TabIndex="-1"><span>Upload</span></button>
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
										<div runat="server" id="fundsCheckRow" class="row mb-3">
											<%-- FUNDS CHECK BY --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="fundsCheckByGroup" class="mb-1">Funds Check By <asp:LinkButton runat="server" ID="fundsCheckEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" OnClick="signatureEmailBtn_Click" CommandArgument="fundsCheckByBtn;Funds Check By" CommandName="FundsCheckBy" TabIndex="-1"><span class="fa-kit fa-solid-signature-circle-user"></span>&nbsp;Assign Signature</asp:LinkButton></label>
													<div id="fundsCheckByGroup">
														<div runat="server" id="fundsCheckByBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="fundsCheckByBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('fundsCheckBy');" TabIndex="-1" />
															</div>
														</div>
														<div runat="server" id="fundsCheckByInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="fundsCheckBySig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" data-required="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="fundsCheckByDate" CssClass="form-control" TextMode="Date" ReadOnly="true" TabIndex="-1"></asp:TextBox>
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
										<div runat="server" id="directorSupervisorRow" class="row mb-3">
											<%-- DIRECTOR/SUPERVISOR --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="directorSupervisorGroup">Director/Supervisor <asp:LinkButton runat="server" ID="directorSupervisorEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" OnClick="signatureEmailBtn_Click" CommandArgument="directorSupervisorBtn;Director/Supervisor" CommandName="DirectorSupervisor" TabIndex="-1"><span class="fa-kit fa-solid-signature-circle-user"></span>&nbsp;Assign Signature</asp:LinkButton></label>
													<div id="directorSupervisorGroup">
														<div runat="server" id="directorSupervisorBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="directorSupervisorBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('directorSupervisor');" TabIndex="-1" />
															</div>
														</div>
														<div runat="server" id="directorSupervisorInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="directorSupervisorSig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" data-required="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="directorSupervisorDate" CssClass="form-control" TextMode="Date" ReadOnly="true" TabIndex="-1"></asp:TextBox>
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
										<div runat="server" id="cPARow" class="row mb-3">
											<%-- CITY PURCHASING AGENT --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="cPAGroup">City Purchasing Agent <asp:LinkButton runat="server" ID="cPAEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" OnClick="signatureEmailBtn_Click" CommandArgument="cPABtn;City Purchasing Agent" CommandName="CityPurchasingAgent" TabIndex="-1"><span class="fa-kit fa-solid-signature-circle-user"></span>&nbsp;Assign Signature</asp:LinkButton></label>
													<div id="cPAGroup">
														<div runat="server" id="cPABtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="cPABtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('cPA');" TabIndex="-1" />
															</div>
														</div>
														<div runat="server" id="cPAInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="cPASig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" data-required="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="cPADate" CssClass="form-control" TextMode="Date" ReadOnly="true" TabIndex="-1"></asp:TextBox>
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
										<div runat="server" id="obmDirectorRow" class="row mb-3">
											<%-- OBM DIRECTOR --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="obmDirectorGroup">OBM Director <asp:LinkButton runat="server" ID="obmDirectorEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" OnClick="signatureEmailBtn_Click" CommandArgument="obmDirectorBtn;OBM Director" CommandName="OBMDirector" TabIndex="-1"><span class="fa-kit fa-solid-signature-circle-user"></span>&nbsp;Assign Signature</asp:LinkButton></label>
													<div id="obmDirectorGroup">
														<div runat="server" id="obmDirectorBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="obmDirectorBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('obmDirector');" TabIndex="-1" />
															</div>
														</div>
														<div runat="server" id="obmDirectorInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="obmDirectorSig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" data-required="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="obmDirectorDate" CssClass="form-control" TextMode="Date" ReadOnly="true" TabIndex="-1"></asp:TextBox>
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
										<div runat="server" id="mayorRow" class="row mb-3">
											<%-- MAYOR --%>
											<div class="col-md-12">
												<div class="form-group">
													<label for="mayorGroup">Mayor <asp:LinkButton runat="server" ID="mayorEmailBtn" CssClass="text-primary fs-7 text-decoration-none ms-2" OnClick="signatureEmailBtn_Click" CommandArgument="mayorBtn;Mayor" CommandName="Mayor" TabIndex="-1"><span class="fa-kit fa-solid-signature-circle-user"></span>&nbsp;Assign Signature</asp:LinkButton></label>
													<div id="mayorGroup">
														<div runat="server" id="mayorBtnDiv" class="row readonly-color">
															<%-- SIGN BUTTON --%>
															<div class="col-md-6">
																<asp:Button runat="server" ID="mayorBtn" UseSubmitBehavior="false" CssClass="btn btn-success float-start" Width="50%" Text="Sign" data-toggle="modal" data-target="#signatureModal" OnClientClick="setSigModal('mayor');" TabIndex="-1" />
															</div>
														</div>
														<div runat="server" id="mayorInputGroup" class="row">
															<div class="col-md-4">
																<div class="input-group">
																	<span class="input-group-text fas fa-signature"></span>
																	<asp:TextBox runat="server" ID="mayorSig" CssClass="form-control" TextMode="SingleLine" AutoCompleteType="Disabled" data-required="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
																</div>
															</div>
															<div class="col-md-2">
																<div class="input-group">
																	<span class="input-group-text fas fa-calendar-days"></span>
																	<asp:TextBox runat="server" ID="mayorDate" CssClass="form-control" TextMode="Date" ReadOnly="true" TabIndex="-1"></asp:TextBox>
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
												<button id="saveBtn" class="btn btn-primary float-end w-50" onclick="validateFactSheetDraft('factSheetMain');" type="button" data-disable-btn="htmlIconBtn" data-disable-btn-icon="fa-floppy-disk" data-disable-btn-text="Saving" tabindex="-1">Save Fact Sheet</button>
												<asp:Button runat="server" ID="SaveFactSheet" UseSubmitBehavior="false" OnClick="SaveFactSheet_Click" CommandName="save" hidden="true" TabIndex="-1" />
											</div>
											<%-- DELETE BUTTON --%>
											<div class="col-md-6">
												<asp:Button runat="server" ID="DeleteFactSheet" UseSubmitBehavior="false" CssClass="btn btn-danger float-start" Width="50%" Text="Delete" data-toggle="modal" data-target="#deleteModal" TabIndex="-1" />
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
						<div runat="server" id="auditPane" class="tab-pane fade" role="tabpanel">
							<div class="card custom-card-tab">
								<div class="card-header bg-body-secondary">
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
													<tr id='auditRow<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>' class="accordion">
														<asp:HiddenField runat="server" ID="hdnAuditItem" Value='<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>' />
														<td class="align-middle text-start mw-0">
															<a href="javascript:void(0);" class="btn-accordion audit-link" data-toggle="collapse" data-target='<%# DataBinder.Eval(Container.DataItem, "UpdateType").Equals("UPDATED") || DataBinder.Eval(Container.DataItem, "UpdateType").Equals("REJECTED") ?  $"#auditItem{DataBinder.Eval(Container.DataItem, "OrdinanceAuditID")}" : string.Empty %>' tabindex="-1"><p class="m-0"><%# DataBinder.Eval(Container.DataItem, "LastUpdateDate", "{0:MM/dd/yyyy}") %> &mdash; <%# DataBinder.Eval(Container.DataItem, "LastUpdateBy") %> &mdash; <%# DataBinder.Eval(Container.DataItem, "UpdateType") %>
																	<span runat="server" class='<%# DataBinder.Eval(Container.DataItem, "UpdateType").Equals("UPDATED") || DataBinder.Eval(Container.DataItem, "UpdateType").Equals("REJECTED") ? "float-end lh-1p5 fas fa-chevron-down" : string.Empty %>'>
																	</span>
																</p></a>


															<div id='auditItem<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>' class="collapse audit-content" data-parent='#auditRow<%# DataBinder.Eval(Container.DataItem, "OrdinanceAuditID") %>'>
																<br />
																<p class="m-0"><%# DataBinder.Eval(Container.DataItem, "UpdateType").Equals("REJECTED") ? "Rejection Reason:" : "Changes:" %></p>


																<ul class="auditList" style="padding-right: 2rem;">
																	<asp:Repeater runat="server" ID="rpAuditDesc">
																		<ItemTemplate>
																			<li>
																				<%# Container.DataItem %>
																			</li>
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
													<asp:LinkButton ID="lnkAuditFirstSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="first" data-list="auditTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1" Enabled="false"><i class="fas fa-angles-left"></i>&nbsp;First</asp:LinkButton>
												</td>
												<td class="text-center">
													<asp:LinkButton ID="lnkAuditPreviousSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="previous" data-list="auditTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1"><i class="fas fa-angle-left"></i>&nbsp;Previous</asp:LinkButton>
												</td>
												<td class="text-center">
													<div style="margin-top: 5px">
														<asp:Label Style="font-weight: bold; font-size: 18px" ID="lblAuditCurrentPageBottomSearchP" runat="server"></asp:Label>
													</div>
												</td>
												<td class="text-center">
													<asp:LinkButton ID="lnkAuditNextSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="next" data-list="auditTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1">Next&nbsp;<i class="fas fa-angle-right"></i></asp:LinkButton>
												</td>
												<td class="text-end">
													<asp:LinkButton ID="lnkAuditLastSearchP" CssClass="btn btn-primary" runat="server" OnClick="paginationBtn_Click" data-command="last" data-list="auditTable" style="width: 150px;" CausesValidation="false" OnClientClick="showLoadingModal();" TabIndex="-1">Last&nbsp;<i class="fas fa-angles-right"></i></asp:LinkButton>
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
				<div class="modal-body bg-body-tertiary">
					<asp:Label runat="server" ID="deleteLabel" Style="font-size: 18px; font-weight: bold" CssClass="text-danger" Text="Are you sure you want to delete this ordinance fact sheet? (This cannot be undone)" />
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal" tabindex="-1">Cancel</button>
					<asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" CausesValidation="false" UseSubmitBehavior="false" Visible="true" OnClick="mdlDeleteSubmit_ServerClick" OnClientClick="ShowSubmitToast();" data-disable-btn="htmlBtn" data-disable-btn-text="Deleting" TabIndex="-1" />
				</div>
			</div>
		</div>
	</div>

	<%-- SIGNATURE EMAIL --%>
	<div runat="server" class="modal fade" id="signatureEmailModal" role="dialog" aria-labelledby="signatureEmailModalLabel">
		<div class="modal-dialog" role="document" style="max-width: 750px;">
			<div class="modal-content">
				<asp:UpdatePanel runat="server" ID="pnlSigEmail">
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="AddRequestEmailAddress" EventName="Click" />
					</Triggers>
					<ContentTemplate>
						<div class="modal-header">
							<h4 class="modal-title" id="signatureEmailModalLabel">Assign Signature</h4>
						</div>
						<div class="modal-body bg-body-tertiary">
							<div runat="server" id="emailListDiv" class="row mb-5">
								<div class="col-md-12">
								<asp:Repeater runat="server" ID="rpEmailList" OnItemCommand="rpEmailList_ItemCommand" OnItemCreated="rpEmailList_ItemCreated">
										<ItemTemplate>
											<div class="badge rounded-pill text-bg-secondary m-1" style="font-size: .95rem">
												<%# Container.DataItem %>
												<asp:LinkButton runat="server" ID="removeBtn" CssClass="text-danger" style="margin-left: 10px;" CommandName="remove" CommandArgument='<%# Container.DataItem %>' TabIndex="-1"><span class="fa-solid fa-xmark"></span></asp:LinkButton>
											</div>
										</ItemTemplate>
									</asp:Repeater>
								</div>
							</div>
							<div class="row mb-5">
								<div class="col-md-10">
									<div class="input-group">
										<span class="input-group-text fas fa-address-book"></span>
										<asp:TextBox runat="server" ID="signatureEmailAddress" CssClass="form-control" TextMode="Email" AutoCompleteType="Email" placeholder="john.doe@corporate.com" ></asp:TextBox>
									</div>
								</div>
								<div class="col-md-2">
									<div class="btn-group">
										<asp:Button runat="server" ID="AddRequestEmailAddress" UseSubmitBehavior="false" CssClass="btn btn-success fas-btn" OnClick="AddRequestEmailAddress_Click" disabled="disabled" Text='&#xf055;' TabIndex="-1"/>
										<asp:Button ID="btnSendSigEmail" runat="server" CssClass="btn btn-primary fas-btn" CausesValidation="false" UseSubmitBehavior="false" Visible="true" OnClick="btnSendSigEmail_Click" OnClientClick="ShowEmailToast();" Text="&#xf1d8;" TabIndex="-1" />
									</div>
								</div>
							</div>
						</div>
						<div class="modal-footer">							
							<button type="button" class="btn btn-success" data-dismiss="modal" tabindex="-1">Done</button>
							<input runat="server" id="sigBtnTarget" type="hidden" name="sigBtnTarget"  />
							<input runat="server" id="sigBtnLabel" type="hidden" name="sigBtnLabel"  />
							<input runat="server" id="sigBtnType" type="hidden" name="sigBtnType"  />
						</div>
					</ContentTemplate>
				</asp:UpdatePanel>
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
								<%--<span class="input-group-text fas fa-calendar-days"></span>--%>
								<asp:TextBox runat="server" ID="sigDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
								<button runat="server" id="sigDatePicker" type="button" class="btn input-group-text" tabindex="-1"><span class="fas fa-calendar-days"></span></button>
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
									<asp:CheckBox runat="server" ID="certifySig" CssClass="form-check-input" style="transform: scale(1.5); margin-left: -1em;" TabIndex="-1"/>
								</div>
							</div>
						</div>
					</div>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal" tabindex="-1">Cancel</button>
					<asp:Button ID="btnSignDoc" runat="server" Text="Sign Document" CssClass="btn btn-success" CausesValidation="false" UseSubmitBehavior="false" Visible="true" data-dismiss="modal" OnClick="btnSignDoc_Click" disabled="disabled" TabIndex="-1" />
					<input runat="server" id="sigType" type="hidden" name="sigType"/>
				</div>
			</div>
		</div>
	</div>

	<!-- REJECTION MODAL -->
	<div class="modal fade" id="rejectionModal" tabindex="-1" role="dialog" aria-labelledby="rejectionModalLabel">
		<div class="modal-dialog modal-lg" role="document">
			<div class="modal-content">
				<div class="modal-header">
					<h4 class="modal-title" id="rejectionModalLabel">Rejection</h4>
				</div>
				<div class="modal-body bg-body-tertiary">
					<div class="row mb-2">
						<div class="col-md-12">
							<div class="form-group">
								<label for="rejectionReason">Rejection Details/Changes Needed:</label>
								<asp:TextBox runat="server" ID="rejectionReason" CssClass="form-control" TextMode="MultiLine" Rows="8" AutoCompleteType="Disabled" data-enter-btn="btnSendRejection"></asp:TextBox>
							</div>
						</div>
					</div>
				</div>
				<div class="modal-footer">
					<asp:Button ID="btnCancelRejection" runat="server" Text="Cancel" CssClass="btn btn-secondary" CausesValidation="false" UseSubmitBehavior="false" Visible="true" data-dismiss="modal" OnClick="cancelRejection_Click" TabIndex="-1" />
					<asp:Button ID="btnSendRejection" runat="server" Text="Send Rejection" CssClass="btn btn-primary" CausesValidation="false" UseSubmitBehavior="false" Visible="true" OnClick="sendRejection_Click" OnClientClick="ShowSubmitToast();" TabIndex="-1" />
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
			multiValidation();
			DisableDDInitialOption([
				{ id: '<%= ddStatus.ClientID %>', opacity: "75" },
				{ id: '<%= requestDepartment.ClientID %>', opacity: "75" },
				{ id: '<%= requestDivision.ClientID %>', opacity: "35" },
				{ id: '<%= purchaseMethod.ClientID %>', opacity: "75" },
			]);
			FilterFirstItem();
			scrollToElement();
			SetSignEnabled();			
			FormatAudit();
			SetModalDatePicker("signatureModal");
		}
		

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			SetTooltips();
			cancelFilePick('<%= supportingDocumentation.ClientID %>');
			addSignatureEmails([{ addressID: '<%= signatureEmailAddress.ClientID %>', btnID: '<%= AddRequestEmailAddress.ClientID %>' }]);
			DisableDDInitialOption([
				{ id: '<%= ddStatus.ClientID %>', opacity: "75" },
				{ id: '<%= requestDepartment.ClientID %>', opacity: "75" },
				{ id: '<%= requestDivision.ClientID %>', opacity: "35" },
				{ id: '<%= purchaseMethod.ClientID %>', opacity: "75" },
			]);
			FilterFirstItem();
			scrollToElement();
			SetSignEnabled();
			FormatAudit();			
			scrollToElement();
			HideAllTooltips();			
		});

		

		function validateFactSheetDraft(validationGroups) {
			if (Page_ClientValidate(validationGroups)) {
				isValid = true;
				var btn = document.getElementById('<%= SaveFactSheet.ClientID %>');
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

		function setEmailModal(btnID, btnLabel, sigType) {
			const sigBtnTarget = $('#<%= sigBtnTarget.ClientID %>');
			const sigBtnLabel = $('#<%= sigBtnLabel.ClientID %>');
			const sigBtnType = $('#<%= sigBtnType.ClientID %>');
			$(sigBtnTarget).attr("value", btnID);
			$(sigBtnLabel).attr("value", btnLabel);
			$(sigBtnType).attr("value", sigType);
			$('#<%= signatureEmailAddress.ClientID %>').val('');
			$('#signatureEmailModal').modal('show');
		}

		function scrollToElement() {
			const urlParams = new URLSearchParams(window.location.search);
			if (urlParams.get('f')) {
				setTimeout(() => document.getElementById(urlParams.get('f')).scrollIntoView(), 500);
				document.getElementById(urlParams.get('f')).focus();
				//setTimeout(() => , 500);
			}
		}

		function setSigModal(typeOfSig) {
			const sigType = $('#<%= sigType.ClientID %>');
			$(sigType).attr("value", typeOfSig);
			$('#<%= sigName.ClientID %>').val('');
			$('#<%= sigDate.ClientID %>').val('');
			$('#<%= btnSignDoc.ClientID %>').prop('disabled', true);
			$('#<%= certifySig.ClientID %>').prop('checked', false);

			$('#signatureModal').on('shown.bs.modal', function () {
				
			});
		}

		function OpenRejectionModal() {
			$('#rejectionModal').modal('show');
		}

		function SetSignEnabled() {
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
		}
	</script>
</asp:Content>