<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin-manage-patient.aspx.cs" Inherits="Assignment.admin_manage_patient" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Patient Management</title>
    <!-- CSS Links -->
    <link rel="stylesheet" href="~/css/global.css" />
    <link rel="stylesheet" href="~/css/admin/admin-manage-patient.css" />
    <!-- Font Awesome Icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <!-- Animate.css -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css" />

                <script type="text/javascript">
  function googleTranslateElementInit() {
    new google.translate.TranslateElement(
      {pageLanguage: 'en'},
      'google_translate_element'
    );
  }
                </script>

<script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Sidebar -->
        <div class="sidebar">
            <h2>Admin Panel</h2>
            <a href="admin-dashboard.aspx"><i class="fas fa-tachometer-alt"></i> Dashboard</a>
            <a href="admin-manage-doctor.aspx"><i class="fas fa-user-md"></i> Manage Doctors</a>
            <a href="admin-manage-patient.aspx"class="active"><i class="fas fa-hospital-user"></i> Manage Patients</a>
            <a href="admin-manage-appointments.aspx"><i class="fas fa-calendar-check"></i> Manage Appointments</a>
            <a href="admin-reports.aspx"><i class="fas fa-chart-bar"></i> Reports</a>
            <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
            <!-- Translate Widget at the Bottom -->
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>
        </div>



        <!-- Main Content -->
        <div class="dashboard-container animate_animated animate_fadeIn">
            <!-- Header -->
            <div class="header">
                <h2><i class="fas fa-hospital-user"></i> Patient Management</h2>
                <asp:Button ID="btnAddPatient" runat="server" CssClass="add-new-btn" Text="Add New Patient" Height="41px" Width="200px" 
                    PostBackUrl="admin-add-patient.aspx" />
            </div>
            <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
                <div class="search-bar">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Search patient by name..." Width="515px" />
                    &nbsp;&nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="search-btn" OnClick="btnSearch_Click" Text="Search" Width="250px" />
&nbsp;
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="reset-btn" OnClick="btnReset_Click" Width="250px" />
                </div>
            </asp:Panel>

            <!-- Patients Table -->
            <div class="table-container">
                <asp:GridView ID="gvPatients" runat="server" AutoGenerateColumns="False" CssClass="patient-table"
    DataKeyNames="PatientID" OnRowCommand="gvPatients_RowCommand" DataSourceID="SqlDataSource1" AllowPaging="True">
    <Columns>
        <asp:TemplateField HeaderText="Photo">
            <ItemTemplate>
                <asp:Image ID="imgProfile" runat="server" 
                    ImageUrl='<%# GetProfileImagePath(Eval("Profile_Pic")) %>' 
                    CssClass="profile-thumbnail" 
                    AlternateText="Profile Picture" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="PatientID" HeaderText="Patient ID" 
            HtmlEncode="false" ReadOnly="True" SortExpression="PatientID" />
        <asp:BoundField DataField="FullName" HeaderText="Full Name" 
            SortExpression="FullName" />
        <asp:BoundField DataField="ICNumber" HeaderText="IC Number" 
            SortExpression="ICNumber" />
        <asp:BoundField DataField="DOB" HeaderText="DOB" 
            SortExpression="DOB" DataFormatString="{0:dd/MM/yyyy}" />
        <asp:BoundField DataField="Gender" HeaderText="Gender" 
            SortExpression="Gender" />
        <asp:BoundField DataField="Email" HeaderText="Email" 
            SortExpression="Email" />
        <asp:BoundField DataField="Phone" HeaderText="Phone" 
            SortExpression="Phone" />
        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="action-btn edit-btn" 
    CommandName="Edit" CommandArgument='<%# Eval("PatientID") %>' />

            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                ConnectionString="<%$ ConnectionStrings:MyDbConnection %>"
                SelectCommand="SELECT * FROM [Patients]">
            </asp:SqlDataSource>
            </div>
        </div>
    </form>
</body>
</html>