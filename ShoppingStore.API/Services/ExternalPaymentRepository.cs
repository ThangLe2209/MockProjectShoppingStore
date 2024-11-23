using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public class ExternalPaymentRepository : IExternalPaymentRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IdentityDbContext _identityContext;
        public ExternalPaymentRepository(ShoppingStoreContext context, IdentityDbContext identityContext)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _identityContext = identityContext ?? throw new ArgumentException(nameof(identityContext));
        }

        public void AddVnpayItem(VnpayModel vnpayItem)
        {
            _context.VnpayInfos.Add(vnpayItem);
        }

        public void DeleteVnpayItem(VnpayModel vnpayItem)
        {
            _context.VnpayInfos.Remove(vnpayItem);
        }

        public async Task<VnpayModel> GetVnpayItemById (Guid Id)
        {
            return await _context.VnpayInfos.FirstOrDefaultAsync(vn => vn.Id == Id);
        }
        public async Task<VnpayModel> GetVnpayItemByPaymentId(string paymentId)
        {
            return await _context.VnpayInfos.FirstOrDefaultAsync(vn => vn.PaymentId == paymentId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
	}
}
