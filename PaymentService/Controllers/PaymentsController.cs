using Microsoft.AspNetCore.Mvc;
using PaymentService.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentRequest request)
    {
        var payment = await _paymentService.ProcessPaymentAsync(request.OrderId, request.Amount);
        return Ok(payment);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> Get(Guid orderId)
    {
        var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
        if (payment == null) return NotFound();
        return Ok(payment);
    }
}

public class PaymentRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}