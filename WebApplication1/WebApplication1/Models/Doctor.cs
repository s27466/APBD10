namespace WebApplication1.Models;

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Specialty { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; }
}