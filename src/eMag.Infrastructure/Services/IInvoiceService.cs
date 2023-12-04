namespace eMag.Infrastructure.Services
{
    public interface IInvoiceService
    {
        public Task FetchOrdersAsync();
        public Task CreateInvoicesAsync();
        public Task FetchInvoiceAsync();
        public Task PutInvoiceInEmagAsync();
    }
}