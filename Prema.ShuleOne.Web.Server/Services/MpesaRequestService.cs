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
using Prema.ShuleOne.Web.Server.Models;

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
        string url = "https://api.safaricom.co.ke/mpesa/stkpush/v2/processrequest";


        string token = await GetAccessTokenAsync();
        string password = GeneratePassword(token);

        Console.WriteLine($"token:  {token}");
        Console.WriteLine($"Password:  {password}");

        // Prepare the request body
        var requestBody = new
        {
            BusinessShortCode = 4148215,
            Password = password,
            Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"), // Generate timestamp dynamically
            TransactionType = "CustomerPayBillOnline",
            Amount = amount,
            PartyA = mpesaNumber,
            PartyB = 4148215,
            PhoneNumber = mpesaNumber,
            CallBackURL = "https://www.prema.co.ke/api/Transactions/Confirmation",
            AccountReference = "Admission Fee",
            TransactionDesc = "Payment of Admission Fee"
        };


        Console.WriteLine($"requestBody:  {System.Text.Json.JsonSerializer.Serialize(requestBody)}");

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


    public async Task<string> QueryTransactionStatusAsync(string securityCredential, string transactionID, string occasion)
    {
        string queueTimeoutUrl = "";
        string remarks = "";
        string identifierType = "4";
        string resultUrl = "";
        string partyA = "600992";
        string initiator = "premaadmin";
        string accessToken = await GetAccessTokenAsync();
        var requestUrl = "https://sandbox.safaricom.co.ke/mpesa/transactionstatus/v1/query";


       

        // Prepare the payload
        var payload = new
        {
            Initiator = initiator,
            SecurityCredential = securityCredential,
            CommandID = "TransactionStatusQuery",
            TransactionID = transactionID,
            PartyA = partyA,
            IdentifierType = identifierType,
            ResultURL = resultUrl,
            QueueTimeOutURL = queueTimeoutUrl,
            Remarks = remarks,
            Occasion = occasion
        };

        var jsonPayload = JsonConvert.SerializeObject(payload);

        // Prepare the request
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };

        // Add headers
        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        try
        {
            // Send the request
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Read and return the response content
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            // Handle exceptions (log or rethrow as needed)
            throw new Exception("Error querying transaction status", ex);
        }
    }

    private static async Task<string> GetAccessTokenAsync()
    {
        var client = new HttpClient();

        // Define the request URL
        var requestUrl = "https://api.safaricom.co.ke/oauth/v2/generate?grant_type=client_credentials";

        // Create the request
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        //string authToken = GenerateAuthToken("6Rg70lB6mQutR9FdOAYJJc1jg9hAjqcR", "9NfZGGqEkUOG6pYN");
        //request.Headers.Add("Authorization", $"{authToken}");

        //request.Headers.Add("Authorization", "Basic NlJnNzBsQjZtUXV0UjlGZE9BWUpKYzFqZzloQWpxY1I6OU5mWkdHcUVrVU9HNnBZTg==");
        request.Headers.Add("Authorization", $"Basic {Base64UrlEncoder.Encode("ACAlSZnp822Epfecmmwyca6t9yFQG96UQV6k8Me2wiHqOn7C:eHfkokl2UiUMjW1XezfmB87WAjen23ASEL7DmZ1EZo25GAtGnZLqNp5DNs4j5Ofw")}");



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
        //string passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919";
        string passkey = "95990e167ff6d333a4d5f2392a72f8b7700b48b890d78c5ac97b70cef4ee0e43";
        return Base64UrlEncoder.Encode($"4148215{passkey}{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}");
    }

    private static string GenerateAuthToken(string key, string secret)
    {
        return Base64UrlEncoder.Encode($"{key}:{secret}");
    }

    public IActionResult ProcessExpressCallback([FromBody] MpesaExpressCallback callback)
    {
        // Validate input
        if (callback?.Body?.stkCallback == null)
        {
            return new BadRequestResult();
        }

        // Handle different callback scenarios
        switch (callback.Body.stkCallback.ResultCode)
        {
            case 0: // Successful transaction
                return ProcessSuccessfulTransaction(callback);

            case 1032: // User canceled
                return ProcessCancelledTransaction(callback);

            default: // Other error codes
                return ProcessFailedTransaction(callback);
        }
    }

    public IActionResult ProcessCallback([FromBody] MpesaC2BResult mpesaC2BResult)
    {
        // Validate input
        if (mpesaC2BResult == null)
        {
            return new BadRequestResult();
        }
        // Log the result
        Console.WriteLine($"C2B Result: " +
            $"TransAmount: {mpesaC2BResult.TransAmount}, " +
            $"Transaction ID: {mpesaC2BResult.TransID}");

        return new OkResult();
    }
    private IActionResult ProcessSuccessfulTransaction(MpesaExpressCallback callback)
    {
        var stkCallback = callback.Body.stkCallback;

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

    private IActionResult ProcessCancelledTransaction(MpesaExpressCallback callback)
    {
        var stkCallback = callback.Body.stkCallback;

        // Log cancellation
        Console.WriteLine($"Transaction Canceled: " +
            $"MerchantRequestID: {stkCallback.MerchantRequestID}, " +
            $"CheckoutRequestID: {stkCallback.CheckoutRequestID}");

        return new OkResult();
    }

    private IActionResult ProcessFailedTransaction(MpesaExpressCallback callback)
    {
        var stkCallback = callback.Body.stkCallback;

        // Log failure
        Console.WriteLine($"Transaction Failed: " +
            $"Result Code: {stkCallback.ResultCode}, " +
            $"Description: {stkCallback.ResultDesc}");

        return new OkResult();
    }

    public class MpesaExpressCallback
    {
        public Body Body { get; set; }
    }

    public class Body
    {
        public StkCallback stkCallback { get; set; }
    }

    public class StkCallback
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public CallbackMetadata? CallbackMetadata { get; set; }
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

    public class ResultParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ReferenceItem
    {
        public string Key { get; set; }
    }

    public class ReferenceData
    {
        public ReferenceItem ReferenceItem { get; set; }
    }

    public class Result
    {
        public string ConversationID { get; set; }
        public string OriginatorConversationID { get; set; }
        public ReferenceData ReferenceData { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public List<ResultParameter> ResultParameters { get; set; }
        public int ResultType { get; set; }
        public string TransactionID { get; set; }
    }

    public class ApiResponse
    {
        public Result Result { get; set; }
    }

}
