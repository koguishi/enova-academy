using enova_academy.Data;
using Microsoft.AspNetCore.Mvc;

namespace enova_academy.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WebhooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("payment")]
    public async Task<IActionResult> Payment([FromBody] PaymentRequest request)
    {
        var enrollment = await _context.Enrollments.FindAsync(request.EnrollmentId);
        if (enrollment is null) return NotFound(new { error = "Enrollment not found" } );

        // Atualiza status conforme o request
        enrollment.SetStatus(request.Status);
        await _context.SaveChangesAsync();

        // Publica welcome_email se pago
        if (request.Status == "paid")
        {
            Console.WriteLine($@" >>> [WELCOME_EMAIL - via webhook] to enrollment: {enrollment.Id} <<< ");
        }

        return Ok(new { message = "Webhook processed" });
    }
}

public class PaymentRequest
{
    public Guid EnrollmentId { get; set; }
    public string Status { get; set; } = default!;
}
