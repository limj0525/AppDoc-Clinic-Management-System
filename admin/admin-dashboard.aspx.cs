using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Assignment
{
    public partial class admin_dashboard : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                GetRegisteredDoctor();
                GetTotalAppointments();
                GetRegisteredPatients();
                LoadUpcomingAppointments();
                UpdatePastAppointmentsStatus();
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


        private void GetRegisteredDoctor()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Doctors";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    int count = (int)cmd.ExecuteScalar();
                    lblRegisteredDoctor.Text = count.ToString();
                }
            }
        }

        private void GetTotalAppointments()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Appointments";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    int count = (int)cmd.ExecuteScalar();
                    lblTotalAppointment.Text = count.ToString();
                }
            }
        }

        private void GetRegisteredPatients()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Patients";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    int count = (int)cmd.ExecuteScalar();
                    lblRegisteredPatient.Text = count.ToString();
                }
            }
        }

        // ✅ Fixed Method to Load Upcoming Appointments for the Next 7 Days (DATE-only comparison)
        private void LoadUpcomingAppointments()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT 
                p.FullName AS PatientName,
                a.AppointmentDate,
                d.FullName AS DoctorName,
                a.Status
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientID = p.PatientID
            INNER JOIN Doctors d ON a.DoctorID = d.DoctorID
            WHERE 
                CAST(a.AppointmentDate AS DATE) BETWEEN CAST(GETDATE() AS DATE) AND CAST(DATEADD(DAY, 7, GETDATE()) AS DATE)
                AND a.Status = 'Scheduled'
            ORDER BY a.AppointmentDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvUpcomingAppointments.DataSource = dt;
                    gvUpcomingAppointments.DataBind();
                }
            }
        }

    }
}
