using FineService.Aggregates.Fine.Entities;
using FineService.DTOs;
using System.Text.Json;

namespace FineService.Factory;

public static class PaymentReceiptFactory
{
    public static string PriceServiceUrl { get; set; }
    public static async Task<PaymentReceipt> Create(PaymentReceiptFactoryDto dto)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync(PriceServiceUrl, JsonContent.Create<PaymentReceiptFactoryDto>(dto));
        var receiptDtostr = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"\n\n\n\n{receiptDtostr}\n\n\n\n");
        var receiptDto = JsonSerializer.Deserialize<PaymentReceiptDto>(receiptDtostr);
        return new PaymentReceipt(receiptDto.Id, receiptDto.Price, receiptDto.BankCode, receiptDto.AccountCode);
    }
}
