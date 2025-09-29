using System.Security.Claims;
using enova_academy.Application.DTOs;
using enova_academy.Application.Services;
using enova_academy.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace enova_academy.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EnrollmentsController(EnrollmentService service, SqsService sqsService) : ControllerBase
{
    private readonly EnrollmentService _service = service;
    private readonly SqsService _sqsService = sqsService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (enrollments, total) = await _service.ListarEnrollmentsPagAsync(page, pageSize);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return Ok(new
        {
            totalItems = total,
            totalPages,
            currentPage = page,
            pageSize,
            enrollments
        });

    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<EnrollmentDto>> GetById(Guid id)
    {
        var enrollment = await _service.ReadAsync(id);
        if (enrollment is null) return NotFound();
        return Ok(enrollment);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Create([FromBody] EnrollmentCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var enrollment = await _service.CreateAsync(dto, Guid.Parse(userId!));

            // quando implementar o webhook ...
            // var useWebhook = config.GetValue<bool>("APP_USE_WEBHOOK");
            // if (!useWebhook)
            // {
            // }
            await _sqsService.SendMessageAsync(
                new PaymentRequestedEvent(enrollment!.Id!.Value));

            return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
        }
        catch (EnrollmentAlreadyTakenException ex)
        {
            return StatusCode(409, new { error = ex.Message });
        }
        catch (CourseCapacityExceededException ex)
        {
            return StatusCode(422, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("ADMIN");
        try
        {
            await _service.DeleteAsync(id, Guid.Parse(userId!), isAdmin);
        }
        catch (Exception ex)
        {
            return ex.Message.Contains("not found", StringComparison.CurrentCultureIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
        return NoContent();
    }

    [HttpGet("/students/{id:Guid}/enrollments")]
    public async Task<IActionResult> GetEnrollments(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var isAdmin = User.IsInRole("ADMIN");

        try
        {
            var result = await _service.GetByStudentAsync(id, userId, isAdmin);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
