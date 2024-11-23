﻿using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShoppingStoreContext _context;
        public OrderRepository(ShoppingStoreContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        public void AddOrder(OrderModel order)
        {
            _context.Orders.Add(order);
        }

        public void AddOrderDetails(IEnumerable<OrderDetails> orderDetails)
        {
            _context.OrderDetails.AddRange(orderDetails);
        }

        public void DeleteOrder(OrderModel order)
        {
            _context.Orders.Remove(order);
        }

        public async Task DeleteOrderDetails(string orderCode)
        {
            var orderDetails = await _context.OrderDetails.Where(o => o.OrderCode == orderCode).ToListAsync();
            _context.OrderDetails.RemoveRange(orderDetails);
        }

        public async Task<OrderModel> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<OrderModel> GetOrderByOrderCodeAsync(string orderCode)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<IEnumerable<OrderModel?>> GetOrdersByUserEmailAsync(string userEmail)
        {
            return await _context.Orders.Where(o => o.UserName == userEmail).OrderByDescending(o => o.CreatedDate).ToListAsync();
        }

        public async Task<IEnumerable<OrderDetails>> GetOrderDetailsByOrderCodeAsync(string orderCode)
        {
            return await _context.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == orderCode).ToListAsync();
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersAsync()
        {
            return await _context.Orders.OrderByDescending(o => o.CreatedDate).ToListAsync();
        }

        public async Task AddStatisticalByOrder(OrderModel order)
        {
            // get orderDetail by orderCode
            var detailsOrder = await _context.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == order.OrderCode)
                .Select(od => new
                {
                    od.Quantity,
                    od.Product.Price,
                    od.Product.CapitalPrice,
                }).ToListAsync();
            // get StatisticalModel by order createdDate
            var statisticalModel = await _context.Statisticals.FirstOrDefaultAsync(s => s.CreatedDate.Date == order.CreatedDate.Date);

            if (statisticalModel != null) {
                foreach (var orderDetail in detailsOrder)
                {
                    // if date existed then sum it all
                    statisticalModel.Quantity += 1;
                    statisticalModel.Sold += orderDetail.Quantity;
                    statisticalModel.Revenue += (Int32) (orderDetail.Quantity * orderDetail.Price);
                    statisticalModel.Profit += (Int32) (orderDetail.Price - orderDetail.CapitalPrice);
                }
                _context.Update(statisticalModel);
            }
            else
            {
                var newStatisticalModel = new StatisticalModel()
                {
                    CreatedDate = order.CreatedDate,
                    Quantity = 0,
                    Sold = 0,
                    Revenue = 0,
                    Profit = 0
                };
                foreach (var orderDetail in detailsOrder)
                {
                    newStatisticalModel.Quantity += 1;
                    newStatisticalModel.Sold += orderDetail.Quantity;
                    newStatisticalModel.Revenue += (Int32)(orderDetail.Quantity * orderDetail.Price);
                    newStatisticalModel.Profit += (Int32)(orderDetail.Price - orderDetail.CapitalPrice);
                }
                _context.Add(newStatisticalModel);

            }
        }


        public async Task RemoveStatisticalByOrder(OrderModel order)
        {
            // này vẫn chưa chuẩn nha còn shippingPrice vs CouponCode nữa thui tạm tha (có orderModel ở trên kìa lấy ra ShippingPrice vs CouponCode để tính toán thêm nha) 
            // get orderDetail by orderCode
            var detailsOrder = await _context.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == order.OrderCode)
                .Select(od => new
                {
                    od.Quantity,
                    od.Product.Price,
                    od.Product.CapitalPrice,
                }).ToListAsync();

            // get StatisticalModel by order createdDate
            var statisticalModel = await _context.Statisticals.FirstOrDefaultAsync(s => s.CreatedDate.Date == order.CreatedDate.Date);

            if (statisticalModel != null)
            {
                foreach (var orderDetail in detailsOrder)
                {
                    // if date existed then sum it all
                    statisticalModel.Quantity -= 1;
                    statisticalModel.Sold -= orderDetail.Quantity;
                    statisticalModel.Revenue -= (Int32)(orderDetail.Quantity * orderDetail.Price);
                    statisticalModel.Profit -= (Int32)(orderDetail.Price - orderDetail.CapitalPrice);
                }
                _context.Update(statisticalModel);
            }
        }

            public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
