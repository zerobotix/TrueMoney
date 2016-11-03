﻿using TrueMoney.Common.Enums;
using TrueMoney.Data.Entities;
using TrueMoney.Models.Basic;

namespace TrueMoney.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Data;
    using Interfaces;
    using TrueMoney.Models;
    using TrueMoney.Models.Deal;
    using TrueMoney.Models.User;
    using TrueMoney.Models.ViewModels;

    public class DealService : IDealService
    {
        private readonly IUserService _userService;
        private readonly IOfferService _offerService;
        private readonly ITrueMoneyContext _context;

        public DealService(
            IUserService userService,
            ITrueMoneyContext context,
            IOfferService offerService)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (offerService == null)
            {
                throw new ArgumentNullException(nameof(offerService));
            }

            if (userService == null)
            {
                throw new ArgumentNullException(nameof(userService));
            }

            _context = context;
            _userService = userService;
            _offerService = offerService;
        }

        public async Task<DealIndexViewModel> GetAllOpen(int currentUserId) //Пример адекватного метода
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
            return new DealIndexViewModel()
            {
                CurrentUserId = currentUserId,
                Deals = Mapper.Map<List<DealModel>>(await _context.Deals.Where(x => x.DealStatus == DealStatus.Open).ToListAsync()),
                IsCurrentUserActive = currentUser.IsActive
            };
        }

        public async Task<YourActivityViewModel> GetYourActivityViewModel(int currentUserId)
        {
            var deals = await GetByUser(currentUserId);
            var offers = await _offerService.GetByUser(currentUserId);
            var user = await _context.Users.FirstAsync(x => x.Id == currentUserId);
            var model = new YourActivityViewModel
            {
                Deals = deals,
                Offers = offers,
                IsCurrentUserActive = user.IsActive
            };

            return model;
        }

        public async Task<DealIndexViewModel> GetAll(int currentUserId)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
            return new DealIndexViewModel
            {
                Deals = Mapper.Map<IList<DealModel>>(await _context.Deals.ToListAsync()),
                CurrentUserId = currentUserId,
                IsCurrentUserActive = currentUser.IsActive
            };
        }

        public async Task<IList<DealModel>> GetByUser(int userId)
        {
            var deals = await _context.Deals.Where(x => x.OwnerId == userId).ToListAsync();
            return Mapper.Map<List<DealModel>>(deals);
        }

        public async Task<DealDetailsViewModel> GetById(int id, int userId)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var deal = await _context.Deals.FirstOrDefaultAsync(x => x.Id == id);
            var result = new DealDetailsViewModel
            {
                CurrentUserId = userId,
                IsCurrentUserActive = currentUser.IsActive,
                Offers = Mapper.Map<IList<OfferModel>>(deal.Offers),
                Deal = Mapper.Map<DealModel>(deal),
                PaymentPlanModel = Mapper.Map<PaymentPlanModel>(deal.PaymentPlan)
            };
            if (deal.PaymentPlan != null)
            {
                result.Payments = Mapper.Map<IList<PaymentModel>>(deal.PaymentPlan.Payments);
            }

            return result;
        }

        //public async Task<Deal> GetByOfferId(int offerId)
        //{
        //    var deals = await _context.Deals
        //        .Where(x => x.Offers.Any(y => y.OffererId == offerId))
        //        .ToListAsync();

        //    return Mapper.Map(deals); // а че за метод? он же вообще не юзается, пока коменчу
        //}

        public async Task ApproveOffer(int offerId)
        {
            var offer = await _context.Offers
                .Include(x => x.Deal)
                .Include(x => x.Deal.Owner)
                .FirstAsync(x => x.Id == offerId);
            offer.IsApproved = true;
            var deal = offer.Deal;
            deal.DealStatus = DealStatus.WaitForApprove;
            await _context.SaveChangesAsync();
        }

        public async Task CancelOfferApproval(int offerId)
        {
            var offer = await _context.Offers
                .Include(x => x.Deal)
                .Include(x => x.Deal.Owner)
                .FirstAsync(x => x.Id == offerId);
            offer.IsApproved = false;
            var deal = offer.Deal;
            deal.DealStatus = DealStatus.Open;
            await _context.SaveChangesAsync();
        }

        public async Task RevertOffer(int offerId)
        {
            var offer = await _context.Offers.FirstAsync(x => x.Id == offerId);
            var deal = await _context.Deals.FirstOrDefaultAsync(x => x.Id == offer.DealId);
            deal.DealStatus = DealStatus.Open;
            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
        }

        public async Task<CreateDealForm> GetCreateDealForm(int userId)
        {
            var res = new CreateDealForm();
            var dealsByUser = await GetByUser(userId);
            if (dealsByUser.All(x => x.DealStatus == DealStatus.Closed))
            {
                res.IsUserCanCreateDeal = true;
            }

            return res;
        }

        public async Task<CreateOfferForm> GetCreateOfferForm(int dealId, int userId)
        {
            var deal = await _context.Deals.FirstAsync(x => x.Id == dealId);
            return new CreateOfferForm
            {
                DealRate = deal.InterestRate,
                DealId = dealId,
                IsUserCanCreateOffer = !(deal.Offers != null && deal.Offers.Count > 0 && deal.Offers.All(x => x.OffererId != userId)) //какая-то сомнительная штука, надо будет перепроверить
            };
        }

        public async Task CreateOffer(CreateOfferForm model, int userId)
        {
            _context.Offers.Add(new Offer
            {
                CreateTime = DateTime.Now,
                OffererId = model.OffererId,
                DealId = model.DealId,
                InterestRate = model.InterestRate,
                Offerer = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId),
                Deal = await _context.Deals.FirstOrDefaultAsync(x => x.Id == model.DealId)
            }); // тут нужен маппинг, но сейчас лень.

            await _context.SaveChangesAsync();
        }

        public async Task FinishDealStartLoan(int userId, int offerId, int dealId) //TODO: отрефакторить по аналогии с предыдущими
        {
            var deal = await _context.Deals
                .Include(x => x.Offers)
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == dealId);
            var finishOffer = deal.Offers.First(x => x.Id == offerId);
            deal.DealStatus = DealStatus.WaitForLoan;
            deal.InterestRate = finishOffer.InterestRate;
            //finish offer
            finishOffer.IsApproved = true;

            await _context.SaveChangesAsync();
        }

        public async Task<int> CreateDeal(CreateDealForm model, int userId)
        {
            var deal = Mapper.Map<Deal>(model);
            deal.OwnerId = userId;
            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();
            return deal.Id;
        }

        public async Task DeleteDeal(int dealId, int userId)
        {
            var deal = await _context.Deals.FirstOrDefaultAsync(x => x.Id == dealId);
            _context.Deals.Remove(deal);

            await _context.SaveChangesAsync();
        }

        private PaymentPlan CalculatePayments(Deal deal)
        {
            return new PaymentPlan { Deal = deal, CreateTime = DateTime.Now };
        }
    }
}