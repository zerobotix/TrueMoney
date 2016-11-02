﻿namespace TrueMoney.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Enums;
    using TrueMoney.Models.Basic;

    public class DealDetailsViewModel
    {
        public DealModel Deal { get; set; }

        public bool IsCurrentUserBorrower => CurrentUserId == Deal.OwnerId;

        public int CurrentUserId { get; set; }

        public bool IsCurrentUserLender
        {
            get
            {
                return Offers != null && Offers.Any() && Offers.Any(x => x.OffererId == CurrentUserId);
            }
        }

        public bool ShowOffers => IsCurrentUserBorrower;

        public OfferModel CurrentUserOffer
        {
            get
            {
                return Offers.FirstOrDefault(x => x.OffererId == CurrentUserId);
            }
        }

        public IEnumerable<OfferModel> Offers { get; set; }

        public PaymentPlanModel PaymentPlanModel { get; set; }

        public IEnumerable<PaymentModel> Payments { get; set; }
    }
}