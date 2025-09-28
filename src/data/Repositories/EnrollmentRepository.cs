using enova_academy.Domain.Entities;
using enova_academy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace enova_academy.Data.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        var enrollment = await _context.Enrollments
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        return enrollment;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Enrollment enrollment)
    {
        _context.Enrollments.Remove(enrollment);
        return _context.SaveChangesAsync();
    }

    IQueryable<Enrollment> IEnrollmentRepository.Query()
    {
        return _context.Enrollments.AsQueryable();
    }

    public async Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .ToListAsync();
    }

}
