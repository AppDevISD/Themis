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

			<asp:UpdatePanel runat="server" ID="formUpdatePanel">
				<ContentTemplate>
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
						<div class="row mb-5">
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
									<div class="radioListDiv" id="scopeChangeList">
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
									<asp:TextBox runat="server" ID="codeProvision" CssClass="form-control" TextMode="SingleLine" placeholder="123456789" AutoCompleteType="Disabled" required="true"></asp:TextBox>
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
										<div class="form-check form-check-inline">
											<label for="paApprovalRequiredYes">Yes</label>
											<asp:RadioButton runat="server" ID="paApprovalRequiredYes" CssClass="form-check-input" GroupName="paApprovalRequiredList" />
										</div>
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
										<div class="form-check form-check-inline">
											<label for="paApprovalAttachedYes">Yes</label>
											<asp:RadioButton runat="server" ID="paApprovalAttachedYes" CssClass="form-check-input" GroupName="paApprovalAttachedList" />
										</div>
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
							<div class="col-md-6 tableAddTable">
								<label for="rpRevenueTable">Revenue</label>
								<table id="revenueTable" class="table table-bordered table-striped table-hover text-center" style="padding: 0px; margin: 0px">
									<thead>
										<tr>
											<th style="width: 15%; text-align: center"><strong>Fund</strong></th>
											<th style="width: 17%; text-align: center"><strong>Agency</strong></th>
											<th style="width: 17%; text-align: center"><strong>Org</strong></th>
											<th style="width: 17%; text-align: center"><strong>Activity</strong></th>
											<th style="width: 17%; text-align: center"><strong>Object</strong></th>
											<th style="width: 17%; text-align: center"><strong>Amount</strong></th>
										</tr>
									</thead>
									<asp:Repeater runat="server" ID="rpRevenueTable">
										<ItemTemplate>
											<tr>
												<td style="vertical-align: middle">
													<%--<asp:HiddenField runat="server" ID="hdnRevRowID" Value='<%# DataBinder.Eval(Container.DataItem, "RowNum") %>' />--%>
													<asp:DropDownList ID="revenueFundCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# fundCodes %>' DataValueField='<%# DataBinder.Eval(Container.DataItem, "FundCode") %>'></asp:DropDownList>
												</td>
												<td style="vertical-align: middle">
													<asp:DropDownList ID="revenueAgencyCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# agencyCodes %>' DataValueField='<%# DataBinder.Eval(Container.DataItem, "DepartmentCode") %>'></asp:DropDownList>
												</td>
												<td style="vertical-align: middle">
													<asp:DropDownList ID="revenueOrgCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# orgCodes %>' DataValueField='<%# DataBinder.Eval(Container.DataItem, "UnitCode") %>'></asp:DropDownList>
												</td>
												<td style="vertical-align: middle">
													<asp:DropDownList ID="revenueActivityCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# activityCodes %>' DataValueField='<%# DataBinder.Eval(Container.DataItem, "ActivityCode") %>'></asp:DropDownList>
												</td>
												<td style="vertical-align: middle">
													<asp:DropDownList ID="revenueObjectCode" runat="server" CssClass="form-select" required="true" ValidateRequestMode="Enabled" DataSource='<%# objectCodes %>' DataValueField='<%# DataBinder.Eval(Container.DataItem, "ObjectCode") %>'></asp:DropDownList>
												</td>
												<td style="vertical-align: middle">
													<asp:TextBox runat="server" ID="revenueAmount" CssClass="form-control" TextMode="SingleLine" data-type="currency" placeholder="$10,000.00" AutoCompleteType="Disabled"></asp:TextBox>
												</td>
											</tr>
										</ItemTemplate>
									</asp:Repeater>
								</table>
								<asp:Button runat="server" ID="newRevenueRow" CssClass="btn tableAdd" OnClick="newAccountingRow_ServerClick" CausesValidation="false" UseSubmitBehavior="false" />
								<%--<button runat="server" type="button" id="newRevenueRow" class="btn tableAdd" onserverclick="newAccountingRow_ServerClick" data-row-type="revenue" causesvalidation="false"><i class="fas fa-circle-plus text-success"></i></button>--%>
							</div>
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
							<div class="col-md-6">
								<div class="form-group">
									<label for="supportingDocumentation">Supporting Documentation (Ex: Contract, Agreement, Change Order, Bid Book)</label>
									<asp:FileUpload runat="server" ID="supportingDocumentation" CssClass="form-control" AllowMultiple="true" />
								</div>
							</div>
						</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>

			

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
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			
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

			$(document).ready(function () {
				$("[data-type='telephone']").mask('(000) 000-0000');
				$("[data-type='extension']").mask('x000000');
				$("[data-type='currency']").each(function () {
					$(this).on("change keyup paste", function () {
						formatCurrency($(this));
					});

					$(this).on("focusout", function () {
						formatCurrency($(this), "blur");
					});
				});

				function formatNumber(n) {
					// format number 1000000 to 1,234,567
					return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
				}
				function formatCurrency(input, blur) {
					// appends $ to value, validates decimal side
					// and puts cursor back in right position.

					// get input value
					var input_val = input.val();

					// don't validate empty input
					if (input_val === "") { return; }

					// original length
					var original_len = input_val.length;

					// initial caret position 
					/*    var caret_pos = input.prop("selectionStart");*/

					// check for decimal
					if (input_val.indexOf(".") >= 0) {

						// get position of first decimal
						// this prevents multiple decimals from
						// being entered
						var decimal_pos = input_val.indexOf(".");

						// split number by decimal point
						var left_side = input_val.substring(0, decimal_pos);
						var right_side = input_val.substring(decimal_pos);

						// add commas to left side of number
						left_side = formatNumber(left_side);

						// validate right side
						right_side = formatNumber(right_side);

						// On blur make sure 2 numbers after decimal
						if (blur === "blur") {
							right_side += "00";
						}

						// Limit decimal to only 2 digits
						right_side = right_side.substring(0, 2);

						// join number by .
						input_val = "$" + left_side + "." + right_side;

					} else {
						// no decimal entered
						// add commas to number
						// remove all non-digits
						input_val = formatNumber(input_val);
						input_val = "$" + input_val;

						// final formatting
						if (blur === "blur") {
							input_val += ".00";
						}
					}

					// send updated string to input
					input.val(input_val);
				}

				var datePeriodStartEntered;
				var datePeriodEndEntered;
				let dateTerm = $("input[data-type='dateTerm']");
				let datePeriodStart = $("input[data-type='datePeriodStart']");
				let datePeriodEnd = $("input[data-type='datePeriodEnd']");

				LoadTermVars();
				function LoadTermVars() {
					if (datePeriodStart.val() != "") {
						datePeriodStartEntered = true;
					}
					else {
						datePeriodStartEntered = false;
					}
					if (datePeriodEnd.val() != "") {
						datePeriodEndEntered = true;
					}
					else {
						datePeriodEndEntered = false;
					}
					if (datePeriodStart.val() != "" && datePeriodEnd.val() != "") {
						GetTermDate();
					}
				}
				datePeriodStart.each(function () {
					$(this).on("change keyup paste", function () {
						let output = "";
						if (datePeriodStartEntered && datePeriodEndEntered && $(this).val() != "") {
							GetTermDate();
							return output;
						}
						else if (datePeriodEndEntered && $(this).val() != "") {
							datePeriodStartEntered = true;
							GetTermDate();
							return output;
						}
						else if ($(this).val() == "") {
							datePeriodStartEntered = false;
							dateTerm.val(output);
							return output;
						}
						else {
							datePeriodStartEntered = true;
							dateTerm.val(output);
							return output;
						}
					});
				});
				datePeriodEnd.each(function () {
					$(this).on("change keyup paste", function () {
						let output = "";
						if (datePeriodStartEntered && datePeriodEndEntered && $(this).val() != "") {
							GetTermDate();
							return output;
						}
						else if (datePeriodStartEntered && $(this).val() != "") {
							datePeriodEndEntered = true;
							GetTermDate();
							return output;
						}
						else if ($(this).val() == "") {
							datePeriodEndEntered = false;
							dateTerm.val(output);
							return output;
						}
						else {
							datePeriodEndEntered = true;
							dateTerm.val(output);
							return output;
						}
					});
				});
				function GetTermDate() {
					let startDate = new Date(datePeriodStart.val());
					let endDate = new Date(datePeriodEnd.val());
					let TimeDifference = endDate.getTime() - startDate.getTime();
					let DaysDifference = Math.round(TimeDifference / (1000 * 3600 * 24));
					if (DaysDifference > 1 || DaysDifference == 0) {
						dateTerm.val(`${DaysDifference} Days`);
					}
					else {
						dateTerm.val(`${DaysDifference} Day`);
					}
				}
			});
		});

		
	</script>
</asp:Content>
