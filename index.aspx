<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>AppDoc - Appointment System</title>
    <link rel="stylesheet" href="css/global.css">
    <link rel="stylesheet" href="css/index.css">
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
    <div class="index-container">
        <!-- Header Section -->
        <header class="index-header">
            <div class="logo">
                <i class="fas fa-heartbeat"></i>
                <h1>AppDoc</h1>
            </div>

                        <div class="header-right">
            <div class="translate-container">
                <div id="google_translate_element"></div>
            </div>

            <div class="auth-buttons">
                <a href="login.aspx" class="btn btn-outline">Login</a>
                <a href="register.aspx" class="btn btn-primary">Register</a>
            </div>
        </header>

        <!-- Hero Section -->
        <section class="hero">
            <div class="hero-content">
                <h2>Streamlined Patient Management</h2>
                <p class="lead">AppDoc provides healthcare professionals with an intuitive platform to manage patient appointments, medical records, and clinic operations efficiently.</p>
                <div class="hero-buttons">
                    <a href="register.aspx" class="btn btn-primary btn-large">Get Started</a>
                    <a href="#features" class="btn btn-outline btn-large">
                        <i class="fas fa-arrow-down"></i> Learn More
                    </a>
                </div>
            </div>
            <div class="hero-image">
                <img src="image/clinic1.jpg" alt="Medical Team">
            </div>
        </section>

        <!-- Features Section -->
        <section id="features" class="features">
            <div class="section-header">
                <h2><i class="fas fa-star"></i> Key Features</h2>
                <p>Designed to simplify healthcare management</p>
            </div>
            
            <div class="feature-cards">
                <div class="feature-card">
                    <div class="feature-icon">
                        <i class="fas fa-calendar-check"></i>
                    </div>
                    <h3>Appointment Management</h3>
                    <p>Easily schedule, reschedule, and track patient appointments with our intuitive calendar system.</p>
                </div>
                
                <div class="feature-card">
                    <div class="feature-icon">
                        <i class="fas fa-user-md"></i>
                    </div>
                    <h3>Doctor Profiles</h3>
                    <p>Comprehensive profiles for healthcare providers with specialization and availability details.</p>
                </div>
                
                <div class="feature-card">
                    <div class="feature-icon">
                        <i class="fas fa-file-medical"></i>
                    </div>
                    <h3>Medical Records</h3>
                    <p>Secure digital storage for patient histories, prescriptions, and treatment plans.</p>
                </div>
            </div>
        </section>

        <!-- Footer -->
        <footer class="index-footer">
            <div class="footer-content">
                <div class="footer-logo">
                    <i class="fas fa-heartbeat"></i>
                    <span>AppDoc</span>
                </div>
                <div class="footer-links">
                    <a href="#">About Us</a>
                    <a href="#">Contact</a>
                    <a href="#">Privacy Policy</a>
                    <a href="#">Terms of Service</a>
                </div>
                <div class="footer-copyright">
                    &copy; 2023 AppDoc Patient Management System
                </div>
            </div>
        </footer>
    </div>
</body>
</html>