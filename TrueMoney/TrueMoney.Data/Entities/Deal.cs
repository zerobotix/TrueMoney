﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueMoney.Common.Enums;

namespace TrueMoney.Data.Entities
{
    public class Deal : Entity
    {
        public int OwnerId { get; set; }

        [Required]
        public virtual User Owner { get; set; }

        [InverseProperty("Deal")]
        public virtual List<Offer> Offers { get; set; } = new List<Offer>();

        public int? ResultOfferId { get; set; }

        [ForeignKey("ResultOfferId")]
        public virtual Offer ResultOffer { get; set; }

        public int? PaymentPlanId { get; set; }

        public virtual PaymentPlan PaymentPlan { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public string Description { get; set; }

        public int DealPeriod { get; set; }

        public decimal Amount { get; set; }

        public DealStatus DealStatus { get; set; }

        public decimal InterestRate { get; set; }

        public int PaymentCount { get; set; }
    }
}
