using eMag.Infrastructure.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace eMag.AzureFunctions.Functions
{
    public class InvoiceTasks
    {
        private readonly ILogger _logger;
        private readonly InvoiceService _invoiceService;

        public InvoiceTasks(ILoggerFactory loggerFactory, InvoiceService invoiceService)
        {
            _logger = loggerFactory.CreateLogger<InvoiceTasks>();
            _invoiceService = invoiceService;
        }

        [Function("InvoiceFunction")]
        public async Task ExecuteInvoiceTasks([TimerTrigger("0 */30 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Executing Invoice Tasks start at: {DateTime.UtcNow}");

            await _invoiceService.FetchOrdersAsync();
            await _invoiceService.CreateInvoicesAsync();
            await _invoiceService.FetchInvoiceAsync();
            await _invoiceService.PutInvoiceInEmagAsync();
        }
    }
}
