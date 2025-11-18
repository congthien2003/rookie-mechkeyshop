using InvoiceApi.Models;
using InvoiceApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InvoiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly InvoiceService _invoiceService;
    private readonly HttpClient _httpClient;
    public InvoiceController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("print")]
    public IActionResult PrintInvoice([FromBody] InvoiceRequest request)
    {
        InvoiceService _invoiceService = new InvoiceService();
        var pdfBytes = _invoiceService.GenerateInvoicePdf(request);
        return File(pdfBytes, "application/pdf", $"invoice_{request.InvoiceNumber}.pdf");
    }

    [HttpGet("call-webhook")]
    public async Task<IActionResult> CallWebHookAsync()
    {
        // ✅ Data cần gửi
        var payload = new
        {
            user_id = 12,
            balance = 2000000,
            currency = "VND",
            token = "sabodeeptry_secret_key"
        };

        // 🔗 URL webhook WordPress của bạn
        var webhookUrl = "http://localhost/webbanhang/?wpwhpro_action=receive_balance&wpwhpro_api_key=almi4fil5vyxch3dhpzhxpbd2fuup1hmdg8o9s7am7bf1m42gtq4sg8aygny8kpw";

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(webhookUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                status = response.StatusCode,
                response = responseBody
            });
        }
        catch (HttpRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
