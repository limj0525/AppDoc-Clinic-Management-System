using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace patientsystem
{
    public partial class BookAppointment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                // Restrict calendar minimum date
                txtDate.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");

                LoadTreatmentTypes();
                ddlDoctors.Items.Clear();
                ddlDoctors.Items.Add(new ListItem("Select Doctor", ""));
            }
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
                ddlTreatment.Items.Add(new ListItem("Select Treatment Type", ""));

                while (reader.Read())
                {
                    ddlTreatment.Items.Add(new ListItem(reader["Specialization"].ToString(), reader["Specialization"].ToString()));
                }
                reader.Close();
            }
        }

        protected void ddlTreatment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDoctors();
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

        protected void ddlDoctors_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableTimeSlots();
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            LoadAvailableTimeSlots();
        }

        private void LoadAvailableTimeSlots()
        {
            string doctorId = ddlDoctors.SelectedValue;
            DateTime selectedDate;

            pnlMessage.Visible = false; // Hide message panel by default
            rptTimeSlots.Visible = false; // Hide the Repeater by default

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

                // Step 1: Get schedules for the doctor on the selected date
                string scheduleQuery = "SELECT available_start_time, available_end_time FROM Schedule WHERE DoctorID = @DoctorID AND [date] = @Date";
                SqlCommand scheduleCmd = new SqlCommand(scheduleQuery, conn);
                scheduleCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                scheduleCmd.Parameters.AddWithValue("@Date", selectedDate.Date);

                SqlDataReader scheduleReader = scheduleCmd.ExecuteReader();
                while (scheduleReader.Read())
                {
                    TimeSpan start = (TimeSpan)scheduleReader["available_start_time"];
                    TimeSpan end = (TimeSpan)scheduleReader["available_end_time"];
                    scheduleSlots.Add((start, end));
                }
                scheduleReader.Close();

                if (scheduleSlots.Count == 0)
                {
                    pnlMessage.Visible = true;
                    lblMessage.Text = "No available time slots for the selected doctor and date. Please choose another date or doctor.";
                    return;
                }

                // Step 2: Get existing booked appointments
                string bookedQuery = "SELECT StartTime FROM Appointments WHERE DoctorID = @DoctorID AND AppointmentDate = @Date AND Status = 'Scheduled'";
                SqlCommand bookedCmd = new SqlCommand(bookedQuery, conn);
                bookedCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                bookedCmd.Parameters.AddWithValue("@Date", selectedDate.Date);

                SqlDataReader bookedReader = bookedCmd.ExecuteReader();
                while (bookedReader.Read())
                {
                    bookedTimes.Add((TimeSpan)bookedReader["StartTime"]);
                }
                bookedReader.Close();
            }

            // Step 3: Create 1-hour slots, exclude booked and past time slots for today
            DateTime now = DateTime.Now;
            bool isToday = selectedDate.Date == now.Date;

            foreach (var slot in scheduleSlots)
            {
                TimeSpan current = slot.Start;
                while (current.Add(TimeSpan.FromHours(1)) <= slot.End)
                {
                    bool isPastTime = isToday && current <= now.TimeOfDay;

                    if (!bookedTimes.Contains(current) && !isPastTime)
                    {
                        availableTimeSlots.Add(current);
                    }
                    current = current.Add(TimeSpan.FromHours(1));
                }
            }



            // Step 4: Bind to Repeater or show message if no available slots
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
                lblMessage.Text = "No available time slots for the selected doctor and date. Please choose another date or doctor.";
            }

            rptTimeSlots.DataBind();
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            string selectedTime = txtTime.Text;

            if (ddlDoctors.SelectedValue == "" || txtDate.Text == "" || string.IsNullOrEmpty(selectedTime))
            {
                pnlMessage.Visible = true;
                lblMessage.Text = "Please select all fields before booking.";
                return;
            }

            string patientId = Session["PatientID"].ToString();
            DateTime appointmentDate = DateTime.Parse(txtDate.Text);
            TimeSpan startTime = TimeSpan.Parse(selectedTime);

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // CHECK if patient already has an appointment at the same date and time
                string checkQuery = "SELECT COUNT(*) FROM Appointments " +
                                    "WHERE PatientID = @PatientID AND AppointmentDate = @AppointmentDate AND StartTime = @StartTime AND Status = 'Scheduled'";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@PatientID", patientId);
                checkCmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);
                checkCmd.Parameters.AddWithValue("@StartTime", startTime);

                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    pnlMessage.Visible = true;
                    lblMessage.Text = "You already have an appointment booked at this time.";
                    return;
                }

                // Proceed to insert the new appointment
                string newAppointmentID = GenerateNewAppointmentID();
                DateTime appointmentDateTime = appointmentDate.Add(startTime);
                DateTime endTime = appointmentDateTime.AddHours(1);

                string query = "INSERT INTO Appointments (AppointmentID, PatientID, DoctorID, AppointmentDate, StartTime, EndTime, Status, TreatmentType, Duration) " +
                               "VALUES (@AppointmentID, @PatientID, @DoctorID, @AppointmentDate, @StartTime, @EndTime, 'Scheduled', @TreatmentType, @Duration)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AppointmentID", newAppointmentID);
                cmd.Parameters.AddWithValue("@PatientID", patientId);
                cmd.Parameters.AddWithValue("@DoctorID", ddlDoctors.SelectedValue);
                cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime.TimeOfDay);
                cmd.Parameters.AddWithValue("@TreatmentType", ddlTreatment.SelectedValue);
                cmd.Parameters.AddWithValue("@Duration", 60);

                cmd.ExecuteNonQuery();
            }

            pnlMessage.Visible = true;
            lblMessage.Text = "Appointment successfully booked!";
        }


        private string GenerateNewAppointmentID()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            string lastAppointmentID = string.Empty;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT TOP 1 AppointmentID FROM Appointments ORDER BY AppointmentID DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lastAppointmentID = reader["AppointmentID"].ToString();
                }
                reader.Close();
            }

            if (string.IsNullOrEmpty(lastAppointmentID))
            {
                // No previous appointments found, so start with B001
                return "B001";
            }
            else
            {
                // Extract the numeric part of the last appointment ID (after "B")
                string numericPart = lastAppointmentID.Substring(1); // Remove "B"
                int number;

                if (int.TryParse(numericPart, out number))
                {
                    return "B" + (number + 1).ToString("D3");
                }
                else
                {
                    return "B001";
                }
            }
        }
    }
}
