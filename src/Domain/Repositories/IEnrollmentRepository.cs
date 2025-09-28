using enova_academy.Domain.Entities;

namespace enova_academy.Domain.Repositories;

public interface IEnrollmentRepository
{
    Task AddAsync(Enrollment enrollment);
    Task<Enrollment?> GetByIdAsync(Guid id);
    IQueryable<Enrollment> Query();
    Task DeleteAsync(Enrollment enrollment);
    Task SaveChangesAsync();
    Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId);
}
