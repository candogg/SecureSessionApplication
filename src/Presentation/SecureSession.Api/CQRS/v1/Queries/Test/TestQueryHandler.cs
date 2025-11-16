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
        : IRequestHandler<TestQuery, ResponseItem<TestResponseDto>>
    {
        public async Task<ResponseItem<TestResponseDto>> Handle(TestQuery request, CancellationToken cancellationToken)
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

            var serverAnswer = $"Your name is {userRequest.Name} {userRequest.Surname}. Welcome!";

            var responseDto = new TestResponseDto
            {
                Data = CryptoHelper.GetInstance.EncryptString(serverAnswer, userSessionKeys.AesKey, userSessionKeys.AesIv)
            };

            return ResponseItem<TestResponseDto>.GenerateSuccess(responseDto);
        }
    }
}
