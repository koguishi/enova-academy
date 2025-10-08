using enova_academy.Application.DTOs;
using enova_academy.Domain.Entities;
using enova_academy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace enova_academy.Application.Services;

public class EnrollmentService
{
    IEnrollmentRepository Enrollments { get; }
    ICourseRepository Courses { get; }
    public EnrollmentService(IEnrollmentRepository enrollments, ICourseRepository courses)
    {
        Enrollments = enrollments;
        Courses = courses;
    }

    public async Task<EnrollmentDto> CreateAsync(EnrollmentCreateDto createDto, Guid studentId)
    {
        var course = await Courses.GetByIdAsync(createDto.CourseId, true)
            ?? throw new Exception("Course not found");

        var enrollment = new Enrollment(studentId, createDto.CourseId);
        course.Enroll(enrollment);

        await Enrollments.AddAsync(enrollment);
        await Enrollments.SaveChangesAsync();

        return ToEnrollmentDto(enrollment);
    }

    public async Task DeleteAsync(Guid id, Guid userId, bool isAdmin)
    {
        var enrollment = await Enrollments.GetByIdAsync(id)
            ?? throw new Exception("Enrollment not found");

        if (enrollment.StudentId != userId && !isAdmin)
            throw new UnauthorizedAccessException("You cannot delete this enrollment");

        if (enrollment.Status != "pending_payment")
            throw new InvalidOperationException("Enrollment's status must be pending_payment");

        await Enrollments.DeleteAsync(enrollment);
    }

    public async Task<EnrollmentDto?> ReadAsync(Guid id)
    {
        var enrollment = await Enrollments.GetByIdAsync(id);
        return ToEnrollmentDto(enrollment);
    }

    public async Task<(List<EnrollmentDto> Enrollments, int Total)> ListarEnrollmentsPagAsync(
        int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = Enrollments.Query();

        var total = await query.CountAsync();
        var enrollments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ToEnrollmentsDto(enrollments), total);
    }

    private static List<EnrollmentDto> ToEnrollmentsDto(List<Enrollment> enrollments)
    {
        var listDto = new List<EnrollmentDto>();
        enrollments.ForEach(course =>
        {
            listDto.Add(ToEnrollmentDto(course));
        });
        return listDto;
    }

    private static EnrollmentDto ToEnrollmentDto(Enrollment? enrollment)
    {
        if (enrollment == null) return null!;
        return new EnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            Status = enrollment.Status,
        };
    }

    public async Task<List<EnrollmentDto>> GetByStudentAsync(Guid requestedStudentId, string currentUserId, bool isAdmin)
    {
        if (!isAdmin && requestedStudentId.ToString() != currentUserId)
            throw new UnauthorizedAccessException("You can only view your own enrollments.");

        var enrollments = await Enrollments.GetByStudentIdAsync(requestedStudentId);

        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            CourseId = e.CourseId,
            StudentId = e.StudentId,
            Status = e.Status
        }).ToList();
    }

    public async Task AtualizarMetricasAsync()
    {
        var totalPorCurso = Metrics.CreateGauge(
            "dominio_matriculas_total",
            "Quantidade total de matrículas por curso",
            new GaugeConfiguration { LabelNames = ["curso"] }
        );

        var percentualPagoPorCurso = Metrics.CreateGauge(
            "dominio_matriculas_percentual_pago",
            "% de matrículas pagas por curso",
            new GaugeConfiguration { LabelNames = ["curso"] }
        );

        var agrupado = await (
            from e in Enrollments.Query()
            join c in Courses.Query() on e.CourseId equals c.Id
            group e by new { e.CourseId, c.Title } into g
            select new
            {
                CourseId = g.Key.CourseId,
                CourseName = g.Key.Title,
                Total = g.Count(),
                Pagos = g.Count(x => x.Status == "paid"),
                PercentualPagos = g.Count(x => x.Status == "paid") * 100.0 / g.Count()
            }
        ).ToListAsync();

        foreach (var grupo in agrupado)
        {
            totalPorCurso.WithLabels(grupo.CourseName).Set(grupo.Total);
            percentualPagoPorCurso.WithLabels(grupo.CourseName).Set(grupo.PercentualPagos);
        }
    }
}
