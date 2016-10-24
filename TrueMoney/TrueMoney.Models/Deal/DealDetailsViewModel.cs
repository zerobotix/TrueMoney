﻿namespace TrueMoney.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Common.Enums;
    using TrueMoney.Models.Basic;

    public class DealDetailsViewModel
    {
        public DealModel Deal { get; set; }

        public bool IsCurrentUserBorrower { get; set; }

        public int CurrentUserId { get; set; }

        public bool IsCurrentUserLender { get; set; }

        public bool ShowOffers => IsCurrentUserBorrower;

        public OfferModel CurrentUserOffer { get; set; } // что это? зачем это в деталях сделки??

        public IEnumerable<OfferModel> Offers { get; set; }

        public bool IsCanDeleteOffer => IsCurrentUserBorrower && Deal.DealStatus != DealStatus.Closed && Deal.DealStatus != DealStatus.InProgress;

        //public bool IsMustLoanMoney => IsCurrentUserLender && !Deal.IsWaitForLoan;
    }
}