using enova_academy.Domain.Entities;

namespace enova_academy.Domain.Repositories;

public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task<Course?> GetByIdAsync(Guid id);
    Task<Course?> GetBySlugAsync(string slug);
    Task<List<Course>> GetAllAsync();
    IQueryable<Course> Query();
    Task DeleteAsync(Course course);
    Task SaveChangesAsync();
}
