<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="privacy.aspx.cs" Inherits="Web_Assignment.privacy" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Privacy Policy - Healthcare System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="css/login.css" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
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
                <h2>Privacy Policy</h2>
                <p>We value and protect your personal data</p>
            </div>

            <div class="login-body">
                <div class="terms-content" style="text-align:left; max-height:400px; overflow-y:auto; padding: 15px; background-color: #fff; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
                    <h3>1. Information Collection</h3>
                    <p>We collect personal and health-related information for the purpose of providing healthcare services through the AppDoc system.</p>

                    <h3>2. How We Use Your Information</h3>
                    <p>Data is used to facilitate appointments, manage your medical profile, and communicate important updates about your care.</p>

                    <h3>3. Data Security</h3>
                    <p>All stored data is encrypted and protected using industry-standard security measures. Access is restricted to authorized personnel only.</p>

                    <h3>4. Sharing Information</h3>
                    <p>We do not sell or share your information with third parties. Information may be shared only with licensed healthcare professionals as necessary for care delivery.</p>

                    <h3>5. Your Rights</h3>
                    <p>You have the right to view, update, or delete your personal information at any time. You can do this through your profile settings or by contacting support.</p>

                    <h3>6. Contact</h3>
                    <p>If you have any questions or concerns about your privacy, please contact us at <strong>privacy@appdoc.com</strong>.</p>
                </div>

                <div class="form-options" style="margin-top: 20px;">
                    <a href="register.aspx" class="register-link">← Back to Registration</a>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
