using System.Text.Json;
using enova_academy.Application.DTOs;
using enova_academy.Domain.Entities;
using enova_academy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace enova_academy.Application.Services;

public class CourseService
{
    ICourseRepository Courses { get; }
    private readonly IDistributedCache _cache;
    private const string CoursesCacheKey = "all_courses";
    public CourseService(ICourseRepository courses, IDistributedCache cache)
    {
        Courses = courses;
        _cache = cache;
    }

    public async Task<CourseDto> CreateAsync(CourseDto dto)
    {
        var dbCourse = await Courses.GetBySlugAsync(dto.Slug!);
        if (dbCourse != null) throw new Exception("This Slug is already taken");

        var course = new Course(
            dto.Title!,
            dto.Slug!,
            dto.Price!.Value,
            dto.Capacity
        );
        await Courses.AddAsync(course);
        await Courses.SaveChangesAsync();

        // Invalida o cache
        await _cache.RemoveAsync(CoursesCacheKey);

        return ToCourseDto(course);
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await Courses.GetByIdAsync(id)
            ?? throw new Exception("Course not found");

        await Courses.DeleteAsync(course);

        // Invalida o cache
        await _cache.RemoveAsync(CoursesCacheKey);
    }

    public async Task<List<CourseDto>> ReadAllAsync()
    {
        // 1. Tenta buscar do cache
        var cached = await _cache.GetStringAsync(CoursesCacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<CourseDto>>(cached)!;
        }

        // 2. Se não tiver cache, busca do banco
        var courses = await Courses.GetAllAsync();

        // 3. Salva no cache com TTL (ex: 5 minutos)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        await _cache.SetStringAsync(CoursesCacheKey, JsonSerializer.Serialize(courses), cacheOptions);

        return ToCoursesDto(courses);
    }

    public async Task<CourseDto?> ReadAsync(Guid id)
    {
        var course = await Courses.GetByIdAsync(id);
        return ToCourseDto(course);
    }

    public async Task<CourseDto?> ReadAsync(string slug)
    {
        var course = await Courses.GetBySlugAsync(slug);
        return ToCourseDto(course);
    }

    public async Task UpdateAsync(Guid id, CourseDto dto)
    {
        var course = await Courses.GetByIdAsync(id)
            ?? throw new Exception("Course not found");

        course.Atualizar(dto.Title, dto.Slug, dto.Price, dto.Capacity);
        await Courses.SaveChangesAsync();
        // Invalida o cache
        await _cache.RemoveAsync(CoursesCacheKey);
    }

    public async Task<(List<CourseDto> Courses, int Total)> ListarCoursesPagAsync(
        int page = 1, int pageSize = 10,
        string? search = null, decimal? min_price = null, decimal? max_price = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = Courses.Query();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.Title.Contains(search) || a.Slug.Contains(search));
        if (min_price.HasValue)
            query = query.Where(a => a.Price >= min_price);
        if (max_price.HasValue)
            query = query.Where(a => a.Price <= max_price);

        var total = await query.CountAsync();
        var courses = await query
            .OrderBy(a => a.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ToCoursesDto(courses), total);
    }

    private static List<CourseDto> ToCoursesDto(List<Course> courses)
    {
        var listDto = new List<CourseDto>();
        courses.ForEach(course =>
        {
            listDto.Add(ToCourseDto(course));
        });
        return listDto;
    }

    private static CourseDto ToCourseDto(Course? course)
    {
        if (course == null) return null!;
        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Slug = course.Slug,
            Price = course.Price,
            Capacity = course.Capacity,
        };
    }
}
