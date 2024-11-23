using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ShoppingStoreContext _context;
        public CouponRepository(ShoppingStoreContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }
        public void AddCoupon(CouponModel coupon)
        {
            _context.Coupons.Add(coupon);
        }

        public void DeleteCoupon(CouponModel coupon)
        {
            _context.Coupons.Remove(coupon);
        }

        public async Task<CouponModel?> GetCouponById(Guid couponId)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);
        }

        public async Task<CouponModel?> GetCouponByName(string name)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<CouponModel?> GetCouponValidByName(string name)
        {
            var couponResult = await _context.Coupons.FirstOrDefaultAsync(c => c.Name == name && c.Status == 1);
            if (couponResult != null)
            {
                if(couponResult.Quantity <= 0)
                {
                    throw new Exception("Coupon has over in stock!");
                }
                TimeSpan remainingTime = couponResult.DateExpired - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining > 0)
                {
                    return couponResult;
                }
                else throw new Exception("Coupon already expired");
            }
            return null;
        }

        public async Task<IEnumerable<CouponModel>> GetCouponsAsync()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
