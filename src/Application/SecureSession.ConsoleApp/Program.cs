using SecureSession.ConsoleApp.Dto;
using SecureSession.ConsoleApp.Extensions;
using SecureSession.ConsoleApp.Helpers.Abstract;
using SecureSession.ConsoleApp.Items;
using System.Diagnostics;

/// <summary>
/// Author: Can DOĞU
/// Basitçe tüm bireysel cihaz veya oturumların şifreleme anahtarlarının dinamik oluştuğu ve paylaşıldığı infrastructure
/// </summary>
internal class Program
{
    static async Task Main()
    {
        Console.Write("Tünel oluşturuluyor...");

        if (!await TunnelHelper.GetInstance.CreateTunnelAsync())
        {
            Console.Write("\n\nTünel oluşturulamadı. Çıkış yapılıyor");
            return;
        }

        Console.Write("\nTünel oluşturuldu!\n\n");

        while (true)
        {
            Console.Write("Ad Soyad: ");
            var name = Console.ReadLine()?.Trim();

            if (name.IsNullOrEmpty() || name!.Equals("x", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            var myNewRequest = new TestPackageDto
            {
                Name = name,
            };

            var cryptedRequest = await TunnelHelper.GetInstance.CryptPackageAsync(myNewRequest);

            if (cryptedRequest is null)
            {
                Console.Write("Sistem hatası\n\n");
                continue;
            }

            var sWatch = Stopwatch.StartNew();

            var serverResponseCrypted = await HttpHelper.GetInstance.PostAsync<ResponseItem<CryptedDto>, RequestDto>(cryptedRequest, "https://localhost:7138/api/data/test", CancellationToken.None);

            sWatch.Stop();

            if (serverResponseCrypted is null || serverResponseCrypted.Data is null)
            {
                Console.Write("Sistem hatası\n\n");
                continue;
            }

            var serverResponse = TunnelHelper.GetInstance.DecryptPackage<TestResponseDto>(serverResponseCrypted!.Data!.Data);

            if (serverResponse is null || serverResponse.Data.IsNullOrEmpty())
            {
                Console.Write("Sistem hatası\n\n");
                continue;
            }

            var totalElapsed = sWatch.Elapsed.TotalMilliseconds;

            Console.Write($"{serverResponse.Data} (Response in {totalElapsed} MS){Environment.NewLine}{Environment.NewLine}");
        }
    }
}