using eMag.Infrastructure.Models;
using eMag.Infrastructure.Requests;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;


namespace eMag.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ILogger _logger;
        private List<Order> orders { get; set; }
        private List<Invoice> invoices { get; set; }
        private List<InvoicePdf> invoicesPdf { get; set; }



        public InvoiceService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<InvoiceService>();
            orders = new List<Order>();
            invoices = new List<Invoice>();
            invoicesPdf = new List<InvoicePdf>();
        }



        public async Task FetchOrdersAsync()
        {
            try
            {
                var apiUrls = new List<string>()
                {
                    "https://marketplace-api.emag.ro/api-3",
                    "https://marketplace-api.emag.bg/api-3",
                    "https://marketplace-api.emag.hu/api-3"
                };

                _logger.LogInformation("Encoding credentials...");

                var credentials = GetEncodedCredentials();

                foreach (var apiUrl in apiUrls)
                {
                    var newOrders = await FetchOrdersFromApiAsync(apiUrl, credentials);

                    if (newOrders != null && newOrders.Any())
                    {
                        var ordersWithoutInvoice = GetOrdersWithoutInvoices(newOrders);

                        ordersWithoutInvoice.ForEach(order => order.ApiUrl = apiUrl);

                        orders.AddRange(ordersWithoutInvoice);
                    }
                    else
                    {
                        _logger.LogWarning($"No orders fetched from: {apiUrl}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching orders: {ex.Message}");
            }
        }

        public async Task CreateInvoicesAsync()
        {
            try
            {
                var client = new RestClient("https://YOUR_DOMAIN.fakturownia.pl");
                var request = CreateRequestForInvoices();

                var apiToken = GetApiToken();

                foreach (var order in orders)
                {
                    var invoice = CreateInvoiceFromOrder(order);
                    invoices.Add(invoice);

                    var requestBody = new PostInvoiceRequest(apiToken!, invoice);

                    AddInvoiceToRequest(request, requestBody);

                    var response = await client.PostAsync(request);

                    if (!response.IsSuccessful)
                    {
                        _logger.LogError($"Failed to create invoice for order {order.OrderId}. HTTP status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating invoices: {ex.Message}");
            }
        }


        public async Task FetchInvoiceAsync()
        {
            try
            {
                var client = new RestClient("https://YOUR_DOMAIN.fakturownia.pl");

                foreach (var invoice in invoices)
                {
                    var invoicePdf = await GetInvoicePdfAsync(client, invoice);

                    if (invoicePdf != null)
                    {
                        AddInvoicePdfDetails(invoicePdf, invoice);
                        invoicesPdf.Add(invoicePdf);
                        _logger.LogInformation($"Fetched invoice PDF for order: {invoice.InvoiceId}");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to fetch invoice PDF for order: {invoice.InvoiceId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching invoice PDFs: {ex.Message}");
            }
        }

        public async Task PutInvoiceInEmagAsync()
        {
            try
            {
                foreach (var invoicePdf in invoicesPdf)
                {
                    var credentials = GetEncodedCredentials();

                    var client = new RestClient(invoicePdf.ApiUrl);
                    var request = CreateRequestForOrderAttachment(invoicePdf.OrderId);

                    var invoiceAttachment = CreateInvoiceAttachment(invoicePdf);

                    AddInvoiceAttachmentToRequest(request, invoiceAttachment);
                    var response = await client.PostAsync(request);

                    if (response.IsSuccessful)
                    {
                        _logger.LogInformation($"Invoice attachment successfully added for order: {invoicePdf.OrderId}");
                    }
                    else
                    {
                        _logger.LogError($"Failed to add invoice attachment for order: {invoicePdf.OrderId}. HTTP status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error putting invoice in Emag: {ex.Message}");
            }
        }

        //FetchOrdersAsync
        private async Task<List<Order>> FetchOrdersFromApiAsync(string apiUrl, string credentials)
        {
            _logger.LogInformation($"Fetching orders from API: {apiUrl}");

            try
            {
                var client = new RestClient(apiUrl);
                var request = new RestRequest("order");

                request.AddHeader("Authorization", "Basic " + credentials);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("status", 3);

                var response = await client.GetAsync(request);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation($"Successfully fetched orders from API: {apiUrl}");

                    return JsonConvert.DeserializeObject<List<Order>>(response.Content!)!;
                }
                else
                {
                    _logger.LogError($"Failed to fetch orders from API: {apiUrl}. HTTP status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching orders from API: {apiUrl}. Exception: {ex.Message}");
            }

            return new List<Order>();
        }

        private string GetEncodedCredentials()
        {
            var userName = Environment.GetEnvironmentVariable("EMAG_USER_NAME");
            var userPassword = Environment.GetEnvironmentVariable("EMAG_USER_PASSWORD");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))
            {
                _logger.LogError("EMAG_USER_NAME or EMAG_USER_PASSWORD environment variable is missing or empty.");

                throw new ArgumentException("EMAG_USER_NAME or EMAG_USER_PASSWORD environment variable is missing or empty.");
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{userPassword}"));
        }


        private List<Order> GetOrdersWithoutInvoices(List<Order> orders)
        {
            return orders.Where(order => !order.Attachments.Any(attachment => attachment.Type != 1)).ToList();
        }


        
        //CreateInvoicesAsync
        private RestRequest CreateRequestForInvoices()
        {
            var request = new RestRequest("invoices.json", Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            return request;
        }

        private string GetApiToken()
        {
            var apiToken = Environment.GetEnvironmentVariable("FAKTUROWANIA_API_TOKEN");

            if (string.IsNullOrEmpty(apiToken))
            {
                _logger.LogError("FAKTUROWANIA_API_TOKEN environment variable is missing or empty.");

                throw new Exception("FAKTUROWANIA_API_TOKEN environment variable is missing or empty.");
            }

            return apiToken;
        }

        private Invoice CreateInvoiceFromOrder(Order order)
        {
            var regionInfo = new RegionInfo(order.CustomerDetails.Country);

            return new Invoice
            {
                InvoiceId = order.OrderId,
                Kind = "vat",
                Income = 1,
                SellDate = order.SellDate,
                PaymentToKind = 5,
                CustomerId = order.CustomerDetails.CustomerId,
                Positions = order.Positions,
                Language = regionInfo.TwoLetterISORegionName,
                Currency = regionInfo.ISOCurrencySymbol,
                ApiUrl = order.ApiUrl
            };
        }

        private void AddInvoiceToRequest(RestRequest request, PostInvoiceRequest requestBody)
        {
            request.AddBody(JsonConvert.SerializeObject(requestBody));
        }



        //FetchInvoice
        private async Task<InvoicePdf> GetInvoicePdfAsync(RestClient client, Invoice invoice)
        {
            try
            {
                var request = CreateRequestForInvoicePdf(invoice.InvoiceId);
                var apiToken = GetApiToken();

                request.AddJsonBody(apiToken!);

                var response = await client.GetAsync(request);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation($"Successfully fetched invoice PDF for order: {invoice.InvoiceId}");
                    return JsonConvert.DeserializeObject<InvoicePdf>(response.Content!)!;
                }
                else
                {
                    _logger.LogError($"Failed to fetch invoice PDF for order: {invoice.InvoiceId}. HTTP status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching invoice PDF: {ex.Message}");
            }

            return null;
        }

        private RestRequest CreateRequestForInvoicePdf(int invoiceId)
        {
            var request = new RestRequest($"/invoices/{invoiceId}.pdf");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            return request;
        }

        private void AddInvoicePdfDetails(InvoicePdf invoicePdf, Invoice invoice)
        {
            invoicePdf.OrderId = invoice.InvoiceId;
            invoicePdf.ApiUrl = invoice.ApiUrl;
            invoicePdf.InvoiceUrl = $"https://twojaDomena.fakturownia.pl/invoice/{invoicePdf.InvoiceToken}.pdf";
        }

        
        //PutInvoiceInEmag
        private RestRequest CreateRequestForOrderAttachment(int orderId)
        {
            var request = new RestRequest("order/attachment");
            request.AddHeader("Authorization", "Basic " + GetEncodedCredentials());
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("id", orderId);
            return request;
        }

        private InvoiceAttachment CreateInvoiceAttachment(InvoicePdf invoicePdf)
        {
            return new InvoiceAttachment
            {
                OrderId = invoicePdf.OrderId,
                Url = invoicePdf.InvoiceUrl,
                Type = 1
            };
        }

        private void AddInvoiceAttachmentToRequest(RestRequest request, InvoiceAttachment invoiceAttachment)
        {
            request.AddJsonBody(invoiceAttachment);
        }
    }
}