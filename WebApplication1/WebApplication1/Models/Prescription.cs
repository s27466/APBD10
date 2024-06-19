namespace WebApplication1.Models;

public class Prescription
{
    public int Id { get; set; }
    public string Medication { get; set; }
    public DateTime Date { get; set; }
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
}