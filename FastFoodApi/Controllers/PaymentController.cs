using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

[ApiController]
[Route("/")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "CreatePaymentIntent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] decimal amount)
    {
        var paymentIntent = await _paymentService.CreatePaymentIntentAsync(amount);

        return Ok(new { clientSecret = paymentIntent.ClientSecret }); 
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "ConfirmPayment")]
    public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmationRequest request)
    {
        var paymentIntent = await _paymentService.ConfirmPaymentAsync(request.PaymentIntentId, request.PaymentMethodId);

        if (paymentIntent.Status == "succeeded")
        {
            return Ok("Payment successful!");
        }

        return BadRequest("Payment failed");
    }
}

public class PaymentConfirmationRequest
{
    public string PaymentIntentId { get; set; }
    public string PaymentMethodId { get; set; }
}
