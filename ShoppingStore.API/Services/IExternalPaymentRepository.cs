using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public interface IExternalPaymentRepository
    {
		void AddVnpayItem(VnpayModel vnpayItem);
        void DeleteVnpayItem(VnpayModel vnpayItem);

        Task<VnpayModel> GetVnpayItemById(Guid Id);
        Task<VnpayModel> GetVnpayItemByPaymentId(string paymentId);

        Task<bool> SaveChangesAsync();
    }
}
