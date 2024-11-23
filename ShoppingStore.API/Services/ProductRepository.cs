using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using System.Globalization;
using System.Xml.Linq;

namespace ShoppingStore.API.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageUploadService _imageUploadService;

        public ProductRepository(ShoppingStoreContext context, IWebHostEnvironment webHostEnvironment, IImageUploadService imageUploadService)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
            _imageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
        }

        public async Task<IEnumerable<ProductModel>> GetProductsAsync()
        {
            var result =  await _context.Products.Include("Category")
                .Include("Brand").Include(p => p.Ratings)
                .OrderBy(p => p.Name).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<ProductModel>> GetProductsAsync(string? searchTerm)
        {
            if (searchTerm == null) return await GetProductsAsync();
            var products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower())
                || (p.Description != null && p.Description.ToLower().Contains(searchTerm.ToLower())))
                .Include(p => p.Ratings).Include("Category").Include("Brand")
                .OrderBy(p => p.Name)
                .ToListAsync();
            return products;
        }

        public void SortCollections (ref IQueryable<ProductModel> collection, string? sortBy)
        {
            if (sortBy == "price_increase")
            {
                collection = collection.OrderBy(p => p.Price);
            }
            else if (sortBy == "price_decrease")
            {
                collection = collection.OrderByDescending(p => p.Price);
            }
            else if (sortBy == "product_newest")
            {
                collection = collection.OrderByDescending(p => p.UpdatedDate);
            }
            else if (sortBy == "product_oldest")
            {
                collection = collection.OrderBy(p => p.UpdatedDate);
            }
            else
            {
                collection = collection.OrderByDescending(p => p.UpdatedDate);
            }
            //return collection;
        }

        public async Task<(IEnumerable<ProductModel>, PaginationMetadata)> GetProductsAsync(string? searchTerm, string? sortBy, int? min, int? max, int pageNumber, int pageSize)
        {
            // collection to start from
            var collection = _context.Products as IQueryable<ProductModel>; // adding where clause for filtering and searching when needed 

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                collection = collection.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower())
                        || (a.Description != null && a.Description.ToLower().Contains(searchTerm.ToLower())));
            }

            // sort price
            if (min != null && max != null)
            {
                collection = collection.Where(p => p.Price >= Convert.ToDecimal(min) && p.Price <= Convert.ToDecimal(max));
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            //collection = collection.Include(p => p.Ratings).Include("Category").Include("Brand");

            //if (sortBy == "price_increase")
            //{
            //    collection = collection.OrderBy(p => p.Price);
            //}
            //else if (sortBy == "price_decrease")
            //{
            //    collection = collection.OrderByDescending(p => p.Price);
            //}
            //else if (sortBy == "product_newest")
            //{
            //    collection = collection.OrderByDescending(p => p.UpdatedDate);
            //}
            //else if (sortBy == "product_oldest")
            //{
            //    collection = collection.OrderBy(p => p.UpdatedDate);
            //}
            //else
            //{
            //    collection = collection.OrderByDescending(p => p.UpdatedDate);
            //}

            SortCollections(ref collection, sortBy);

            var collectionToReturn = await collection
                            //.OrderBy(p => p.Name)
                            .Skip(pageSize * (pageNumber - 1)) // Paging (similar for filter and search) done at database level not after the records have been fetched thank to deferred execution => build all query first then call ToListAsync at final (line 56)
                            .Take(pageSize)
                            .Include(p => p.Ratings).Include("Category").Include("Brand")
                            .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<ProductModel?> GetProductAsync(Guid productId)
        {
            return await _context.Products.Include("Category")
                    .Include("Brand").Include(p => p.Ratings)
                    .OrderBy(p => p.Name).FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<ProductModel?> GetProductBySlugAsync(string? slug)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<(IEnumerable<ProductModel>, PaginationMetadata)> GetProductsByBrandSlugAsync(string? slug, string? sortBy, int? min, int? max, int pageNumber, int pageSize)
        {
            BrandModel brand = _context.Brands.Where(b => b.Slug == slug).FirstOrDefault();
            if (brand == null)
            {
                var paginationMetadata = new PaginationMetadata(0, pageSize, currentPage: pageNumber);
                return (new List<ProductModel>(), paginationMetadata);
            }
            else
            {
                var collection = _context.Products.Where(p => p.BrandId == brand.Id);

                // sort price
                if (min != null && max != null)
                {
                    collection = collection.Where(p => p.Price >= Convert.ToDecimal(min) && p.Price <= Convert.ToDecimal(max));
                }

                var totalItemCount = await collection.CountAsync();

                var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

                SortCollections(ref collection, sortBy);

                var collectionToReturn = await collection
                                //.OrderBy(p => p.Name)
                                .Skip(pageSize * (pageNumber - 1)) // Paging (similar for filter and search) done at database level not after the records have been fetched thank to deferred execution => build all query first then call ToListAsync at final (line 56)
                                .Take(pageSize)
                                .Include(p => p.Ratings).Include("Category").Include("Brand")
                                .ToListAsync();

                return (collectionToReturn, paginationMetadata);
            }
        }

        public async Task<(IEnumerable<ProductModel>, PaginationMetadata)> GetProductsByCategorySlugAsync(string? slug, string? sortBy, int? min, int? max, int pageNumber, int pageSize)
        {
            CategoryModel category = _context.Categories.Where(c => c.Slug == slug).FirstOrDefault();
            if (category == null)
            {
                var paginationMetadata = new PaginationMetadata(0, pageSize, currentPage: pageNumber);
                return (new List<ProductModel>(), paginationMetadata);
            }
            else
            {
                var collection = _context.Products.Where(p => p.CategoryId == category.Id);

                // sort price
                if (min != null && max != null)
                {
                    collection = collection.Where(p => p.Price >= Convert.ToDecimal(min) && p.Price <= Convert.ToDecimal(max));
                }

                var totalItemCount = await collection.CountAsync();

                var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

                collection = collection.Include(p => p.Ratings).Include("Category").Include("Brand");

                SortCollections(ref collection, sortBy);

                var collectionToReturn = await collection
                                //.OrderBy(p => p.Name)
                                .Skip(pageSize * (pageNumber - 1)) // Paging (similar for filter and search) done at database level not after the records have been fetched thank to deferred execution => build all query first then call ToListAsync at final (line 56)
                                .Take(pageSize)
                                //.Include(p => p.Ratings).Include("Category").Include("Brand")
                                .ToListAsync();
                return (collectionToReturn, paginationMetadata);
            }
        }

        public async Task AddProductAsync(ProductModel product)
        {
            if (product.ImageUpload != null)
            {
                //string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                //string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                //string filePath = Path.Combine(uploadsDir, imageName);
                //FileStream fs = new FileStream(filePath, FileMode.Create);
                //await product.ImageUpload.CopyToAsync(fs); // copy image fo file by mode create
                //fs.Close();
                //product.Image = imageName;

                var urlResponse = await _imageUploadService.Upload(product.ImageUpload);
                string toBeSearched = "upload/";
                product.Image = urlResponse.Substring(urlResponse.IndexOf(toBeSearched) + toBeSearched.Length);
            }
            _context.Products.Add(product);
        }

        public void DeleteProduct(ProductModel product)
        {
            _context.Products.Remove(product);
        }

        public void DeleteProductImage(string productImagePath)
        {
            if (!string.Equals(productImagePath, "noname.jpg"))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string oldfileImage = Path.Combine(uploadsDir, productImagePath);
                if (System.IO.File.Exists(oldfileImage))
                {
                    System.IO.File.Delete(oldfileImage);
                }
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
