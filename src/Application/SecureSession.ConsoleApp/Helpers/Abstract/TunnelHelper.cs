using SecureSession.ConsoleApp.Dto;
using SecureSession.ConsoleApp.Extensions;
using SecureSession.ConsoleApp.Helpers.Base;
using SecureSession.ConsoleApp.Items;

namespace SecureSession.ConsoleApp.Helpers.Abstract
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class TunnelHelper : HelperBase<TunnelHelper>
    {
        private SessionExchangeDto? sessionParameters;

        public SessionExchangeDto? SessionParameters
        {
            get
            {
                if (sessionParameters is null)
                    return null;

                return new SessionExchangeDto
                {
                    ClientSessionCode = sessionParameters.ClientSessionCode,
                    ClientSessionKey = sessionParameters.ClientSessionKey,
                    ClientSessionIV = sessionParameters.ClientSessionIV,
                    KeysValidUntil = sessionParameters.KeysValidUntil
                };
            }
        }

        public async Task<bool> CreateTunnelAsync()
        {
            var result = await HttpHelper.GetInstance.GetAsync<ResponseItem<SessionExchangeDto>>("https://localhost:7138/api/session/getpublickey", CancellationToken.None);

            if (result == null || !result.IsSuccess || result.Data == null)
            {
                sessionParameters = null;
                return false;
            }

            var serverPublicKey = result!.Data!.PublicKey.FromBase64();

            var myPair = CryptoHelper.GetInstance.GetKeyPair();

            if (!myPair.IsValid)
            {
                sessionParameters = null;
                return false;
            }

            var myPublicCrypted = CryptoHelper.GetInstance.EncryptDataWithPublicInChunks(myPair.PublicKey, serverPublicKey);

            var sessionCreateRequestDto = new CreateSessionDto
            {
                Request = new SessionExchangeDto
                {
                    ClientPublicKey = myPublicCrypted
                }
            };

            var sessionResult = await HttpHelper.GetInstance.PostAsync<ResponseItem<SessionExchangeDto>, CreateSessionDto>(sessionCreateRequestDto, "https://localhost:7138/api/session/CreateSession", CancellationToken.None);

            if (sessionResult == null || !sessionResult.IsSuccess || sessionResult.Data == null)
            {
                sessionParameters = null;
                return false;
            }

            sessionResult!.Data!.DecryptAll(myPair.PrivateKey);

            sessionParameters = sessionResult.Data;

            return true;
        }

        public async Task<RequestDto?> CryptPackageAsync<T>(T? data)
        {
            if (data is null)
            {
                return null;
            }

            if (sessionParameters is null || sessionParameters.KeysValidUntil <= DateTime.UtcNow)
            {
                if (!await CreateTunnelAsync() || sessionParameters is null || !sessionParameters.IsValid)
                {
                    return null;
                }
            }

            var requestDto = new RequestDto
            {
                Request = new CryptedDto
                {
                    ClientSessionCode = sessionParameters.ClientSessionCode!,
                    Data = CryptoHelper.GetInstance.EncryptString(data.Serialize(), sessionParameters.ClientSessionKey, sessionParameters.ClientSessionIV)
                }
            };

            return requestDto;
        }

        public TResponse? DecryptPackage<TResponse>(string? data)
        {
            if (data.IsNullOrEmpty())
            {
                return default;
            }

            if (sessionParameters is null || sessionParameters.KeysValidUntil <= DateTime.UtcNow)
            {
                return default;
            }

            var decryptedServerResponse = CryptoHelper.GetInstance.DecryptString(data!, sessionParameters.ClientSessionKey, sessionParameters.ClientSessionIV);

            if (decryptedServerResponse.IsNullOrEmpty())
            {
                return default;
            }

            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse?)(object?)decryptedServerResponse;
            }

            return decryptedServerResponse.Deserialize<TResponse>();
        }
    }
}
