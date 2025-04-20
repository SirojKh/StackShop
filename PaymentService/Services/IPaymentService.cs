using System;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services;

public interface IPaymentService
{
    Task<Payment> ProcessPaymentAsync(Guid orderId, decimal amount);
    Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId);
}