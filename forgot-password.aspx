<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="forgot-password.aspx.cs" Inherits="Assignment.ForgotPassword" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forgot Password - Healthcare System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="css/login.css" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css" />
</head>

<style>
.error-message {
    color: #d93025; /* Google-like red */
    background-color: #ffe6e6;
    border: 1px solid #d93025;
    padding: 10px 15px;
    margin-bottom: 15px;
    border-radius: 5px;
    font-weight: 500;
    font-size: 14px;
    text-align: center;
}
</style>

<body class="login-page">
    <div class="login-background">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
    </div>

    <form id="form1" runat="server">
        <div class="login-container animate_animated animate_fadeIn">
            <div class="login-header">
                <div class="logo-container">
                    <i class="fas fa-heartbeat logo-icon"></i>
                    <span class="logo-text">AppDoc</span>
                </div>
                <h2>Forgot Password</h2>
                <p>Reset your account password securely</p>
            </div>

            <div class="login-body">
                <!-- ✅ Message only shows when set -->
                <asp:Label ID="lblMessage" runat="server" Visible="False" CssClass="error-message" />
                

                <div class="form-group">
                    <div class="input-with-icon">
                        <i class="fas fa-envelope input-icon"></i>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Enter your email" TextMode="Email" />
                    </div>
                </div>
                

                <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn-login" OnClick="btnNext_Click" />

                <!-- ✅ Reset panel shown after question is found -->
                <asp:Panel ID="pnlReset" runat="server" Visible="false">
                    <div class="form-group">
    <div class="input-with-icon">
        <i class="fas fa-question-circle input-icon"></i>
        <asp:Label ID="lblQuestion" runat="server" Font-Bold="true" CssClass="form-control label-as-input" />
    </div>
</div>


                    <div class="form-group">
                        <div class="input-with-icon">
                            <i class="fas fa-question input-icon"></i>
                            <asp:TextBox ID="txtAnswer" runat="server" CssClass="form-control" Placeholder="Your Answer" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="input-with-icon">
                            <i class="fas fa-lock input-icon"></i>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="New Password" />
                        </div>
                    </div>

                    <asp:Button ID="btnReset" runat="server" Text="Reset Password" CssClass="btn-login" OnClick="btnReset_Click" />
                </asp:Panel>

                <div class="login-footer">
                    <p>Back to <a href="login.aspx" class="register-link">Login</a></p>
                </div>
            </div>
        </div>
    </form>
</body>
</html>