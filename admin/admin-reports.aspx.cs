using System;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace Assignment
{
    public partial class admin_reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblCurrentDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            UpdatePastAppointmentsStatus();
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


        protected void btnReport1_Click(object sender, EventArgs e)
        {
            ExportGridToPDF(gvAppointmentSummary, "AppointmentSummaryReport.pdf");
        }


        protected void btnDoctorReport_Click(object sender, EventArgs e)
        {
            ExportGridToPDF(gvDoctorReport, "DoctorReport.pdf");
        }

        protected void btnPatientReport_Click(object sender, EventArgs e)
        {
            ExportGridToPDF(gvPatientReport, "PatientReport.pdf");
        }


        private void ExportGridToPDF(GridView grid, string filename)
        {
            try
            {
                grid.AllowPaging = false;
                grid.DataBind();

                Document pdfDoc = new Document(PageSize.A4);
                MemoryStream memoryStream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);

                pdfDoc.Open();

                Font font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Paragraph title = new Paragraph("Report", font);
                title.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(title);
                pdfDoc.Add(new Chunk("\n"));

                PdfPTable pdfTable = new PdfPTable(grid.Columns.Count);
                pdfTable.WidthPercentage = 100;

                foreach (TableCell cell in grid.HeaderRow.Cells)
                {
                    PdfPCell pdfCell = new PdfPCell(new Phrase(cell.Text));
                    pdfCell.BackgroundColor = new BaseColor(204, 204, 204);
                    pdfTable.AddCell(pdfCell);
                }

                foreach (GridViewRow row in grid.Rows)
                {
                    foreach (TableCell cell in row.Cells)
                    {
                        PdfPCell pdfCell = new PdfPCell(new Phrase(cell.Text));
                        pdfTable.AddCell(pdfCell);
                    }
                }

                pdfDoc.Add(pdfTable);
                pdfDoc.Close();

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename={filename}");
                Response.BinaryWrite(memoryStream.ToArray());
                Response.End();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "pdfError",
                    $@"Swal.fire({{
            icon: 'error',
            title: 'PDF Error',
            text: 'Error generating PDF: {ex.Message}'
        }});",
                    true
                );
            }

        }
    }
}