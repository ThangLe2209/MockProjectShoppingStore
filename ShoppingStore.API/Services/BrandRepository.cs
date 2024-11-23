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
                $"SELECT brands.*, \r\n       COALESCE(totalbrandcounttable.SumBrandQuantity, 0) AS SumBrandQuantity\r\nFROM \"public\".\"Brands\" AS brands\r\nLEFT OUTER JOIN (\r\n    SELECT products.\"BrandId\", SUM(totalproductcounttable.SumProductQuantity) AS SumBrandQuantity\r\n    FROM \"public\".\"Products\" AS products\r\n    JOIN (\r\n        SELECT DISTINCT orderdetails.\"ProductId\", SUM(orderdetails.\"Quantity\") OVER (PARTITION BY orderdetails.\"ProductId\") AS SumProductQuantity\r\n        FROM \"public\".\"OrderDetails\" AS orderdetails\r\n        JOIN \"public\".\"Orders\" AS orders ON orderdetails.\"OrderCode\" = orders.\"OrderCode\"\r\n    ) AS totalproductcounttable\r\n    ON products.\"Id\" = totalproductcounttable.\"ProductId\"\r\n    GROUP BY products.\"BrandId\"\r\n) AS totalbrandcounttable\r\nON brands.\"Id\" = totalbrandcounttable.\"BrandId\"\r\nORDER BY SumBrandQuantity DESC\r\nLIMIT 16"
                ).ToListAsync();
            return query;
            
        }

        public async Task<(IEnumerable<BrandModel>, PaginationMetadata)> GetBrandsByOrderDetailCalculationPaginateAsync(int pageNumber, int pageSize)
        {
            var collection = _context.Brands.FromSql(
                $"SELECT brands.*, \r\n       COALESCE(totalbrandcounttable.SumBrandQuantity, 0) AS SumBrandQuantity\r\nFROM \"public\".\"Brands\" AS brands\r\nLEFT OUTER JOIN (\r\n    SELECT products.\"BrandId\", SUM(totalproductcounttable.SumProductQuantity) AS SumBrandQuantity\r\n    FROM \"public\".\"Products\" AS products\r\n    JOIN (\r\n        SELECT DISTINCT orderdetails.\"ProductId\", SUM(orderdetails.\"Quantity\") OVER (PARTITION BY orderdetails.\"ProductId\") AS SumProductQuantity\r\n        FROM \"public\".\"OrderDetails\" AS orderdetails\r\n        JOIN \"public\".\"Orders\" AS orders ON orderdetails.\"OrderCode\" = orders.\"OrderCode\"\r\n    ) AS totalproductcounttable\r\n    ON products.\"Id\" = totalproductcounttable.\"ProductId\"\r\n    GROUP BY products.\"BrandId\"\r\n) AS totalbrandcounttable\r\nON brands.\"Id\" = totalbrandcounttable.\"BrandId\"\r\nORDER BY SumBrandQuantity DESC\r\nLIMIT 16"
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
