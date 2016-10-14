﻿namespace TrueMoney.Infrastructure.Services
{
    using System.Threading.Tasks;

    using TrueMoney.Infrastructure.Entities;

    public interface IPaymentService
    {
        Task<PaymentResult> LendMoney(User user, int loanId, int payForId, float count, VisaDetails visaDetails);
        //Task<PaymentResult> PayLoanPart(int dealId, int payerId, float count);
    }
}