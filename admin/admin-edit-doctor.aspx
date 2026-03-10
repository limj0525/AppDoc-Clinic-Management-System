<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin-edit-doctor.aspx.cs" Inherits="Assignment.admin_edit_doctor" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Doctor</title>
    <link rel="stylesheet" href="~/css/global.css" />
    <link rel="stylesheet" href="~/css/admin/admin-add-doctor.css" />
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

        <div class="main-content animate_animated animate_fadeIn">
            <div class="header">
                <h2><i class="fas fa-user-edit"></i> Edit Doctor</h2>
            </div>

            <div class="form-container">
                <div class="form-card">

                    <!-- Doctor ID -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-id-card"></i> Doctor ID:</div>
                        <div class="form-input">
                            <asp:TextBox ID="txtDoctorID" runat="server" CssClass="input-field" ReadOnly="true" />
                        </div>
                    </div>

                    <!-- Full Name -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-user"></i> Full Name:</div>
                        <div class="form-input">
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="input-field" required="required" />
                        </div>
                    </div>

                    <!-- IC Number -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-address-card"></i> IC Number:</div>
                        <div class="form-input">
                            <asp:TextBox ID="txtICNumber" runat="server" CssClass="input-field" placeholder="Without dash (-)" required="required" />
                        </div>
                    </div>

                    <!-- DOB -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-birthday-cake"></i> Date of Birth:</div>
                        <div class="form-input">
                            <asp:TextBox ID="txtDOB" runat="server" CssClass="input-field" TextMode="Date" />
                        </div>
                    </div>

                    <!-- Gender -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-venus-mars"></i> Gender:</div>
                        <div class="form-input">
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="input-field">
                                <asp:ListItem Text="Male" Value="Male" />
                                <asp:ListItem Text="Female" Value="Female" />
                                <asp:ListItem Text="Other" Value="Other" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Email -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-envelope"></i> Email:</div>
                        <div class="form-input">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" TextMode="Email" required="required" />
                        </div>
                    </div>

                    <!-- Phone -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-phone"></i> Phone Number:</div>
                        <div class="form-input">
                            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="input-field" TextMode="Phone" />
                        </div>
                    </div>

                    <!-- Address -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-map-marker-alt"></i> Address:</div>
                        <div class="form-input">
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="input-field" />
                        </div>
                    </div>

                    <!-- Specialization -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-stethoscope"></i> Specialization:</div>
                        <div class="form-input">
                            <asp:DropDownList
                                ID="ddlSpecialization"
                                runat="server"
                                CssClass="input-field"
                                AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select Specialization --" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Status -->
                    <div class="form-row">
                        <div class="form-label"><i class="fas fa-toggle-on"></i> Status:</div>
                        <div class="form-input">
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input-field">
                                <asp:ListItem Text="Active" Value="Active" />
                                <asp:ListItem Text="Inactive" Value="Inactive" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Buttons -->
                    <div class="form-footer">
                        <asp:Button ID="btnUpdate" runat="server" Text="Update Doctor" CssClass="submit-btn" OnClick="btnUpdate_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="cancel-btn" OnClick="btnCancel_Click" CausesValidation="false" />
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>