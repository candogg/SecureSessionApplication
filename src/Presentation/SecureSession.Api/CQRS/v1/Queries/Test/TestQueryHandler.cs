using MediatR;
using SecureSession.Api.Contracts;
using SecureSession.Api.Dto;
using SecureSession.Api.Entities.Mongo;
using SecureSession.Api.Extensions;
using SecureSession.Api.Helpers.Abstract;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Queries.Test
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class TestQueryHandler(IMongoUnitOfWork mUnitOfWork)
        : IRequestHandler<TestQuery, ResponseItem<CryptedDto>>
    {
        public async Task<ResponseItem<CryptedDto>> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            if (request is null || request.Request is null || request.Request.ClientSessionCode.IsNullOrEmpty())
            {
                throw new Exception("Hatalı istek");
            }

            var userSessionKeys = await mUnitOfWork.Repository<SessionDoc>()
                .Query()
                .MongoFirstOrDefaultAsync(x => x.SessionCode == request.Request.ClientSessionCode && x.ValidUntil > DateTime.UtcNow, cancellationToken) ?? throw new Exception("Oturum sonlanmış");

            var userRequestUtf8 = CryptoHelper.GetInstance.DecryptString(request.Request.Data!, userSessionKeys.AesKey, userSessionKeys.AesIv);

            if (userRequestUtf8.IsNullOrEmpty())
            {
                throw new Exception("Hatalı istek");
            }

            var userRequest = userRequestUtf8.Deserialize<TestPackageDto>()
                ?? throw new Exception("Hatalı istek");

            var responseDto = new TestResponseDto
            {
                Data = $"Sayın {userRequest.Name}, hoşgeldiniz!"
            };

            var cryptedDto = new CryptedDto
            {
                ClientSessionCode = request.Request.ClientSessionCode,
                Data = CryptoHelper.GetInstance.EncryptString(responseDto.Serialize(), userSessionKeys.AesKey, userSessionKeys.AesIv)
            };

            return ResponseItem<CryptedDto>.GenerateSuccess(cryptedDto);
        }
    }
}
