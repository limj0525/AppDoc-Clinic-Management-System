<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CancelAppointment.aspx.cs" Inherits="patientsystem.CancelAppointment" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Cancel Appointment</title>
    <link rel="stylesheet" href="../css/global.css">
    <link rel="stylesheet" href="../css/patient/patient-cancel-appointment.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
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
    <!-- Consistent Sidebar Navigation -->
    <div class="sidebar">
        <h2>Patient</h2>
        <a href="PatientMain.aspx"><i class="fas fa-home"></i> Dashboard</a>
        <a href="BookAppointment.aspx"><i class="fas fa-calendar-plus"></i> Book Appointment</a>
        <a href="CancelAppointment.aspx" class="active"><i class="fas fa-calendar-times"></i> Cancel Appointment</a>
        <a href="AptHistory.aspx"><i class="fas fa-history"></i> Appointment History</a>
        <a href="PatientProfile.aspx" >Profile Management <i class="fas fa-user-cog"></i></a>
        <a href="../index.aspx"><i class="fas fa-sign-out-alt"></i> Logout</a>
                        <!-- Translate Widget at the Bottom -->
    <div class="translate-container">
        <div id="google_translate_element"></div>
    </div>
    </div>

        

    <!-- Main Content Area -->
    <div class="content">
        <form id="form1" runat="server">
            <!-- Standard Header with Date -->
            <div class="dashboard-header">
                <h1>Cancel Appointment <i class="fas fa-calendar-times"></i></h1>
                <div class="date-label">
                    <i class="fa-solid fa-calendar"></i>
                    Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </div>
            </div>

            <!-- Cancellation Form in Card Layout -->
            <div class="card cancel-form">
                <h2>Select Appointment to Cancel <i class="fas fa-search"></i></h2>
                
                <!-- Appointment Selection -->
            <div class="form-group">
              <label for="ddlAppointments">Your Upcoming Appointments<i class="fas fa-calendar-check"></i> </label>
                  <asp:DropDownList ID="ddlAppointments" runat="server" CssClass="form-control" AutoPostBack="true" 
                      OnSelectedIndexChanged="ddlAppointments_SelectedIndexChanged">
                  <asp:ListItem Text="-- Select an Appointment --" Value="" />
              </asp:DropDownList>
            </div>

                <!-- Appointment Details Card -->
                <asp:Panel ID="pnlAppointmentDetails" runat="server" CssClass="card appointment-details" Visible="false">
                    <h2>Appointment Details <i class="fas fa-info-circle"></i> </h2>
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="detail-label"><i class="fas fa-calendar-day"></i> Booking Date:</span>
                            <asp:Label ID="lblBookDate" runat="server" CssClass="detail-value"></asp:Label>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label"><i class="fas fa-id-badge"></i> Appointment ID:</span>
                            <asp:Label ID="lblAppId" runat="server" CssClass="detail-value"></asp:Label>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label"><i class="fas fa-user-md"></i> Doctor:</span>
                            <asp:Label ID="lblDoctor" runat="server" CssClass="detail-value"></asp:Label>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label"><i class="fas fa-clock"></i> Scheduled Time:</span>
                            <div class="detail-value time-group">
                                <asp:Label ID="lblAppDate" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Cancel Button -->
                <div class="form-group">
                   <asp:Button ID="btnCancelBook" runat="server" Text="Cancel Appointment"
    CssClass="btn-primary btn-cancel"
    OnClick="btnCancelBook_Click"
    OnClientClick="return confirmCancel();" />

                </div>

                <!-- Status Messages -->
                <asp:Panel ID="confirmationMessage" runat="server" CssClass="alert alert-success" Visible="false">
                    <i class="fas fa-check-circle"></i> 
                    <asp:Label runat="server" Text="Your appointment has been successfully canceled!"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="errorMessage" runat="server" CssClass="alert alert-error" Visible="false">
                    <i class="fas fa-exclamation-circle"></i> 
                    <asp:Label runat="server" Text="An error occurred while canceling the appointment. Please try again."></asp:Label>
                </asp:Panel>
            </div>
        </form>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
<script type="text/javascript">
    function confirmCancel() {
        event.preventDefault();

        Swal.fire({
            title: 'Cancel Appointment?',
            text: "Are you sure you want to cancel this appointment? This action cannot be undone.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, cancel it!',
            cancelButtonText: 'No, keep it'
        }).then((result) => {
            if (result.isConfirmed) {
                __doPostBack('<%= btnCancelBook.UniqueID %>', '');
            }
        });

        return false;
    }
</script>

</body>
</html>