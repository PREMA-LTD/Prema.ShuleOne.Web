using System;
using System.Buffers.Text;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prema.ShuleOne.Web.Server.Services;

public class MpesaRequestService
{
    private readonly HttpClient _httpClient;

    public MpesaRequestService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendMpesaRequestAsync(long mpesaNumber, int amount)
    {
        // Define the request URL (replace with the actual endpoint)
        string url = "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest";


        string token = await GetAccessTokenAsync();
        string password = GeneratePassword(token);

        // Prepare the request body
        var requestBody = new
        {
            BusinessShortCode = 174379,
            Password = password,
            Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"), // Generate timestamp dynamically
            TransactionType = "CustomerPayBillOnline",
            Amount = amount,
            PartyA = mpesaNumber,
            PartyB = 174379,
            PhoneNumber = mpesaNumber,
            CallBackURL = "https://f650-41-57-106-66.ngrok-free.app/api/Finance/mpesa/callback",
            AccountReference = "student_admn",
            TransactionDesc = "Payment of X"
        };

        // Serialize the request body to JSON
        string json = JsonConvert.SerializeObject(requestBody);

        // Prepare the HTTP request
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        // Add the Authorization header
        request.Headers.Add("Authorization", $"Bearer {token}");

        try
        {
            // Send the request
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            // Ensure the response is successful
            response.EnsureSuccessStatusCode();

            // Read the response
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response: " + responseContent);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Error sending request: " + ex.Message);
        }
    }

    private static async Task<string> GetAccessTokenAsync()
    {
        var client = new HttpClient();

        // Define the request URL
        var requestUrl = "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials";

        // Create the request
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        //string authToken = GenerateAuthToken("6Rg70lB6mQutR9FdOAYJJc1jg9hAjqcR", "9NfZGGqEkUOG6pYN");
        //request.Headers.Add("Authorization", $"{authToken}");

        request.Headers.Add("Authorization", "Basic NlJnNzBsQjZtUXV0UjlGZE9BWUpKYzFqZzloQWpxY1I6OU5mWkdHcUVrVU9HNnBZTg==");



        try
        {
            // Send the request
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Read and parse the response JSON
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var token = JObject.Parse(jsonResponse)["access_token"]?.ToString();

            return token ?? throw new Exception("Token not found in the response.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    private string GeneratePassword(string token)
    {
        string passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919";
        return Base64UrlEncoder.Encode($"174379{passkey}{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}");
    }

    private static string GenerateAuthToken(string key, string secret)
    {
        return Base64UrlEncoder.Encode($"{key}:{secret}");
    }

    public IActionResult ProcessCallback([FromBody] MpesaCallback callback)
    {
        // Validate input
        if (callback?.Body?.StkCallback == null)
        {
            return new BadRequestResult();
        }

        // Handle different callback scenarios
        switch (callback.Body.StkCallback.ResultCode)
        {
            case 0: // Successful transaction
                return ProcessSuccessfulTransaction(callback);

            case 1032: // User canceled
                return ProcessCancelledTransaction(callback);

            default: // Other error codes
                return ProcessFailedTransaction(callback);
        }
    }

    private IActionResult ProcessSuccessfulTransaction(MpesaCallback callback)
    {
        var stkCallback = callback.Body.StkCallback;

        // Extract transaction details
        var amount = stkCallback.CallbackMetadata?.Item?
            .FirstOrDefault(i => i.Name == "Amount")?.Value;

        var mpesaReceiptNumber = stkCallback.CallbackMetadata?.Item?
            .FirstOrDefault(i => i.Name == "MpesaReceiptNumber")?.Value;

        var phoneNumber = stkCallback.CallbackMetadata?.Item?
            .FirstOrDefault(i => i.Name == "PhoneNumber")?.Value;

        var transactionDate = stkCallback.CallbackMetadata?.Item?
            .FirstOrDefault(i => i.Name == "TransactionDate")?.Value;

        // Log or process the successful transaction
        Console.WriteLine($"Successful Transaction: " +
            $"Amount: {amount}, " +
            $"Receipt: {mpesaReceiptNumber}, " +
            $"Phone: {phoneNumber}, " +
            $"Date: {transactionDate}");

        return new OkResult();
    }

    private IActionResult ProcessCancelledTransaction(MpesaCallback callback)
    {
        var stkCallback = callback.Body.StkCallback;

        // Log cancellation
        Console.WriteLine($"Transaction Canceled: " +
            $"MerchantRequestID: {stkCallback.MerchantRequestID}, " +
            $"CheckoutRequestID: {stkCallback.CheckoutRequestID}");

        return new OkResult();
    }

    private IActionResult ProcessFailedTransaction(MpesaCallback callback)
    {
        var stkCallback = callback.Body.StkCallback;

        // Log failure
        Console.WriteLine($"Transaction Failed: " +
            $"Result Code: {stkCallback.ResultCode}, " +
            $"Description: {stkCallback.ResultDesc}");

        return new OkResult();
    }

    public class MpesaCallback
    {
        public Body Body { get; set; }
    }

    public class Body
    {
        public StkCallback StkCallback { get; set; }
    }

    public class StkCallback
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public CallbackMetadata CallbackMetadata { get; set; }
    }

    public class CallbackMetadata
    {
        public List<MetadataItem> Item { get; set; }
    }

    public class MetadataItem
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }
    }
}
