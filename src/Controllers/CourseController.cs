using enova_academy.Application.DTOs;
using enova_academy.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace enova_academy.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CoursesController(CourseService service) : ControllerBase
{

    private readonly CourseService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchString = null,
        [FromQuery] decimal? min_price = null,
        [FromQuery] decimal? max_price = null)
    {
        var (courses, total) = await _service.ListarCoursesPagAsync(page, pageSize
            , searchString, min_price, max_price);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return Ok(new
        {
            totalItems = total,
            totalPages,
            currentPage = page,
            pageSize,
            courses
        });
            
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<CourseDto>> GetById(Guid id)
    {
        var course = await _service.ReadAsync(id);
        if (course is null) return NotFound();
        return Ok(course);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CourseDto>> GetBySlug(string slug)
    {
        var course = await _service.ReadAsync(slug);
        if (course is null) return NotFound();
        return Ok(course);
    }

    [Authorize(Roles = "ADMIN")]    
    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CourseDto dto)
    {
        try
        {
            var course = await _service.CreateAsync(dto);
            dto.Id = course.Id;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CourseDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
                return NotFound(ex.Message);
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var course = await _service.ReadAsync(id);
            if (course is null || !course.Id.HasValue)
                return NotFound();
            await _service.DeleteAsync(course.Id.Value);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }
}
