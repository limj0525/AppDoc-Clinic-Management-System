using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Assignment
{
    public partial class admin_edit_patient : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string patientID = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(patientID))
                {
                    LoadPatientData(patientID);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "noPatientId",
                        @"Swal.fire({
        icon: 'warning',
        title: 'Missing ID',
        text: 'Patient ID not provided.'
    }).then(() => {
        window.location = 'admin-manage-patient.aspx';
    });",
                        true
                    );

                }
            }
        }

        private void LoadPatientData(string patientID)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientID);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtPatientID.Text = dr["PatientID"].ToString();
                        txtFullName.Text = dr["FullName"].ToString();
                        txtICNumber.Text = dr["ICNumber"].ToString();
                        txtDOB.Text = Convert.ToDateTime(dr["DOB"]).ToString("yyyy-MM-dd");
                        ddlGender.SelectedValue = dr["Gender"].ToString();
                        txtEmail.Text = dr["Email"].ToString();
                        txtPhone.Text = dr["Phone"].ToString();
                        txtAddress.Text = dr["Address"].ToString();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "patientNotFound",
                            @"Swal.fire({
        icon: 'error',
        title: 'Not Found',
        text: 'Patient not found.'
    }).then(() => {
        window.location = 'admin-manage-patient.aspx';
    });",
                            true
                        );

                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            // Validate required fields
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrEmpty(fullName))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "fullNameRequired",
                    "Swal.fire({ icon: 'warning', title: 'Full Name Required', text: 'Please enter the full name.' });", true);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "emailRequired",
                    "Swal.fire({ icon: 'warning', title: 'Email Required', text: 'Please enter the email address.' });", true);
                return;
            }

            if (string.IsNullOrEmpty(phone))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "phoneRequired",
                    "Swal.fire({ icon: 'warning', title: 'Phone Number Required', text: 'Please enter the phone number.' });", true);
                return;
            }

            if (string.IsNullOrEmpty(address))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addressRequired",
                    "Swal.fire({ icon: 'warning', title: 'Address Required', text: 'Please enter the address.' });", true);
                return;
            }

            // Validate phone number (between 10 to 15 characters and numeric)
            if (phone.Length < 10 || phone.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidPhone",
                    "Swal.fire({ icon: 'warning', title: 'Invalid Phone Number', text: 'Phone number must be between 10 and 15 digits and contain only numbers.' });", true);
                return; // Stop the save process if phone number is invalid
            }

            // Handle profile picture upload validation
            string imagePath = "";
            if (fileProfilePic.HasFile)
            {
                string fileExt = Path.GetExtension(fileProfilePic.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!Array.Exists(allowedExtensions, ext => ext == fileExt))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "invalidFile",
                        "Swal.fire({ icon: 'warning', title: 'Invalid File Format', text: 'Only image files (.jpg, .jpeg, .png, .gif) are allowed.' });", true);
                    return; // Stop the save process if file is invalid
                }

                string fileName = Path.GetFileName(fileProfilePic.FileName);
                imagePath = "~/Uploads/" + fileName;
                fileProfilePic.SaveAs(Server.MapPath(imagePath));
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE Patients SET 
                         FullName = @FullName,
                         ICNumber = @ICNumber,
                         DOB = @DOB,
                         Gender = @Gender,
                         Email = @Email,
                         Phone = @Phone,
                         Address = @Address" +
                                       (imagePath != "" ? ", Profile_Pic = @Profile_Pic" : "") +
                                       " WHERE PatientID = @PatientID";  // Note: Ensure space before WHERE

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", txtPatientID.Text);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@ICNumber", txtICNumber.Text);
                    cmd.Parameters.AddWithValue("@DOB", string.IsNullOrWhiteSpace(txtDOB.Text) ? (object)DBNull.Value : txtDOB.Text);
                    cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);

                    if (imagePath != "")
                    {
                        cmd.Parameters.AddWithValue("@Profile_Pic", imagePath);
                    }

                    con.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "patientUpdateSuccess",
                            @"Swal.fire({
                        icon: 'success',
                        title: 'Updated!',
                        text: 'Patient updated successfully!'
                    }).then(() => {
                        window.location = 'admin-manage-patient.aspx';
                    });",
                            true
                        );
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "patientUpdateFail",
                            @"Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Update failed.'
                    });",
                            true
                        );
                    }
                }
            }
        }




        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin-manage-patient.aspx");
        }
    }
}
