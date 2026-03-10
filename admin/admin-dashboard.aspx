<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin-dashboard.aspx.cs" Inherits="Assignment.admin_dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard</title>
    <link rel="stylesheet" href="~/css/global.css" />
    <link rel="stylesheet" href="~/css/admin/admin-dashboard.css" />
    <!-- Add Font Awesome and Animate.css -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
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
        <!-- Sidebar with Icons -->
        <div class="sidebar">
            <h2>Admin Panel</h2>
            <a href="admin-dashboard.aspx" class="active"><i class="fas fa-tachometer-alt"></i> Dashboard</a>
            <a href="admin-manage-doctor.aspx"><i class="fas fa-user-md"></i> Manage Doctors</a>
            <a href="admin-manage-patient.aspx"><i class="fas fa-hospital-user"></i> Manage Patients</a>
            <a href="admin-manage-appointments.aspx"><i class="fas fa-calendar-check"></i> Manage Appointments</a>
            <a href="admin-reports.aspx"><i class="fas fa-chart-bar"></i> Reports</a>
            <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
            <!-- Translate Widget at the Bottom -->
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>
        </div>



        <div class="dashboard-container">
            <!-- Animated Header -->
            <div class="dashboard-header animate__animated animate__fadeIn">
                <h1><i class="fas fa-tachometer-alt"></i> Admin Dashboard</h1>
                <div class="date-label">
                    <i class="fas fa-calendar-day"></i>
                    Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

            <!-- Welcome Message with Animation -->

            <!-- Animated Stats Cards -->
            <div class="summary-cards">
                <div class="card animate__animated animate__bounce">
                    <h3><i class="fas fa-calendar-check"></i> Total Appointments</h3>
                    <p><asp:Label ID="lblTotalAppointment" runat="server"></asp:Label></p>
                </div>
                <div class="card animate__animated animate__bounce" style="animation-delay: 0.2s">
                    <h3><i class="fas fa-user-md"></i> Registered Doctors</h3>
                    <p><asp:Label ID="lblRegisteredDoctor" runat="server"></asp:Label></p>
                </div>
                <div class="card animate__animated animate__bounce" style="animation-delay: 0.4s">
                    <h3><i class="fas fa-hospital-user"></i> Registered Patients</h3>
                    <p><asp:Label ID="lblRegisteredPatient" runat="server"></asp:Label></p>
                </div>
            </div>

            <!-- Appointments Section with Animation -->
            <div class="appointments-section animate__animated animate__fadeIn">
                <h2><i class="fas fa-calendar-alt"></i> Upcoming Appointments For The Next 7 Days</h2>
                <p><i class="fas fa-info-circle"></i> Here's quick access to upcoming appointments for the next 7 days. More details are available in the 
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="admin-manage-appointments.aspx">
                        <i class="fas fa-external-link-alt"></i> Appointments
                    </asp:HyperLink>
                    section.
                </p>
                <asp:GridView ID="gvUpcomingAppointments" runat="server" CssClass="appointment-grid"
                    AutoGenerateColumns="False" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="PatientName" HeaderText="<i class='fas fa-user'></i> Patient" HtmlEncode="false" />
                                <asp:BoundField DataField="AppointmentDate" HeaderText="Appointment Date" SortExpression="AppointmentDate" 
            DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="DoctorName" HeaderText="<i class='fas fa-user-md'></i> Doctor" HtmlEncode="false" />
                        <asp:BoundField DataField="Status" HeaderText="<i class='fas fa-info-circle'></i> Status" HtmlEncode="false" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>