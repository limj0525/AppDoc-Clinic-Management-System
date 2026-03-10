<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin-manage-appointments.aspx.cs" Inherits="Assignment.admin_manage_appointments" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Appointment Management</title>
    <!-- CSS Links -->
    <link rel="stylesheet" href="~/css/global.css" />
    <link rel="stylesheet" href="~/css/admin/admin-manage-appointment.css" />
    <!-- Font Awesome Icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <!-- Animate.css -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css" />\
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

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

<style>
.cancel-btn:disabled,
.edit-btn:disabled {
    background-color: #ccc;
    color: #888;
    border: none;
    cursor: not-allowed;
    pointer-events: none;
}
</style>
<body>
    <form id="form1" runat="server">
        <div class="sidebar">
            <h2>Admin Panel</h2>
            <a href="admin-dashboard.aspx" ><i class="fas fa-tachometer-alt"></i> Dashboard</a>
            <a href="admin-manage-doctor.aspx"><i class="fas fa-user-md"></i> Manage Doctors</a>
            <a href="admin-manage-patient.aspx"><i class="fas fa-hospital-user"></i> Manage Patients</a>
            <a href="admin-manage-appointments.aspx" class="active"><i class="fas fa-calendar-check"></i> Manage Appointments</a>
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
                <h2><i class="fas fa-calendar-check"></i> Appointment Management</h2>
            </div>

            <!-- Filter Section -->
            <div class="filter-section animate__animated animate__fadeIn">
                <div class="filter-row">
                    <div class="filter-group">
                        <label for="ddlDoctor"><i class="fas fa-user-md"></i> Doctor:</label>
                        <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="filter-control" AutoPostBack="false"></asp:DropDownList>
                    </div>

                    <div class="filter-group">
                        <label for="txtDate"><i class="fas fa-calendar-day"></i> Date:</label>
                        <asp:TextBox ID="txtDate" runat="server" CssClass="filter-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="filter-group">
                        <label for="ddlStatus"><i class="fas fa-info-circle"></i> Status:</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-control">
                            <asp:ListItem Text="All" Value="All"></asp:ListItem>
                            <asp:ListItem Text="Scheduled" Value="Scheduled"></asp:ListItem>
                            <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
                            <asp:ListItem Text="Cancelled" Value="Cancelled"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="filter-group">
                        <asp:Button ID="btnFilter" runat="server" Text="Apply Filters" CssClass="filter-btn" OnClick="FilterAppointments" />
                    </div>

                    <div class="filter-group">
                        <asp:Button ID="btnClearFilters" runat="server" Text="Remove Filter" CssClass="clear-btn" OnClick="ClearFilters" />
                    </div>
                </div>
            </div>

            <!-- Appointments Table -->
            <div class="table-container">
                <asp:GridView ID="gvAppointments" runat="server" AutoGenerateColumns="False" 
                                CssClass="appointment-table"
                                DataKeyNames="AppointmentID"
                                DataSourceID="SqlDataSource1"
                                OnRowCommand="gvAppointments_RowCommand"
                                OnPageIndexChanging="gvAppointments_PageIndexChanging"
                                AllowPaging="True">

                    
                    <Columns>
                        <asp:BoundField DataField="AppointmentID" HeaderText="Appointment ID" />
                        <asp:BoundField DataField="PatientName" HeaderText="Patient Name" />
                        <asp:BoundField DataField="DoctorName" HeaderText="Doctor Name" />
                        <asp:BoundField DataField="AppointmentDate" HeaderText="Appointment Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="StartTime" HeaderText="Start Time" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="TreatmentType" HeaderText="Treatment Type" />

                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="action-btn edit-btn"
                                    CommandName="EditAppointment" CommandArgument='<%# Eval("AppointmentID") %>'
                                    Enabled='<%# Eval("Status").ToString() == "Scheduled" %>' />

                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="action-btn cancel-btn"
                                    CommandName="CancelAppointment" CommandArgument='<%# Eval("AppointmentID") %>'
                                    Enabled='<%# Eval("Status").ToString() == "Scheduled" %>'
                                    OnClientClick='<%# Eval("Status").ToString() != "Cancelled" ? "return confirm(\"Are you sure you want to cancel this appointment?\");" : "" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <EmptyDataTemplate>
                        <div class="no-data-message">
                            <i class="fas fa-info-circle"></i> No appointments found. Please adjust your filter options and try again.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>

<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:MyDbConnection %>" 
    SelectCommand="SELECT a.AppointmentID, a.PatientID, a.DoctorID, p.FullName AS PatientName, d.FullName AS DoctorName, 
                          a.AppointmentDate, a.StartTime, a.Status, a.TreatmentType
                   FROM Appointments a
                   JOIN Patients p ON a.PatientID = p.PatientID
                   JOIN Doctors d ON a.DoctorID = d.DoctorID">
</asp:SqlDataSource>


            </div>

        </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
    </form>
</body>
</html>
