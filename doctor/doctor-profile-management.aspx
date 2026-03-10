<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="doctor-profile-management.aspx.cs" Inherits="Web_Assignment.doctor_profile_management" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Doctor Profile Management</title>
    <link rel="stylesheet" href="../css/global.css" />
    <link rel="stylesheet" href="../css/doctor/doctor-profile-management.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css" />
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

        <!-- Sidebar -->
        <div class="sidebar">
            <h2>Doctor Panel</h2>
            <a href="doctor-dashboard.aspx">Dashboard <i class="fa-solid fa-house"></i></a>
            <a href="doctor-view-appointment.aspx">View Appointment <i class="fa-solid fa-calendar-check"></i></a>
            <a href="doctor-profile-management.aspx" class="active">Profile Management <i class="fa-solid fa-user"></i></a>
            <a href="../index.aspx">Logout <i class="fas fa-sign-out-alt"></i></a>
                                        <!-- Translate Widget at the Bottom -->
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>
        </div>



        <div class="dashboard-container">
    <!-- Dashboard Header -->
    <div class="dashboard-header">
        <h1>Profile Management</h1>
        <div class="date-label">
            <i class="fa-solid fa-calendar"></i>
            Today's Date: <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
        </div>
    </div>

            <!-- Current Profile Information -->
            <div class="current-profile animate__animated animate__fadeIn">
                <h2>Current Profile Information <i class="fa-solid fa-circle-info"></i></h2>
                <div class="profile-details">
                    <p><strong>Full Name:</strong> <asp:Label ID="lblFullName" runat="server"></asp:Label></p>
                    <p><strong>Email:</strong> <asp:Label ID="lblEmail" runat="server"></asp:Label></p>
                    <p><strong>Phone Number:</strong> <asp:Label ID="lblPhoneNumber" runat="server"></asp:Label></p>
                    <p><strong>Specialization:</strong> <asp:Label ID="lblSpecialization" runat="server"></asp:Label></p>
                </div>
            </div>

            <!-- Profile Form -->
            <div class="profile-form animate__animated animate__fadeIn">
                <h2>Update Your Profile <i class="fa-solid fa-pen"></i></h2>
                <div class="form-group">
                    <label for="txtFullName">Full Name:</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Leave blank to keep current value"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtEmail">Email:</label>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="Leave blank to keep current value"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtPhoneNumber">Phone Number:</label>
                    <asp:TextBox ID="txtPhoneNumber" runat="server" TextMode="Phone" CssClass="form-control" placeholder="Leave blank to keep current value"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtSpecialization">Specialization:</label>
                    <asp:TextBox ID="txtSpecialization" runat="server" CssClass="form-control" placeholder="Leave blank to keep current value"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtPassword">New Password:</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Leave blank to keep current value"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtConfirmPassword">Confirm Password:</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Leave blank to keep current value"></asp:TextBox>
                </div>
                <div class="form-group">
                   <asp:Button ID="btnSaveProfile" runat="server" Text="Save Changes" CssClass="save-btn" OnClick="btnSaveProfile_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>