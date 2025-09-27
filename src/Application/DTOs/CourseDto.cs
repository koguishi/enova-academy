namespace enova_academy.Application.DTOs;

public class CourseDto
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public decimal? Price { get; set; }
    public Int32? Capacity { get; set; }
}
