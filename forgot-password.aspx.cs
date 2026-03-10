using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Assignment
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Visible = false;
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lblMessage.Text = "Please enter your email.";
                lblMessage.Visible = true;
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT SecurityQuestion FROM Patients WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    lblQuestion.Text = "Security Question: " + result.ToString();
                    pnlReset.Visible = true;
                    btnNext.Visible = false;
                    lblMessage.Text = "";
                    lblMessage.Visible = false;
                    ViewState["Email"] = email;
                }
                else
                {
                    lblMessage.Text = "Email not found.";
                    lblMessage.Visible = true;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            string email = ViewState["Email"]?.ToString();
            string answer = txtAnswer.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();

            if (string.IsNullOrEmpty(answer) || string.IsNullOrEmpty(newPassword))
            {
                lblMessage.Text = "Please provide both answer and new password.";
                lblMessage.Visible = true;
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand checkCmd = new SqlCommand("SELECT SecurityAnswer FROM Patients WHERE Email = @Email", conn);
                checkCmd.Parameters.AddWithValue("@Email", email);
                string correctAnswer = checkCmd.ExecuteScalar()?.ToString();

                if (correctAnswer != null && correctAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
                {
                    string hashedPassword = HashPassword(newPassword);

                    SqlCommand updateCmd = new SqlCommand("UPDATE Patients SET Password = @Password WHERE Email = @Email", conn);
                    updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
                    updateCmd.Parameters.AddWithValue("@Email", email);
                    updateCmd.ExecuteNonQuery();

                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "Password has been reset successfully.";
                    lblMessage.Visible = true;
                    pnlReset.Visible = false;
                }
                else
                {
                    lblMessage.Text = "Incorrect answer.";
                    lblMessage.Visible = true;
                }
            }
        }

        // This method is identical to the one in register.aspx.cs for consistency.
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}