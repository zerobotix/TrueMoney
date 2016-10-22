﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueMoney.Data.Entities
{
    public class BankTransaction : Entity
    {
        public int PaymentPlanId { get; set; }

        [Required]
        public PaymentPlan PaymentPlan { get; set; }

        public DateTime DateOfPayment { get; set; }

        public decimal Amount { get; set; }
    }
}