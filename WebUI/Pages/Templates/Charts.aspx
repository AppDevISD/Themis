<%@ Page Title="Charts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Charts.aspx.cs" Inherits="WebUI.Charts" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<section>
		<div class="card">
			<div class="card-header bg-body">
				<h3><i class="fas fa-chart-pie"></i>&nbsp;Charts</h3>
			</div>
			<div class="card-body bg-body-tertiary">
				<div class="container-fluid">
					<%-- LINE CHARTS --%>
					<label for="lineCharts" class="section-label">Line Charts</label>
					<div id="lineCharts" class="row mb-5">
						<%-- TABS --%>
						<div id="lineChartDiv" class="col-md-9 mb-3">
							<%-- TAB BUTTONS --%>
							<div id="lineChartTabs" class="nav nav-tabs" role="tablist">
								<button id="lineChartTab" class="nav-link active" data-toggle="tab" data-target="#lineChart-tab-pane" type="button" role="tab">Chart</button>
								<button id="lineChartHTMLJSTab" class="nav-link" data-toggle="tab" data-target="#lineChartHTMLJS-tab-pane" type="button" role="tab">HTML/JS</button>
								<button id="lineChartDataOptionsTab" class="nav-link" data-toggle="tab" data-target="#lineChartDataOptions-tab-pane" data-source-active="lineChart" type="button" role="tab">Data/Options</button>
								<button id="lineChartDataSourceTab" class="nav-link" data-toggle="tab" data-target="#lineChartDataSource-tab-pane" data-source-hide="lineChart" type="button" role="tab">Data Source</button>
							</div>

							<%-- TABS CONTENT --%>
							<div id="lineChartTabsContent" class="tab-content jsChartTabs">
								<%-- CHART TAB --%>
								<div id="lineChart-tab-pane" class="tab-pane fade show active" role="tabpanel">
									<canvas runat="server" id="lineChart" class="jsChart" ClientIDMode="static"></canvas>
								</div>

								<%-- HTML/JS TAB --%>
								<div id="lineChartHTMLJS-tab-pane" class="tab-pane fade codePane" role="tabpanel">
									<%-- HTML --%>
									<div class="row">
										<label for="lineChartHTMLCode">HTML</label>
										<div runat="server" id="lineChartHTMLCode" class="mb-3" data-chart-key="lineChart" data-language-key="html"></div>
									</div>

									<%-- JAVASCRIPT/HELPERS --%>
									<div class="row codeRowGrow">
										<%-- JAVASCRIPT --%>
										<div class="codeCol col-md-4">
											<label for="lineChartJSCode">JavaScript</label>
											<div runat="server" id="lineChartJSCode" class="codeGrow" data-chart-key="lineChart" data-language-key="javascript"></div>
										</div>

										<%-- HELPERS --%>
										<div class="codeCol col-md-8">
											<label for="lineChartHelpersCode">Chart Helpers</label>
											<div runat="server" id="lineChartHelpersCode" class="codeGrow" data-chart-key="helpers" data-language-key="javascript"></div>
										</div>
									</div>
								</div>

								<%-- DATA/OPTIONS TAB --%>
								<div id="lineChartDataOptions-tab-pane" class="tab-pane fade codePane" role="tabpanel">
									<div class="row codeRowGrow">
										<%-- DATA --%>
										<div class="codeCol col-md-6">
											<label for="lineChartDataCodeDiv">Data</label>
											<div id="lineChartDataCodeDiv" class="codeGrow">
												<pre class="codeClip"><code class="language-javascript" id="lineChartDataCode" data-chart-code="lineChart" data-code-type="data" data-prismjs-copy="Copy"></code></pre>
											</div>
										</div>

										<%-- OPTIONS --%>
										<div class="codeCol col-md-6">
											<label for="lineChartOptionsCodeDiv">Options</label>
											<div id="lineChartOptionsCodeDiv" class="codeGrow">
												<pre class="codeClip"><code class="language-javascript" id="lineChartOptionsCode" data-chart-code="lineChart" data-code-type="options" data-prismjs-copy="Copy"></code></pre>
											</div>
										</div>
									</div>
								</div>

								<%-- DATA SOURCE TAB --%>
								<div id="lineChartDataSource-tab-pane" class="tab-pane fade codePane" role="tabpanel">
									<div class="row codeRowGrow">
										<%-- AJAX JAVASCRIPT --%>
										<div class="codeCol col-md-6">
											<label for="lineChartDataSourceCodeDiv">JavaScript (Ajax for Random Coords)</label>
											<div id="lineChartDataSourceCodeDiv" class="codeGrow">
												<pre class="codeNoClip"><code class="language-javascript" id="lineChartDataSourceCode" data-chart-code="lineChart" data-code-type="dataSource" data-source-type="c#" data-prismjs-copy="Copy"></code></pre>
											</div>
										</div>

										<%-- CHART HANDLER --%>
										<div class="codeCol col-md-6">
											<label for="lineChartHandlerCode">ChartHandler.asmx</label>
											<div runat="server" id="lineChartHandlerCode" class="codeGrow"></div>
										</div>
									</div>
								</div>
							</div>
						</div>

						<%-- CONTROLS --%>
						<div id="lcControls" class="col-md-3 d-flex align-items-center">
							<div class="w-100 px-5">
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="lineTitleSwitch">Graph Title</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="title" type="checkbox" role="switch" id="lineTitleSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="lineXTitleSwitch">X-Axis Title</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="xTitle" type="checkbox" role="switch" id="lineXTitleSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="lineYTitleSwitch">Y-Axis Title</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="yTitle" type="checkbox" role="switch" id="lineYTitleSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="colorLinesSwitch">Color Lines</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="colorLines" type="checkbox" role="switch" id="lineColorLinesSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="linePointBorderSwitch">Point Border</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="pointBorder" type="checkbox" role="switch" id="linePointBorderSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="lineFillSwitch">Fill</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="fill" type="checkbox" role="switch" id="lineFillSwitch">
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="orientationSwitch">Horizontal</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="orientation" type="checkbox" role="switch" id="lineOrientationSwitch">
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="dataSourceSwitch">JS</label>
									<input class="form-check-input chart-control" data-chart="lineChart" data-control="dataSource" type="checkbox" role="switch" id="lineDataSourceSwitch">
								</div>
								<div class="w-100 text-center mt-5">
									<button class="btn btn-primary w-100 chart-control" type="button" data-chart="lineChart" data-control="randomize">Randomize</button>
								</div>
							</div>
						</div>
					</div>

					<%-- Bar CHARTS --%>
					<label for="barCharts" class="section-label">Bar Charts</label>
					<div id="barCharts" class="row mb-3">
						<%-- TABS --%>
						<div id="barChartDiv" class="col-md-9 mb-3">
							<%-- TAB BUTTONS --%>
							<div id="barChartTabs" class="nav nav-tabs" role="tablist">
								<button id="barChartTab" class="nav-link active" data-toggle="tab" data-target="#barChart-tab-pane" type="button" role="tab">Chart</button>
								<button id="barChartHTMLJSTab" class="nav-link" data-toggle="tab" data-target="#barChartHTMLJS-tab-pane" type="button" role="tab">HTML/JS</button>
								<button id="barChartDataOptionsTab" class="nav-link" data-toggle="tab" data-target="#barChartDataOptions-tab-pane" data-source-active="barChart" type="button" role="tab">Data/Options</button>
								<button id="barChartDataSourceTab" class="nav-link" data-toggle="tab" data-target="#barChartDataSource-tab-pane" data-source-hide="barChart" type="button" role="tab">Data Source</button>
							</div>

							<%-- TABS CONTENT --%>
							<div id="barChartTabsContent" class="tab-content jsChartTabs">
								<%-- CHART TAB --%>
								<div id="barChart-tab-pane" class="tab-pane fade show active" role="tabpanel">
									<canvas runat="server" id="barChart" class="jsChart" clientidmode="static"></canvas>
								</div>

								<%-- HTML/JS TAB --%>
								<div id="barChartHTMLJS-tab-pane" class="tab-pane fade codePane" role="tabpanel">
									<%-- HTML --%>
									<div class="row">
										<label for="barChartHTMLCode">HTML</label>
										<div runat="server" id="barChartHTMLCode" class="mb-3" data-chart-key="barChart" data-language-key="html"></div>
									</div>

									<%-- JAVASCRIPT/HELPERS --%>
									<div class="row codeRowGrow">
										<%-- JAVASCRIPT --%>
										<div class="codeCol col-md-4">
											<label for="barChartJSCode">JavaScript</label>
											<div runat="server" id="barChartJSCode" class="codeGrow" data-chart-key="barChart" data-language-key="javascript"></div>
										</div>

										<%-- HELPERS --%>
										<div class="codeCol col-md-8">
											<label for="barChartHelpersCode">Chart Helpers</label>
											<div runat="server" id="barChartHelpersCode" class="codeGrow" data-chart-key="helpers" data-language-key="javascript"></div>
										</div>
									</div>
								</div>

								<%-- DATA/OPTIONS TAB --%>
								<div id="barChartDataOptions-tab-pane" class="tab-pane fade codePane" role="tabpanel">
									<div class="row codeRowGrow">
										<%-- DATA --%>
										<div class="codeCol col-md-6">
											<label for="barChartDataCodeDiv">Data</label>
											<div id="barChartDataCodeDiv" class="codeGrow">
												<pre class="codeNoClip"><code class="language-javascript" id="barChartDataCode" data-chart-code="barChart" data-code-type="data" data-prismjs-copy="Copy"></code></pre>
											</div>
										</div>

										<%-- OPTIONS --%>
										<div class="codeCol col-md-6">
											<label for="barChartOptionsCodeDiv">Options</label>
											<div id="barChartOptionsCodeDiv" class="codeGrow">
												<pre class="codeClip"><code class="language-javascript" id="barChartOptionsCode" data-chart-code="barChart" data-code-type="options" data-prismjs-copy="Copy"></code></pre>
											</div>
										</div>
									</div>
								</div>

								<%-- DATA SOURCE TAB --%>
								<div id="barChartDataSource-tab-pane" class="tab-pane fade codePane" role="tabpanel">
									<div class="row codeRowGrow">
										<%-- AJAX JAVASCRIPT --%>
										<div class="codeCol col-md-6">
											<label for="barChartDataSourceCodeDiv">JavaScript (Ajax for Random Coords)</label>
											<div id="barChartDataSourceCodeDiv" class="codeGrow">
												<pre class="codeNoClip"><code class="language-javascript" id="barChartDataSourceCode" data-chart-code="barChart" data-code-type="dataSource" data-source-type="c#" data-prismjs-copy="Copy"></code></pre>
											</div>
										</div>

										<%-- CHART HANDLER --%>
										<div class="codeCol col-md-6">
											<label for="barChartHandlerCode">ChartHandler.asmx</label>
											<div runat="server" id="barChartHandlerCode" class="codeGrow"></div>
										</div>
									</div>
								</div>
							</div>
						</div>

						<%-- CONTROLS --%>
						<div id="bcControls" class="col-md-3 d-flex align-items-center">
							<div class="w-100 px-5">
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="barTitleSwitch">Graph Title</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="title" type="checkbox" role="switch" id="barTitleSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="barXTitleSwitch">X-Axis Title</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="xTitle" type="checkbox" role="switch" id="barXTitleSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="barYTitleSwitch">Y-Axis Title</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="yTitle" type="checkbox" role="switch" id="barYTitleSwitch" checked>
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="barStackedSwitch">Stacked</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="stacked" type="checkbox" role="switch" id="barStackedSwitch">
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="barStackedGroupSwitch">Stacked Group</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="stackedGroup" type="checkbox" role="switch" id="barStackedGroupSwitch">
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="barFillSwitch">Fill</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="fill" type="checkbox" role="switch" id="barFillSwitch">
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="orientationSwitch">Horizontal</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="orientation" type="checkbox" role="switch" id="barOrientationSwitch">
								</div>
								<div class="form-check form-switch chart-controls fs-4 mb-3 p-0">
									<label class="form-check-label" for="dataSourceSwitch">JS</label>
									<input class="form-check-input chart-control" data-chart="barChart" data-control="dataSource" type="checkbox" role="switch" id="barDataSourceSwitch">
								</div>
								<div class="w-100 text-center mt-5">
									<button class="btn btn-primary w-100 chart-control" type="button" data-chart="barChart" data-control="randomize">Randomize</button>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</section>

	<%-- CHARTS FILE (USED FOR PRODUCTION) --%>
	<script type="text/javascript" src="./assets/js/Charts.js"></script>

	<%-- IMPORT JSON FOR DEVELOPMENT SETTINGS CODE BLOCKS (USED FOR DEVELOPMENT) --%>
	<script type="module" src="./Scripts/ChartCode/ChartOptions.js"></script>

	<script type="text/javascript" src="./Scripts/Helpers/JS/ChartPrism.js"></script>
</asp:Content>
