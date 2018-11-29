using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Starter.Common.DomainTaskStatus;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.Crypto;
using Starter.Services.Enums;
using Starter.Services.QRCodes;
using Starter.Services.Registration.Models;
using Starter.Services.Token;

namespace Starter.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ICryptoContext _cryptoContext;
        private readonly IQRCodeService _qRCodeService;
        private readonly IOptions<JwtOptions> _options;
        private readonly DomainTaskStatus _taskStatus;

        public RegistrationService(IUnitOfWork unitOfWork, ITokenService tokenService, ICryptoContext cryptoContext, IQRCodeService qRCodeService, IOptions<JwtOptions> options, DomainTaskStatus taskStatus)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _cryptoContext = cryptoContext;
            _qRCodeService = qRCodeService;
            _options = options;
            _taskStatus = taskStatus;
        }

        public FinishRegistrationModel Register(UserRegistrationModel user)
        {
            var existedUser = _unitOfWork.Repository<UserEntity>().Set.FirstOrDefault(x => x.Email == user.Email);

            if (existedUser != null)
            {
                _taskStatus.AddError("user", "user already exists");
                return null;
            }

            var salt = _cryptoContext.GenerateSaltAsBase64();
            var secret = _cryptoContext.GeneratePasswordAsBase32();
            var newUser = new UserEntity
            {
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Salt = salt,
                Password = Convert.ToBase64String(_cryptoContext.DeriveKey(user.Password, salt)),
                IsVerified = false,
                Role = UserRoles.User.ToString(),
                TwoFactorAuth = new TwoFactorAuthEntity
                {
                    CreatedAt = DateTimeOffset.Now,
                    Secret = secret,
                }
            };

            _unitOfWork.Repository<UserEntity>().Insert(newUser);
            _unitOfWork.SaveChanges();

            var qrCode = _qRCodeService.GenerateQR(new QRCodes.Models.TotpEncodingOptions
            {
                Email = user.Email,
                Issuer = _options.Value.ValidIssuer,
                Secret = newUser.TwoFactorAuth.Secret
            });

            return new FinishRegistrationModel
            {
                QRCode = qrCode,
                Token = _tokenService.GetRegistrationToken(user.Email)
            };
        }

        public void RegisterServer(ServerRegistrationModel user)
        {
            var salt = _cryptoContext.GenerateSaltAsBase64();
            var newUser = new UserEntity
            {
                Email = null,
                Firstname = null,
                Lastname = null,
                Salt = salt,
                Password = Convert.ToBase64String(_cryptoContext.DeriveKey(user.Password, salt)),
                IsVerified = true,
                Role = UserRoles.Server.ToString()
            };
        }
    }
}