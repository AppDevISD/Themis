﻿<%@ Page Title="New Fact Sheet" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewFactSheet.aspx.cs" Inherits="WebUI.NewFactSheet" ClientIDMode="Static" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
	<%-- FORM HEADER --%>
	<section class="container form-header bg-body text-center">
		<div class="row h-100 align-items-center">
			<h1><span class="fas fa-file-circle-plus"></span>&nbsp;New Fact Sheet</h1>
		</div>
	</section>

	<%-- FORM BODY --%>
	<div class="container form-page bg-body-tertiary">
		<div class="px-2 py-4">

			<%-- FORM DESCRIPTION / INSTRUCTIONS --%>
			<p class="text-justify"></p>

			<%-- REQUIRED FIELD DESCRIPTOR --%>
			<p class="text-justify" style="color: gray;"><i class="fa-solid fa-asterisk"></i>&nbsp;= Required Field</p>

			<%-- FORM UPDATE PANEL --%>
			<asp:UpdatePanel runat="server" ID="formUpdatePanel" UpdateMode="Always" data-inactivity-refresh="true">
				<%-- TRIGGERS --%>
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="epYes" EventName="CheckedChanged" />
					<asp:AsyncPostBackTrigger ControlID="epNo" EventName="CheckedChanged" />
					<asp:AsyncPostBackTrigger ControlID="scYes" EventName="CheckedChanged" />
					<asp:AsyncPostBackTrigger ControlID="scNo" EventName="CheckedChanged" />
					<asp:AsyncPostBackTrigger ControlID="purchaseMethod" EventName="SelectedIndexChanged" />

					<asp:PostBackTrigger ControlID="UploadImageBtn" />
					<asp:PostBackTrigger ControlID="SubmitFactSheet" />

					<asp:AsyncPostBackTrigger ControlID="lnkInactivityRefresh" EventName="Click" />
				</Triggers>

				<%-- FORM CONTENT --%>
				<ContentTemplate>
					<%-- USER VISIBLE FIELDS --%>
					<div id="userFields" runat="server">
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
										<div class="radioListDiv" id="epList">
											<%-- YES --%>
											<div class="form-check form-check-inline">
												<label for="epYes">Yes</label>
												<asp:RadioButton runat="server" ID="epYes" CssClass="form-check-input" GroupName="epList" OnCheckedChanged="EPCheckedChanged" AutoPostBack="true" />
											</div>

											<%-- NO --%>
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
					</div>

					<%-- ADMIN VISIBLE FIELDS --%>
					<div id="adminFields" runat="server">
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
									<div id="otherExceptionDiv" class="form-group">
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
										<asp:TextBox runat="server" ID="codeProvision" CssClass="form-control" TextMode="SingleLine" placeholder="123456789" AutoCompleteType="Disabled"></asp:TextBox>
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
												<asp:RadioButton runat="server" ID="paApprovalRequiredNo" CssClass="form-check-input" GroupName="paApprovalRequiredList" Checked="true" />
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
												<asp:RadioButton runat="server" ID="paApprovalAttachedNo" CssClass="form-check-input" GroupName="paApprovalAttachedList" Checked="true" />
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
											<asp:Repeater runat="server" ID="rpRevenueTable" OnItemCommand="rpAccountingTable_ItemCommand" >
												<ItemTemplate>
													<tr>
														<td style="vertical-align: middle">
															<asp:HiddenField runat="server" ID="hdnRevID" Value='<%# Container.ItemIndex %>' />
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
														
															<div>
																<asp:Button runat="server" ID="removeRevenueRow" CssClass="btn row-delete" UseSubmitBehavior="false" CommandName="delete" CommandArgument="revenue" Text="&#xf068;" />
															</div>
														</td>
													</tr>
												</ItemTemplate>
											</asp:Repeater>
										</tbody>
									</table>

									<%-- ADD REVENUE ROW BUTTON --%>
									<div class="text-center w-100">
										<asp:Button runat="server" ID="newRevenueRow" CssClass="btn btn-success w-100 row-add" OnClick="newAccountingRow_ServerClick" UseSubmitBehavior="false" CommandName="revenue" Text="Add Row" />
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
													<tr>
														<td style="vertical-align: middle">
															<asp:HiddenField runat="server" ID="hdnExpID" Value='<%# Container.ItemIndex %>' />
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

															<div>
																<asp:Button runat="server" ID="removeExpenditureRow" CssClass="btn row-delete" UseSubmitBehavior="false" CommandName="delete" CommandArgument="expenditure" Text="&#xf068;" />
															</div>
														</td>
													</tr>
												</ItemTemplate>
											</asp:Repeater>
										</tbody>
									</table>

									<%-- ADD EXPENDITURE ROW BUTTON --%>
									<div class="text-center w-100">
										<asp:Button runat="server" ID="newExpenditureRow" CssClass="btn btn-success w-100 row-add" OnClick="newAccountingRow_ServerClick" UseSubmitBehavior="false" CommandName="expenditure" Text="Add Row" />
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
							<div class="row mb-3">
								<%-- SUPPORTING DOCUMENTATION --%>
								<div class="col-md-8">
									<div class="form-group">
										<label for="supportingDocumentationGroup">Supporting Documentation (Ex: Contract, Agreement, Change Order, Bid Book)</label>
										<ul class="list-group mt-1">
											<asp:Repeater runat="server" ID="rpSupportingDocumentation" OnItemCommand="rpSupportingDocumentation_ItemCommand">
												<ItemTemplate>
													<li class="list-group-item" style="line-height: 2.25;">
														<asp:HiddenField runat="server" ID="hdnDocIndex" Value='<%# Container.ItemIndex %>' />
														<%# DataBinder.Eval(Container.DataItem, "DocumentName") %>
														<div class="d-flex float-end">
															<asp:LinkButton runat="server" ID="deleteFile" CssClass="btn btn-danger" CommandName="delete" Style="margin-left: 5px;"><span class="fas fa-trash-can"></span></asp:LinkButton>
														</div>
													</li>
												</ItemTemplate>
											</asp:Repeater>
										</ul>
										<div id="supportingDocumentationGroup" class="d-flex">
											<asp:FileUpload runat="server" ID="supportingDocumentation" CssClass="form-control mt-3" AllowMultiple="true" onchange="SetUploadActive();" />
											<asp:Button runat="server" ID="UploadImageBtn" UseSubmitBehavior="false" CssClass="btn btn-success mt-3 ms-3" Width="25%" Text="Upload" OnClick="UploadImageBtn_Click" disabled="disabled" />
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>

					<%-- SUBMIT SECTION --%>
					<div class="form-section">
						<%-- FIRST ROW --%>
						<div class="row mt-3 mb-3 text-center">
							<%-- SUBMIT BUTTON --%>
							<div class="col-md-12">
								<asp:Button runat="server" ID="SubmitFactSheet" UseSubmitBehavior="true" CssClass="btn btn-primary" Width="25%" Text="Submit" OnClick="SubmitForm_Click" OnClientClick="ShowSubmitToast();" />
							</div>
						</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
			
		</div>
	</div>

	<%-- TOAST MESSAGE --%>
	<div class="toast-container position-fixed bottom-0 end-0 p-3">
		<div id="submitToast" class='toast <%:toastColor%> border-0 fade-slide-in' role="alert" aria-live="assertive" aria-atomic="true" data-delay="10000" data-animation="true">
			<div class="d-flex">
				<div class="toast-body"><%:toastMessage%></div>
				<button type="button" class="btn-close btn-close-white me-2 m-auto" data-dismiss="toast" aria-label="Close"></button>
			</div>
		</div>
	</div>

	<%-- JAVASCRIPT --%>
	<script type="text/javascript" src="./assets/js/FileUploadSaving.js"></script>
	<script>
		DisableDDInitialOption();

		var prm = Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			GetToastStatus();
			FormatForms();
			DisableDDInitialOption();
			$("[data-type='currency']").each(function () {
				formatCurrency($(this), "blur");
			});
		});

		function SetUploadActive() {
			const supportingDocumentation = document.getElementById('<%= supportingDocumentation.ClientID %>')
			var UploadImageBtn = document.getElementById('<%= UploadImageBtn.ClientID %>')
			if (supportingDocumentation.files.length > 0) {
				UploadImageBtn.disabled = false;
			}
			else {
				UploadImageBtn.disabled = true;
			}

		}

		function DisableDDInitialOption() {
			var ddDepartment = document.getElementById('<%= requestDepartment.ClientID %>');
			var ddMethod = document.getElementById('<%= purchaseMethod.ClientID %>');
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
	</script>
</asp:Content>