using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;
using Prema.ShuleOne.Web.Backend.BulkSms;
using Prema.ShuleOne.Web.Backend.Database;
using Prema.ShuleOne.Web.Server.AppSettings;
using Prema.ShuleOne.Web.Server.Logging;
using Prema.ShuleOne.Web.Server.Models;
using Prema.ShuleOne.Web.Server.Telegram;

namespace Prema.ShuleOne.Web.Server.BulkSms
{
    public class MobileSasa : IBulkSms
    {
        private readonly MobileSasaSettings mobileSasaSettings;
        private readonly Logger logger;
        private readonly TelegramBot telegram;
        private readonly HttpClient httpClient;
        private readonly IServiceProvider serviceProvider;
        public MobileSasa(IOptionsMonitor<MobileSasaSettings> mobileSasaSettings, Logger logger, TelegramBot telegram, HttpClient httpClient, IServiceProvider serviceProvider)
        {
            this.mobileSasaSettings = mobileSasaSettings.CurrentValue;
            this.logger = logger;
            this.telegram = telegram;
            this.httpClient = httpClient;
            this.serviceProvider = serviceProvider;
        }

        public async Task<bool> SendSms(string recipient_contact, string recipient_name, string message)
        {
            SMSRecord smsRecord = new SMSRecord
            {
                recipient_contact = recipient_contact,
                recipient_name = recipient_name,
                message = message,
                date_time_sent = DateTime.Now,
                failure_count = 0,
                status = sms_status.Pending
            };

            try
            {

                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ShuleOneDatabaseContext>();

                    context.SMSRecord.Add(smsRecord);
                    await context.SaveChangesAsync(); // Save the initial record to get the Id                    

                    if (await MakeSmsRequest(recipient_contact, message))
                    {
                        smsRecord.status = sms_status.Sent;
                        await context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        var errorContent = "Check logs."; // await response.Content.ReadAsStringAsync();
                        smsRecord.status = sms_status.Failed;
                        smsRecord.failure_count += 1;

                        var smsFailure = new SMSFailure
                        {
                            fk_sms_record_id = smsRecord.id,
                            error = errorContent,
                            date_time = DateTime.Now
                        };

                        context.SMSFailure.Add(smsFailure);
                        await context.SaveChangesAsync();

                        logger.WriteToLog($"Failed to send message.", $"Error: {errorContent}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                smsRecord.status = sms_status.Failed;
                smsRecord.failure_count += 1;

                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ShuleOneDatabaseContext>();

                    var smsFailure = new SMSFailure
                    {
                        fk_sms_record_id = smsRecord.id,
                        error = ex.Message,
                        date_time = DateTime.Now
                    };

                    context.SMSFailure.Add(smsFailure);
                    await context.SaveChangesAsync();

                    logger.WriteToLog($"SendSms: {ex}", "Error");
                }
                return false;
            }
        }

        public async Task<bool> ResendSms(int smsRecordId)
        {
            bool request_status = false;
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ShuleOneDatabaseContext>();

                    SMSRecord smsRecord = context.SMSRecord.SingleOrDefault(x => x.id == smsRecordId) ?? throw new Exception("SMS Record Missing");

                    if(await MakeSmsRequest(smsRecord.recipient_contact, smsRecord.message))
                    {
                        smsRecord.status = sms_status.Sent;
                        smsRecord.date_time_sent = DateTime.UtcNow;
                        context.SMSRecord.Update(smsRecord);
                    } else
                    {
                        smsRecord.failure_count += 1;

                        var smsFailure = new SMSFailure
                        {
                            fk_sms_record_id = smsRecord.id,
                            error = "Check logs.",
                            date_time = DateTime.Now
                        };

                        context.SMSFailure.Add(smsFailure);
                    }

                    await context.SaveChangesAsync();

                    request_status = true;
                }
            }
            catch(Exception ex) 
            {
                logger.WriteToLog($"ResendSms: {ex}", "Error");
            }
            return request_status;
        }

        private async Task<bool> MakeSmsRequest(string recipient_contact, string message)
        {
            try
            {
                var requestContent = new
                {
                    senderID = mobileSasaSettings.SenderId,
                    message = message,
                    phones = recipient_contact
                };

                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(requestContent),
                    Encoding.UTF8,
                    "application/json");

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {mobileSasaSettings.ApiToken}");

                var response = await httpClient.PostAsync(mobileSasaSettings.ApiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    logger.WriteToLog($"Failed to send message. Status code: {response.StatusCode}", "Error");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.WriteToLog($"MakeSmsRequest: {ex}", "Error");
                return false;
            }
        }

        public async Task<int> GetBalance()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {mobileSasaSettings.ApiToken}");

                var response = await httpClient.GetAsync($"{mobileSasaSettings.ApiUrl}/v1/get-balance");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error fetching balance: {response.ReasonPhrase}");
                }

                // Parse the response body
                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);

                if (jsonResponse["responseCode"].ToString() == "0400")
                {
                    throw new Exception($"Error fetching balance: {jsonResponse["message"]}");
                }


                // Extract the balance field
                int balance = (int)jsonResponse["balance"];

                return balance;
            }
            catch (Exception ex)
            {
                logger.WriteToLog($"Fetching Balance: {ex}", "Error");
                return -1;
            }
        }

    }
}