using MediatR;
using SecureSession.Api.Contracts;
using SecureSession.Api.Entities.Mongo;
using SecureSession.Api.Extensions;
using SecureSession.Api.Helpers.Abstract;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Commands.CreateKeys
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class CreateKeysCommandHandler(IMongoUnitOfWork mUnitOfWork)
        : IRequestHandler<CreateKeysCommand, ResponseItem<object>>
    {
        public async Task<ResponseItem<object>> Handle(CreateKeysCommand request, CancellationToken cancellationToken)
        {
            var activeKey = await mUnitOfWork.Repository<CertificateDoc>()
                .Query()
                .Where(x => x.ValidUntil > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .MongoFirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (activeKey is null)
            {
                var newPair = CryptoHelper.GetInstance.GetKeyPair();

                if (!newPair.IsValid)
                {
                    throw new Exception("Sistemsel hata oluştu");
                }

                activeKey = new CertificateDoc
                {
                    CreatedAt = DateTime.UtcNow,
                    ValidUntil = DateTime.UtcNow.AddDays(7),
                    PrivateKey = newPair.PrivateKey,
                    PublicKey = newPair.PublicKey
                };

                await mUnitOfWork.Repository<CertificateDoc>()
                    .InsertAsync(activeKey, cancellationToken);
            }
            else if (request.WillReplace)
            {
                var newPair = CryptoHelper.GetInstance.GetKeyPair();

                if (!newPair.IsValid)
                {
                    throw new Exception("Sistemsel hata oluştu");
                }

                activeKey.CreatedAt = DateTime.UtcNow;
                activeKey.ValidUntil = DateTime.UtcNow.AddDays(7);
                activeKey.PrivateKey = newPair.PrivateKey;
                activeKey.PublicKey = newPair.PublicKey;

                await mUnitOfWork.Repository<CertificateDoc>()
                    .UpdateAsync(activeKey, cancellationToken);
            }

            return ResponseItem<object>.GenerateSuccess("Ok");
        }
    }
}
