<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin-manage-doctor.aspx.cs" Inherits="Assignment.admin_manage_doctor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Doctor Management</title>
    <!-- CSS Links -->
    <link rel="stylesheet" href="/css/global.css" />
<link rel="stylesheet" href="/css/admin/admin-manage-doctor.css" />


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
            <a href="admin-manage-doctor.aspx" class="active"><i class="fas fa-user-md"></i> Manage Doctors</a>
            <a href="admin-manage-patient.aspx"><i class="fas fa-hospital-user"></i> Manage Patients</a>
            <a href="admin-manage-appointments.aspx"><i class="fas fa-calendar-check"></i> Manage Appointments</a>
            <a href="admin-reports.aspx"><i class="fas fa-chart-bar"></i> Reports</a>
            <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
            <!-- Translate Widget at the Bottom -->
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>
        </div>



        <!-- Main Content -->
        <div class="dashboard-container animate__animated animate__fadeIn">
            <!-- Header -->
           <div class="header">
                <h2><i class="fas fa-user-md"></i> Doctor Management</h2>
                <asp:Button ID="btnAddDoctor" runat="server" CssClass="add-new-btn" Text="Add New Doctor" 
                    Height="41px" Width="200px" PostBackUrl="admin-add-doctor.aspx" />
            </div>
            <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
                <div class="search-bar">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" placeholder="Search doctor by name..." Width="515px" />
                    &nbsp;&nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="search-btn" OnClick="btnSearch_Click" Text="Search" Width="250px" />
&nbsp;
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="reset-btn" OnClick="btnReset_Click" Width="250px" />
                    <br />
                </div>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="filter-dropdown" 
                    OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Text="All Status" Value="" />
                    <asp:ListItem Text="Active" Value="Active" />
                    <asp:ListItem Text="Inactive" Value="Inactive" />
                </asp:DropDownList>

            </asp:Panel>



            <!-- Doctors Table -->
            <div class="table-container">
                <asp:GridView ID="gvDoctors" runat="server" AutoGenerateColumns="False" CssClass="doctor-table" 
                    DataSourceID="SqlDataSource1" DataKeyNames="DoctorID" OnRowCommand="gvDoctors_RowCommand" AllowPaging="True">
                    <Columns>
                        <asp:BoundField DataField="DoctorID" HeaderText="Doctor ID"
                            HtmlEncode="false" ReadOnly="True" SortExpression="DoctorID" />
                        <asp:BoundField DataField="FullName" HeaderText="Name" 
                            SortExpression="FullName" />
                        <asp:BoundField DataField="ICNumber" HeaderText="IC Number" 
                            SortExpression="ICNumber" />
                        <asp:BoundField DataField="DOB" HeaderText="DOB" 
                            SortExpression="DOB" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Gender" HeaderText="Gender" 
                            SortExpression="Gender" />
                        <asp:BoundField DataField="Email" HeaderText="Email" 
                            SortExpression="Email" />
                        <asp:BoundField DataField="PhoneNumber" HeaderText="Phone" 
                            SortExpression="PhoneNumber" />
                        <asp:BoundField DataField="Specialization" HeaderText="Specialization" 
                            SortExpression="Specialization" />
                        <asp:BoundField DataField="Status" HeaderText="Status" 
                            SortExpression="Status" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="action-btn edit-btn" 
                                    CommandName="Edit" CommandArgument='<%# Eval("DoctorID") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                    ConnectionString="<%$ ConnectionStrings:MyDbConnection %>"
                    SelectCommand="SELECT * FROM [Doctors]">
                </asp:SqlDataSource>
            </div>
        </div>
    </form>
</body>
</html>