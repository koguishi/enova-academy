namespace enova_academy.Domain.Entities;

public class Course
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public Int32? Capacity { get; private set; }

    // Construtor vazio para EF Core
    private Course() { }

    public Course(
        string title,
        string slug,
        decimal price,
        Int32? capacity)
    {
        if (string.IsNullOrEmpty(title)) throw new Exception("Title is required");
        if (string.IsNullOrEmpty(slug)) throw new Exception("Slug is required");
        if (price <= 0) throw new Exception("Price must be greater then 0");
        Title = title;
        Slug = slug;
        Price = price;
        Capacity = (capacity.HasValue) ? capacity.Value : null;
        Id = Guid.NewGuid();
    }

    public void Atualizar(
        string? title,
        string? slug,
        decimal? price,
        Int32? capacity)
    {
        if (title != null) Title = title;
        if (slug != null) Slug = slug;
        if (price.HasValue) Price = price.Value;
        if (capacity.HasValue) Capacity = capacity.Value;
    }

    virtual public ICollection<Enrollment>? Enrollments { get; private set; } = [];

    public bool IsFull() => (Enrollments?.Count ?? 0) >= Capacity;

    public bool StudentFound(Guid studentId) => Enrollments!.Any(c => c.StudentId == studentId);

    public void Enroll(Enrollment enrollment)
    {
        if (IsFull())
            throw new InvalidOperationException("Domain Exception: Course capacity exceeded.");

        if (StudentFound(enrollment.StudentId))
            throw new InvalidOperationException("Domain Exception: Enrollment already taken for this Course");

        Enrollments!.Add(enrollment);
    }    
}
