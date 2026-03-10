using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace patientsystem
{
    public partial class PatientMain : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                if (Session["PatientID"] == null)
                {
                    Response.Redirect("../Login.aspx");
                    return;
                }

                string patientId = Session["PatientID"].ToString();

                try
                {
                    LoadPatientData(patientId);
                    LoadNextAppointment(patientId);
                    UpdatePastAppointmentsStatus();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void UpdatePastAppointmentsStatus()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString))
            {
                string query = @"
            UPDATE Appointments
            SET Status = 'Pending'
            WHERE Status = 'Scheduled'
              AND DATEADD(MINUTE, DATEDIFF(MINUTE, 0, StartTime), CAST(AppointmentDate AS DATETIME)) < GETDATE()";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        private void LoadPatientData(string patientId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT FullName, Phone, DOB, Gender, Email, Profile_Pic 
                                 FROM Patients 
                                 WHERE PatientID = @PatientID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        patientName.Text = reader["FullName"].ToString();
                        phoneNumber.Text = reader["Phone"].ToString();
                        DOB.Text = Convert.ToDateTime(reader["DOB"]).ToString("dd-MM-yyyy");
                        gender.Text = reader["Gender"].ToString();
                        email.Text = reader["Email"].ToString();

                        string picPath = reader["Profile_Pic"].ToString();
                        if (!string.IsNullOrEmpty(picPath))
                        {
                            if (picPath.StartsWith("~/")) picPath = picPath.Substring(2);
                            Image1.ImageUrl = "~/" + picPath;
                        }
                        else
                        {
                            Image1.ImageUrl = "~/Uploads/default-profile.png";
                        }
                    }
                }
            }
        }

        private void LoadNextAppointment(string patientId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT TOP 1 A.AppointmentDate, A.StartTime, D.FullName AS DoctorName
            FROM Appointments A
            JOIN Doctors D ON A.DoctorID = D.DoctorID
            WHERE A.PatientID = @PatientID AND A.AppointmentDate >= CAST(GETDATE() AS DATE) AND A.Status = 'Scheduled'
            ORDER BY A.AppointmentDate ASC, A.StartTime ASC";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        nextDoctorName.Text = reader["DoctorName"].ToString();
                        nextAptDate.Text = Convert.ToDateTime(reader["AppointmentDate"]).ToString("yyyy-MM-dd");
                        nextAptTime.Text = TimeSpan.Parse(reader["StartTime"].ToString()).ToString(@"hh\:mm");

                        appointmentCard.Visible = true;
                        countdown.Attributes["data-has-appointment"] = "true";
                    }
                    else
                    {
                        appointmentCard.Visible = false; // 👈 Hides the entire card!
                        countdown.Attributes["data-has-appointment"] = "false";
                    }
                }
            }
        }


        protected void btnBookAppointment_Click(object sender, EventArgs e)
        {
            Response.Redirect("BookAppointment.aspx");
        }

        protected void btnCancelAppointment_Click(object sender, EventArgs e)
        {
            Response.Redirect("CancelAppointment.aspx");
        }
    }
}
