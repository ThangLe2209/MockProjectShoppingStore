using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public interface ICouponRepository
    {

		void AddCoupon(CouponModel coupon);
        void DeleteCoupon(CouponModel coupon);

        Task<CouponModel?> GetCouponById(Guid couponId);

        Task<CouponModel?> GetCouponByName(string name);

        Task<IEnumerable<CouponModel>> GetCouponsAsync();

        Task<CouponModel?> GetCouponValidByName(string name);

        Task<bool> SaveChangesAsync();
    }
}
