namespace enova_academy.Application.DTOs;

public class EnrollmentDto
{
    public Guid? Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string? Status { get; set; }
}

public class EnrollmentCreateDto
{
    public Guid CourseId { get; set; }
}
