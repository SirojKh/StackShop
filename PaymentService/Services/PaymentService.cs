using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;

namespace PaymentService.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentDbContext _context;

    public PaymentService(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> ProcessPaymentAsync(Guid orderId, decimal amount)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            Status = PaymentStatus.Pending
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Simulera delay + resultat
        await Task.Delay(1000);
        payment.Status = PaymentStatus.Succeeded;
        await _context.SaveChangesAsync();

        // TODO: Skicka RabbitMQ-event "PaymentSucceeded"

        return payment;
    }

    public Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId)
    {
        return _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    }
}