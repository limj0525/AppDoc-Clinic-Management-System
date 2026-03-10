<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin-reports.aspx.cs" Inherits="Assignment.admin_reports" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Reports</title>
    <!-- CSS Links -->
    <link rel="stylesheet" href="~/css/global.css" />
    <link rel="stylesheet" href="~/css/admin/admin-reports.css" />
    <!-- Font Awesome Icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <!-- Animate.css -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css" />
    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
            <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        function googleTranslateElementInit() {
            new google.translate.TranslateElement(
                { pageLanguage: 'en' },
                'google_translate_element'
            );
        }
    </script>

    <script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>

</head>
<body>
    <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div class="sidebar">
            <h2>Admin Panel</h2>
            <a href="admin-dashboard.aspx"><i class="fas fa-tachometer-alt"></i> Dashboard</a>
            <a href="admin-manage-doctor.aspx"><i class="fas fa-user-md"></i> Manage Doctors</a>
            <a href="admin-manage-patient.aspx"><i class="fas fa-hospital-user"></i> Manage Patients</a>
            <a href="admin-manage-appointments.aspx"><i class="fas fa-calendar-check"></i> Manage Appointments</a>
            <a href="admin-reports.aspx"class="active"><i class="fas fa-chart-bar"></i> Reports</a>
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
                <h2><i class="fas fa-chart-bar"></i> Admin Reports</h2>
                <div class="date-label">
                    <i class="fas fa-calendar-day"></i>
                    Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

            <!-- Reports Section -->
            <div class="reports-container">

                <!-- Appointment Summary Report -->
                <div class="report-card animate_animated animate_fadeIn">
                    <h3><i class="fas fa-calendar-check"></i> Appointment Summary Report</h3>
                    <asp:GridView ID="gvAppointmentSummary" runat="server" AutoGenerateColumns="False" CssClass="report-table"
                        DataSourceID="SqlDataSource1" DataKeyNames="AppointmentID" AllowPaging="True">
                        <Columns>
                            <asp:BoundField DataField="AppointmentID" HeaderText="AppointmentID" ReadOnly="True" />
                            <asp:BoundField DataField="PatientID" HeaderText="PatientID" />
                            <asp:BoundField DataField="DoctorID" HeaderText="DoctorID" />
                            <asp:BoundField DataField="AppointmentDate" HeaderText="AppointmentDate" />
                            <asp:BoundField DataField="StartTime" HeaderText="StartTime" />
                            <asp:BoundField DataField="EndTime" HeaderText="EndTime" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:BoundField DataField="TreatmentType" HeaderText="TreatmentType" />
                            <asp:BoundField DataField="Duration" HeaderText="Duration" />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                        ConnectionString="<%$ ConnectionStrings:MyDbConnection %>"
                        SelectCommand="SELECT * FROM [Appointments]"></asp:SqlDataSource>
                    <asp:Button ID="btnReport1" runat="server" Text="Generate PDF" CssClass="filter-btn" OnClick="btnReport1_Click"/>
                </div>

                <!-- Doctor Report -->
                <div class="report-card animate_animated animate_fadeIn">
                    <h3><i class="fas fa-user-md"></i> Doctor Report</h3>
                    <div class="table-container">
                        <asp:GridView ID="gvDoctorReport" runat="server" AutoGenerateColumns="False" CssClass="report-table"
                            DataKeyNames="DoctorID" DataSourceID="SqlDataSource2" AllowPaging="True">
                            <Columns>
                                <asp:BoundField DataField="DoctorID" HeaderText="DoctorID" ReadOnly="True" SortExpression="DoctorID" />
                                <asp:BoundField DataField="FullName" HeaderText="FullName" SortExpression="FullName" />
                                <asp:BoundField DataField="ICNumber" HeaderText="ICNumber" SortExpression="ICNumber" />
                                <asp:BoundField DataField="DOB" HeaderText="DOB" SortExpression="DOB" DataFormatString="{0:dd-MMM-yyyy}"/>
                                <asp:BoundField DataField="Gender" HeaderText="Gender" SortExpression="Gender" />
                                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                                <asp:BoundField DataField="PhoneNumber" HeaderText="PhoneNumber" SortExpression="PhoneNumber" />
                                <asp:BoundField DataField="Address" HeaderText="Address" SortExpression="Address" />
                                <asp:BoundField DataField="Specialization" HeaderText="Specialization" SortExpression="Specialization" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:SqlDataSource ID="SqlDataSource2" runat="server"
                        ConnectionString="<%$ ConnectionStrings:MyDbConnection %>"
                        SelectCommand="SELECT * FROM [Doctors]"></asp:SqlDataSource>
                    <asp:Button ID="btnDoctorReport" runat="server" Text="Generate PDF" CssClass="filter-btn" OnClick="btnDoctorReport_Click" />
                </div>
                <!-- Patient Report -->
                <div class="report-card animate_animated animate_fadeIn">
                    <h3><i class="fas fa-hospital-user"></i> Patient Report</h3>
                    <div class="table-container">
                        <asp:GridView ID="gvPatientReport" runat="server" AutoGenerateColumns="False" CssClass="report-table"
                            DataKeyNames="PatientID" DataSourceID="SqlDataSource3" AllowPaging="True">
                            <Columns>
                                <asp:BoundField DataField="PatientID" HeaderText="PatientID" ReadOnly="True" SortExpression="PatientID" />
                                <asp:BoundField DataField="FullName" HeaderText="FullName" SortExpression="FullName" />
                                <asp:BoundField DataField="ICNumber" HeaderText="ICNumber" SortExpression="ICNumber" />
                                <asp:BoundField DataField="DOB" HeaderText="DOB" SortExpression="DOB" DataFormatString="{0:dd-MMM-yyyy}" />
                                <asp:BoundField DataField="Gender" HeaderText="Gender" SortExpression="Gender" />
                                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                                <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" />
                                <asp:BoundField DataField="Address" HeaderText="Address" SortExpression="Address" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:SqlDataSource ID="SqlDataSource3" runat="server"
                        ConnectionString="<%$ ConnectionStrings:MyDbConnection %>"
                        SelectCommand="SELECT * FROM [Patients]"></asp:SqlDataSource>
                    <asp:Button ID="btnPatientReport" runat="server" Text="Generate PDF" CssClass="filter-btn" OnClick="btnPatientReport_Click" />
                </div>


            </div>
        </div>
    </form>
</body>
</html>