using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Collections.Generic;

namespace ShoppingStore.API.Services
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ShoppingStoreContext _context;
        public BrandRepository(ShoppingStoreContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        public void AddBrand(BrandModel brand)
        {
            _context.Brands.Add(brand);
        }

        public void DeleteBrand(BrandModel brand)
        {
			_context.Brands.Remove(brand);
		}

        public async Task<BrandModel> GetBrandById(Guid brandId)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.Id == brandId);
        }

        public async Task<BrandModel> GetBrandBySlug(string? slug)
        {
			return await _context.Brands.FirstOrDefaultAsync(b => b.Slug == slug);
		}

        public async Task<IEnumerable<BrandModel>> GetBrandsAsync()
        {
            return await _context.Brands.Include(b => b.Products).ToListAsync();
        }

        public async Task<(IEnumerable<BrandModel>, PaginationMetadata)> GetBrandsAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            // collection to start from
            var collection = _context.Brands as IQueryable<BrandModel>; // adding where clause for filtering and searching when needed 

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                collection = collection.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower())
                        || (a.Description != null && a.Description.ToLower().Contains(searchTerm.ToLower())));
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection
                            .OrderBy(p => p.Name)
                            .Skip(pageSize * (pageNumber - 1)) // Paging (similar for filter and search) done at database level not after the records have been fetched thank to deferred execution => build all query first then call ToListAsync at final (line 56)
                            .Take(pageSize)
                            .ToListAsync();
            return (collectionToReturn, paginationMetadata);
        }

        // Test function to debug postman and learn .SqlQuery() method
        public async Task<IEnumerable<BrandModelTest>> GetBrandsByOrderDetailCalculationAsync() // Test func (only use to debug postman - check SumBrandQuantity property).
        {
            var query = await _context.Database.SqlQuery<BrandModelTest>(
                $"\t\t\tSelect Brands.*, (CASE WHEN totalBrandCountTable.SumBrandQuantity IS NULL THEN 0 ELSE totalBrandCountTable.SumBrandQuantity END) AS SumBrandQuantity\r\n\t\t\tFrom Brands\r\n\t\t\tLeft Outer Join (\r\n\t\t\t\t--Select Products.BrandId, totalProductCountTable.*\r\n\t\t\t\tselect Products.BrandId , Sum(totalProductCountTable.SumProductQuantity) as SumBrandQuantity\r\n\t\t\t\tFrom Products \r\n\t\t\t\tJoin (\r\n\t\t\t\t\tselect distinct ProductId, SUM(Quantity) OVER (PARTITION BY ProductId) as SumProductQuantity\r\n\t\t\t\t\tFrom OrderDetails \r\n\t\t\t\t\tJoin Orders On OrderDetails.OrderCode = Orders.OrderCode\r\n\t\t\t\t) totalProductCountTable\r\n\t\t\t\tON Products.Id = totalProductCountTable.ProductId\r\n\t\t\t\tGroup by Products.BrandId\r\n\t\t\t) totalBrandCountTable\r\n\t\t\tOn Brands.Id = totalBrandCountTable.BrandId\r\n\t\t\tOrder by SumBrandQuantity desc\r\n\t\t\tLIMIT 16"
                ).ToListAsync();
            return query;
            
        }

        public async Task<(IEnumerable<BrandModel>, PaginationMetadata)> GetBrandsByOrderDetailCalculationPaginateAsync(int pageNumber, int pageSize)
        {
            var collection = _context.Brands.FromSql(
                $"\t\t\tSelect Brands.*, (CASE WHEN totalBrandCountTable.SumBrandQuantity IS NULL THEN 0 ELSE totalBrandCountTable.SumBrandQuantity END) AS SumBrandQuantity\r\n\t\t\tFrom Brands\r\n\t\t\tLeft Outer Join (\r\n\t\t\t\t--Select Products.BrandId, totalProductCountTable.*\r\n\t\t\t\tselect Products.BrandId , Sum(totalProductCountTable.SumProductQuantity) as SumBrandQuantity\r\n\t\t\t\tFrom Products \r\n\t\t\t\tJoin (\r\n\t\t\t\t\tselect distinct ProductId, SUM(Quantity) OVER (PARTITION BY ProductId) as SumProductQuantity\r\n\t\t\t\t\tFrom OrderDetails \r\n\t\t\t\t\tJoin Orders On OrderDetails.OrderCode = Orders.OrderCode\r\n\t\t\t\t) totalProductCountTable\r\n\t\t\t\tON Products.Id = totalProductCountTable.ProductId\r\n\t\t\t\tGroup by Products.BrandId\r\n\t\t\t) totalBrandCountTable\r\n\t\t\tOn Brands.Id = totalBrandCountTable.BrandId\r\n\t\t\tOrder by SumBrandQuantity desc\r\n\t\t\tLIMIT 16"
                ) as IQueryable<BrandModel>;

            //collection = collection.Take(16);
            var totalItemCount = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection
                            .Skip(pageSize * (pageNumber - 1)) // Paging (similar for filter and search) done at database level not after the records have been fetched thank to deferred execution => build all query first then call ToListAsync at final (line 56)
                            .Take(pageSize)
                            //.Include(b => b.Products) // add Include will automatic sort to wrong order because Include will trigger Left Join (View Console window when execute query to see)
                            .ToListAsync();
            return (collectionToReturn, paginationMetadata);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
