<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebUI._Default" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<section class="border-bottom">
		<div class="col-md-12 text-center d-flex align-items-end pb-3">
			<div style="position: relative;">
				<div class="themis-logo m-0"></div>
			</div>
			<h1 class="welcome-text m-0 ms-5">Welcome to <span class="gfs-neohellenic-bold">THΣMIS</span>!</h1>
		</div>
	</section>

	<section class="pt-3">
		<div class="row">
			<div class="col-md-6">
				<a href="./Ordinances" class="btn btn-secondary btn-home-link w-100">
					<span class="fas fa-book-section home-link-icon"></span>
					<div class="home-link-text-div">
						<strong class="home-link-text">View Ordinances</strong>
					</div>
				</a>
			</div>
			<div class="col-md-6">
				<a href="./NewFactSheet" class="btn btn-secondary btn-home-link w-100">
					<span class="fas fa-file-circle-plus home-link-icon"></span>
					<div class="home-link-text-div">
						<strong class="home-link-text">New Fact Sheet</strong>
					</div>
				</a>
			</div>
		</div>
	</section>
</asp:Content>
