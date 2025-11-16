using MediatR;
using SecureSession.Api.Contracts;
using SecureSession.Api.Dto;
using SecureSession.Api.Entities.Mongo;
using SecureSession.Api.Extensions;
using SecureSession.Api.Helpers.Abstract;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Commands.CreateSession
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class CreateSessionCommandHandler(IMongoUnitOfWork mUnitOfWork)
        : IRequestHandler<CreateSessionCommand, ResponseItem<SessionExchangeDto>>
    {
        public async Task<ResponseItem<SessionExchangeDto>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            var activeKey = await mUnitOfWork.Repository<CertificateDoc>()
                .Query()
                .OrderByDescending(x => x.CreatedAt)
                .MongoFirstOrDefaultAsync(r => r.ValidUntil > DateTime.UtcNow, cancellationToken) ?? throw new Exception("Sistem hatası... Baştan başlayınız");

            var userPublicKey = CryptoHelper.GetInstance.DecryptDataWithPrivateInChunks(request.Request.ClientPublicKey, activeKey.PrivateKey);

            if (userPublicKey.IsNullOrEmpty())
            {
                throw new Exception("Gelen istek hatalı");
            }

            var newSessionKeyPair = CryptoHelper.GetInstance.GenerateSessionKeyPair();

            if (!newSessionKeyPair.IsValid)
            {
                throw new Exception("Sistem hatası");
            }

            var newSessionGuid = Guid.NewGuid().ToString();

            var userSessionDoc = new SessionDoc
            {
                CreatedAt = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddHours(1),
                SessionCode = newSessionGuid,
                AesKey = newSessionKeyPair.AesKey,
                AesIv = newSessionKeyPair.AesIv,
            };

            await mUnitOfWork.Repository<SessionDoc>()
                .InsertAsync(userSessionDoc, cancellationToken);

            var response = new SessionExchangeDto
            {
                ClientSessionCode = CryptoHelper.GetInstance.EncryptDataWithPublic(newSessionGuid, userPublicKey),
                ClientSessionKey = CryptoHelper.GetInstance.EncryptDataWithPublic(newSessionKeyPair.AesKey, userPublicKey),
                ClientSessionIV = CryptoHelper.GetInstance.EncryptDataWithPublic(newSessionKeyPair.AesIv, userPublicKey),
                KeysValidUntil = userSessionDoc.ValidUntil
            };

            return ResponseItem<SessionExchangeDto>.GenerateSuccess(response);
        }
    }
}
