using MediatR;
using SecureSession.Api.Contracts;
using SecureSession.Api.Dto;
using SecureSession.Api.Entities.Mongo;
using SecureSession.Api.Extensions;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Queries.GetPublicKey
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class GetPublicKeyQueryHandler(IMongoUnitOfWork mUnitOfWork)
        : IRequestHandler<GetPublicKeyQuery, ResponseItem<SessionExchangeDto>>
    {
        public async Task<ResponseItem<SessionExchangeDto>> Handle(GetPublicKeyQuery request, CancellationToken cancellationToken)
        {
            var activeKey = await mUnitOfWork.Repository<CertificateDoc>()
                .Query()
                .Where(x => x.ValidUntil > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .MongoFirstOrDefaultAsync(cancellationToken: cancellationToken)
                ?? throw new Exception("Sistem hatası");

            var sessionExchangeDto = new SessionExchangeDto
            {
                PublicKey = activeKey.PublicKey.ToBase64()
            };

            return ResponseItem<SessionExchangeDto>.GenerateSuccess(sessionExchangeDto);
        }
    }
}
