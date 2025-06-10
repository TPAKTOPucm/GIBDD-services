using FineService.Aggregates.Fine.Entities;
using FineService.DTOs;

namespace FineService.Factory;

public static class PaymentReceiptFactory
{
    public static string PriceServiceUrl { get; set; }
    public static async Task<PaymentReceipt> Create(PaymentReceiptFactoryDto dto)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync(PriceServiceUrl, JsonContent.Create<PaymentReceiptFactoryDto>(dto));
        var receiptDto = await response.Content.ReadFromJsonAsync<PaymentReceiptDto>();
        return new PaymentReceipt(receiptDto.Id, receiptDto.Price, receiptDto.BankCode, receiptDto.AccountCode);
    }
}
