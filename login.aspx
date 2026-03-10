<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Assignment.login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - Healthcare System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <!-- CSS Links -->
    <link rel="stylesheet" href="css/login.css" />
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <!-- Font Awesome Icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <!-- Animate.css -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css" />

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
<body class="login-page">
    <div class="login-background">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
    </div>
    
    <form id="form1" runat="server">
        <div class="login-container animate__animated animate__fadeIn">
            <div class="login-header">
                <div class="logo-container">
                    <i class="fas fa-heartbeat logo-icon"></i>
                    <span class="logo-text">AppDoc</span>
                </div>
                <h2>Welcome Back</h2>
                <p>Please login to access your dashboard</p>
            </div>

            <div class="login-body">
                <asp:Label ID="lblMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>

                <div class="form-group">
                    <div class="input-with-icon">
                        <i class="fas fa-user input-icon"></i>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Email"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group">
                    <div class="input-with-icon">
                        <i class="fas fa-lock input-icon"></i>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password"></asp:TextBox>
                        <span class="toggle-password" onclick="togglePassword()">
                            <i class="fas fa-eye" id="toggleIcon"></i>
                        </span>
                    </div>
                </div>

                <div class="form-options">
                    <a href="forgot-password.aspx" class="forgot-password">Forgot password?</a>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" />

                <div class="login-footer">
                    <p>Don't have an account? <a href="register.aspx" class="register-link">Sign up</a></p>
                    <p class="copyright">© <%= DateTime.Now.Year %> Healthcare System. All rights reserved.</p>
                </div>
                                <div class="translate-container">
                    <div id="google_translate_element"></div>
                </div>
            </div>
        </div>
    </form>

    <script>
        function togglePassword() {
            const passwordField = document.getElementById('<%= txtPassword.ClientID %>');
            const icon = document.getElementById('toggleIcon');

            if (passwordField.type === "password") {
                passwordField.type = "text";
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
            } else {
                passwordField.type = "password";
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            }
        }

        // Add shake animation to form on error
        if (document.querySelector('.error-message') && document.querySelector('.error-message').textContent.trim() !== '') {
            document.querySelector('.login-container').classList.add('animate__animated', 'animate__shakeX');
        }
    </script>
</body>
</html>