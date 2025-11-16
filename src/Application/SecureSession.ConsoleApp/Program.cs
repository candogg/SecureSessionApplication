using SecureSession.ConsoleApp.Dto;
using SecureSession.ConsoleApp.Extensions;
using SecureSession.ConsoleApp.Helpers.Abstract;
using SecureSession.ConsoleApp.Items;

/// <summary>
/// Author: Can DOĞU
/// Basitçe tüm bireysel cihaz veya oturumların şifreleme anahtarlarının dinamik oluştuğu ve paylaşıldığı infrastructure
/// </summary>
internal class Program
{
    static async Task Main()
    {
        while (true)
        {
            Console.Write("Name: ");
            var name = Console.ReadLine()?.Trim();

            if (name.IsNullOrEmpty() || name!.Equals("x", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            Console.Write("Surname: ");
            var surName = Console.ReadLine()?.Trim();

            var myNewRequest = new TestPackageDto
            {
                Name = name,
                Surname = surName
            };

            var cryptedResult = await TunnelHelper.GetInstance.CryptPackageAsync(myNewRequest);

            var requestDto = new RequestDto
            {
                Request = cryptedResult
            };

            var serverResponseCrypted = await HttpHelper.GetInstance.PostAsync<ResponseItem<TestResponseDto>, RequestDto>(requestDto, "https://localhost:7138/api/data/test", CancellationToken.None);

            if (serverResponseCrypted is null || serverResponseCrypted.Data is null)
            {
                Console.WriteLine("Sistem hatası");
            }

            var serverResponse = TunnelHelper.GetInstance.DecryptPackage<string>(serverResponseCrypted!.Data!.Data);

            Console.Write($"{serverResponse}{Environment.NewLine}{Environment.NewLine}");
        }
    }
}