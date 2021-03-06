using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using TrueMoney.Common;
using TrueMoney.Data;
using TrueMoney.Data.Entities;
using TrueMoney.Services.Services;

namespace TrueMoney.Web.Auth_Identity_Startup
{
    public class ApplicationUserManager : UserManager<User, int>
    {
        public ApplicationUserManager(IUserStore<User, int> store, IDataProtectionProvider dataProtectionProvider)
            : base(store)
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<User, int>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = NumericConstants.PasswordMaxLength,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(1);
            this.MaxFailedAccessAttemptsBeforeLockout = 3;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User, int>
            {
                MessageFormat = "Your security code is {0}"
            });
            this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User, int>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            this.EmailService = new EmailService();
            this.SmsService = new SmsService();

            if (dataProtectionProvider != null)
            {
                this.UserTokenProvider =
                    new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }
    }
}