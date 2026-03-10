using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment
{
    public partial class admin_edit_appointment : System.Web.UI.Page
    {
        protected string originalDoctorId = "";
        protected DateTime originalDate;
        protected TimeSpan originalStartTime;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblNotice.Text = "Note: All appointments are scheduled for 1 hour.";
                LoadTreatmentTypes();  // 1. Load treatments FIRST
                LoadAppointmentDetails(); // 2. Then appointment details
            }
        }

        private void LoadAppointmentDetails()
        {
            string appointmentId = Request.QueryString["AppointmentID"];

            if (string.IsNullOrEmpty(appointmentId))
            {
                Response.Redirect("admin-manage-appointments.aspx");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT a.*, d.Specialization
                         FROM Appointments a
                         INNER JOIN Doctors d ON a.DoctorID = d.DoctorID
                         WHERE AppointmentID = @AppointmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtAppointmentID.Text = reader["AppointmentID"].ToString();
                    txtPatientID.Text = reader["PatientID"].ToString();
                    ddlTreatment.SelectedValue = reader["TreatmentType"].ToString();
                    txtDate.Text = Convert.ToDateTime(reader["AppointmentDate"]).ToString("yyyy-MM-dd");
                    txtTime.Text = TimeSpan.Parse(reader["StartTime"].ToString()).ToString(@"hh\:mm");

                    originalDoctorId = reader["DoctorID"].ToString();
                    originalDate = Convert.ToDateTime(reader["AppointmentDate"]);
                    originalStartTime = TimeSpan.Parse(reader["StartTime"].ToString());
                }
                reader.Close();
            }

            LoadDoctors(); // Reload doctors after treatment is set
            ddlDoctors.SelectedValue = originalDoctorId; // THEN set doctor
            LoadAvailableTimeSlots(); // THEN load slots after dropdown is correct
        }

        private void LoadTreatmentTypes()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT DISTINCT Specialization FROM Doctors";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlTreatment.Items.Clear();
                ddlTreatment.Items.Add(new System.Web.UI.WebControls.ListItem("Select Treatment Type", ""));

                while (reader.Read())
                {
                    ddlTreatment.Items.Add(new System.Web.UI.WebControls.ListItem(reader["Specialization"].ToString(), reader["Specialization"].ToString()));
                }
                reader.Close();
            }
        }

        protected void ddlTreatment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDoctors();
            LoadAvailableTimeSlots();
        }

        protected void ddlDoctors_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableTimeSlots();
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            LoadAvailableTimeSlots();
        }

        private void LoadDoctors()
        {
            string treatmentType = ddlTreatment.SelectedValue;
            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = (treatmentType == "General")
                    ? "SELECT DoctorID, FullName, Specialization FROM Doctors WHERE Status = 'Active'"
                    : "SELECT DoctorID, FullName, Specialization FROM Doctors WHERE Specialization = @Specialization AND Status = 'Active'";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (treatmentType != "General")
                {
                    cmd.Parameters.AddWithValue("@Specialization", treatmentType);
                }

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlDoctors.Items.Clear();
                ddlDoctors.Items.Add(new ListItem("Select Doctor", ""));

                while (reader.Read())
                {
                    string doctorInfo = $"{reader["FullName"]} - {reader["Specialization"]}";
                    ddlDoctors.Items.Add(new ListItem(doctorInfo, reader["DoctorID"].ToString()));
                }
                reader.Close();
            }
        }

        private void LoadAvailableTimeSlots()
        {
            string doctorId = ddlDoctors.SelectedValue;
            DateTime selectedDate;

            rptTimeSlots.Visible = false;
            pnlMessage.Visible = false;

            if (string.IsNullOrEmpty(doctorId))
            {
                rptTimeSlots.DataSource = null;
                rptTimeSlots.DataBind();
                pnlMessage.Visible = true;
                lblMessage.Text = "Please select a doctor.";
                return;
            }

            if (!DateTime.TryParse(txtDate.Text, out selectedDate))
            {
                rptTimeSlots.DataSource = null;
                rptTimeSlots.DataBind();
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            List<TimeSpan> availableTimeSlots = new List<TimeSpan>();
            List<TimeSpan> bookedTimes = new List<TimeSpan>();
            List<(TimeSpan Start, TimeSpan End)> scheduleSlots = new List<(TimeSpan, TimeSpan)>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string scheduleQuery = "SELECT available_start_time, available_end_time FROM Schedule WHERE DoctorID = @DoctorID AND [date] = @Date";
                SqlCommand scheduleCmd = new SqlCommand(scheduleQuery, conn);
                scheduleCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                scheduleCmd.Parameters.AddWithValue("@Date", selectedDate.Date);

                SqlDataReader scheduleReader = scheduleCmd.ExecuteReader();
                while (scheduleReader.Read())
                {
                    scheduleSlots.Add(((TimeSpan)scheduleReader["available_start_time"], (TimeSpan)scheduleReader["available_end_time"]));
                }
                scheduleReader.Close();

                if (scheduleSlots.Count == 0)
                {
                    pnlMessage.Visible = true;
                    lblMessage.Text = "No available time slots for this doctor on the selected date.";
                    return;
                }

                string bookedQuery = @"SELECT StartTime FROM Appointments 
                                        WHERE DoctorID = @DoctorID 
                                        AND AppointmentDate = @Date 
                                        AND Status = 'Scheduled'
                                        AND AppointmentID != @AppointmentID";
                SqlCommand bookedCmd = new SqlCommand(bookedQuery, conn);
                bookedCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                bookedCmd.Parameters.AddWithValue("@Date", selectedDate.Date);
                bookedCmd.Parameters.AddWithValue("@AppointmentID", txtAppointmentID.Text);

                SqlDataReader bookedReader = bookedCmd.ExecuteReader();
                while (bookedReader.Read())
                {
                    bookedTimes.Add((TimeSpan)bookedReader["StartTime"]);
                }
                bookedReader.Close();
            }

            DateTime now = DateTime.Now;
            bool isToday = selectedDate.Date == now.Date;

            foreach (var slot in scheduleSlots)
            {
                TimeSpan current = slot.Start;
                while (current.Add(TimeSpan.FromHours(1)) <= slot.End)
                {
                    bool isPastTime = isToday && current <= now.TimeOfDay;
                    bool isBooked = bookedTimes.Any(bt => Math.Abs((bt - current).TotalMinutes) < 1);

                    if (!isBooked && !isPastTime)
                    {
                        availableTimeSlots.Add(current);
                    }
                    current = current.Add(TimeSpan.FromHours(1));
                }
            }

            if (availableTimeSlots.Count > 0)
            {
                rptTimeSlots.DataSource = availableTimeSlots.Select(t => new { TimeSlot = t.ToString(@"hh\:mm") }).ToList();
                rptTimeSlots.Visible = true;
                pnlMessage.Visible = false;
            }
            else
            {
                rptTimeSlots.DataSource = null;
                rptTimeSlots.Visible = false;
                pnlMessage.Visible = true;
                lblMessage.Text = "No available time slots.";
            }
            rptTimeSlots.DataBind();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (ddlDoctors.SelectedValue == "" || txtDate.Text == "")
            {
                pnlMessage.Visible = true;
                lblMessage.Text = "Please complete all fields before updating.";
                return;
            }

            // 1. Get selected time from the Repeater
            string selectedTime = null;

            foreach (RepeaterItem item in rptTimeSlots.Items)
            {
                var rb = item.FindControl("rbTimeSlot") as System.Web.UI.WebControls.RadioButton;
                if (rb != null && rb.Checked)
                {
                    selectedTime = rb.Text;
                    break;
                }
            }

            if (string.IsNullOrEmpty(selectedTime))
            {
                pnlMessage.Visible = true;
                lblMessage.Text = "Please select a time slot.";
                return;
            }

            // 2. Parse values
            string appointmentId = txtAppointmentID.Text;
            string doctorId = ddlDoctors.SelectedValue;
            string treatmentType = ddlTreatment.SelectedValue;
            DateTime appointmentDate = DateTime.Parse(txtDate.Text);
            TimeSpan startTime = TimeSpan.Parse(selectedTime);

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 3. Check for conflict
                string checkQuery = @"SELECT COUNT(*) FROM Appointments 
                                WHERE DoctorID = @DoctorID 
                                AND AppointmentDate = @AppointmentDate 
                                AND StartTime = @StartTime 
                                AND Status = 'Scheduled'
                                AND AppointmentID != @AppointmentID";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                checkCmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);
                checkCmd.Parameters.AddWithValue("@StartTime", startTime);
                checkCmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                int conflict = (int)checkCmd.ExecuteScalar();
                if (conflict > 0)
                {
                    pnlMessage.Visible = true;
                    lblMessage.Text = "Selected timeslot is already booked.";
                    return;
                }

                // 4. Update the appointment
                string updateQuery = @"UPDATE Appointments
                               SET DoctorID = @DoctorID,
                                   AppointmentDate = @AppointmentDate,
                                   StartTime = @StartTime,
                                   EndTime = @EndTime,
                                   TreatmentType = @TreatmentType
                               WHERE AppointmentID = @AppointmentID";

                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", startTime.Add(TimeSpan.FromHours(1)));
                cmd.Parameters.AddWithValue("@TreatmentType", treatmentType);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                cmd.ExecuteNonQuery();
            }

            pnlMessage.Visible = true;
            pnlMessage.CssClass = "alert-success";
            lblMessage.Text = "Appointment updated successfully! Redirecting...";
            ScriptManager.RegisterStartupScript(this, GetType(), "redirect", "setTimeout(function(){ window.location='admin-manage-appointments.aspx'; }, 500);", true);
        }



        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin-manage-appointments.aspx");
        }
    }
}
