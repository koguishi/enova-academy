namespace enova_academy.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Status { get; private set; } = "pending_payment";

    // Construtor vazio para EF Core
    public Enrollment() { }

    public Enrollment(
        Guid studentId,
        Guid courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
        Id = Guid.NewGuid();
    }
}
