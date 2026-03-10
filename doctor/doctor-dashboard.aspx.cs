// doctor-dashboard.aspx.cs
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_Assignment
{
    public partial class doctor_dashboard : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                if (Session["DoctorID"] == null)
                {
                    Response.Redirect("../index.aspx");
                    return;
                }

                UpdatePastAppointmentsStatus();
                LoadSummaryCards();
                LoadPendingAppointments();
                LoadSchedule();
                LoadUpcomingAppointments();
                LoadTodayAppointments();

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


        private void LoadSummaryCards()
        {
            string doctorId = Session["DoctorID"].ToString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Appointments WHERE DoctorID = @DoctorID", con);
                cmd1.Parameters.AddWithValue("@DoctorID", doctorId);
                lblTotalAppointments.Text = cmd1.ExecuteScalar().ToString();

                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Appointments WHERE DoctorID = @DoctorID AND Status = 'Pending'", con);
                cmd2.Parameters.AddWithValue("@DoctorID", doctorId);
                lblPendingApprovals.Text = cmd2.ExecuteScalar().ToString();

                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(DISTINCT PatientID) FROM Appointments WHERE DoctorID = @DoctorID", con);
                cmd3.Parameters.AddWithValue("@DoctorID", doctorId);
                lblTotalPatients.Text = cmd3.ExecuteScalar().ToString();
            }
        }

        private void LoadPendingAppointments()
        {
            string doctorId = Session["DoctorID"].ToString();
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
    SELECT a.AppointmentID, a.AppointmentDate, a.StartTime, p.FullName AS PatientName
    FROM Appointments a
    INNER JOIN Patients p ON a.PatientID = p.PatientID
    WHERE a.DoctorID = @DoctorID AND a.Status = 'Pending'
    ORDER BY a.AppointmentDate ASC";


                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            rptPendingAppointments.DataSource = dt;
            rptPendingAppointments.DataBind();

            rptPendingAppointments.Visible = dt.Rows.Count > 0;
            lblNoPending.Visible = dt.Rows.Count == 0;
        }

        private void LoadSchedule()
        {
            string doctorId = Session["DoctorID"].ToString();
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT date, available_start_time, available_end_time
            FROM Schedule
            WHERE DoctorID = @DoctorID
              AND date BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, 7, CAST(GETDATE() AS DATE))
            ORDER BY date, available_start_time";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            // Group by date and merge times into a single string
            var grouped = dt.AsEnumerable()
                .GroupBy(row => row.Field<DateTime>("date"))
                .Select(g =>
                {
                    string date = g.Key.ToString("yyyy-MM-dd");
                    string displayDate = g.Key.ToString("dddd, dd MMM yyyy");
                    string timeSlots = string.Join(", ",
                        g.Select(r => $"{r["available_start_time"]:hh\\:mm} to {r["available_end_time"]:hh\\:mm}"));

                    return new
                    {
                        Date = displayDate,
                        TimeSlots = timeSlots
                    };
                }).ToList();

            rptSchedule.DataSource = grouped;
            rptSchedule.DataBind();
        }


        private void LoadUpcomingAppointments()
        {
            string doctorId = Session["DoctorID"].ToString();
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT a.AppointmentID, p.FullName AS PatientName, a.AppointmentDate, a.StartTime, a.Status, a.TreatmentType
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientID = p.PatientID
            WHERE a.DoctorID = @DoctorID 
              AND a.AppointmentDate >= DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
            ORDER BY a.AppointmentDate, a.StartTime";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            gvUpcomingAppointments.DataSource = dt;
            gvUpcomingAppointments.DataBind();
        }


        protected void rptPendingAppointments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string appointmentId = e.CommandArgument.ToString();
            string newStatus = e.CommandName == "Completed" ? "Completed" : "Missed";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Appointments SET Status = @Status WHERE AppointmentID = @AppointmentID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadPendingAppointments();
            LoadSummaryCards();
            LoadUpcomingAppointments();
        }

        private void LoadTodayAppointments()
        {
            string doctorId = Session["DoctorID"].ToString();
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT a.AppointmentID, a.AppointmentDate, a.StartTime, a.Status, a.TreatmentType,
                   p.FullName AS PatientName
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientID = p.PatientID
            WHERE a.DoctorID = @DoctorID AND CAST(a.AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)
            ORDER BY a.StartTime";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            gvTodayAppointments.DataSource = dt;
            gvTodayAppointments.DataBind();
        }

    }
}
