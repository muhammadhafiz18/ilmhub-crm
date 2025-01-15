namespace Ilmhub.Spaces.Client.Models;

public class Course
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationInWeeks { get; set; }
    public string? Instructor { get; set; }
    public DateTime StartDate { get; set; }
    public bool IsActive { get; set; }
}