using enova_academy.Domain.Entities;
using enova_academy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace enova_academy.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        var course = await _context.Courses
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        return course;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        return _context.SaveChangesAsync();
    }

    IQueryable<Course> Query()
    {
        return _context.Courses.AsQueryable();
    }

    public Task<List<Course>> GetAllAsync()
    {
        return _context.Courses.ToListAsync();
    }

    IQueryable<Course> ICourseRepository.Query()
    {
        return Query();
    }
}
