using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShopApi.Dtos;

namespace OnlineShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly OnlineShopDataContext _db;

        public ProductsController(OnlineShopDataContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _db.Products.ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                Price = dto.Price,
                Discount = dto.Discount,
                Stock = dto.Stock,
                CreatedAt = DateTime.Now
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return Ok(product);
        }

        // Hiển thị tất cả các mặt hàng có giảm giá <= 10%
        [HttpGet("discount-less-than-or-equal-10")]
        public async Task<IActionResult> GetProductsDiscountLessThanOrEqual10()
        {
            var products = await _db.Products
                .Where(p => p.Discount <= 10)
                .ToListAsync();

            return Ok(products);
        }

        // Hiển thị tất cả các mặt hàng không có giảm giá
        [HttpGet("no-discount")]
        public async Task<IActionResult> GetProductsNoDiscount()
        {
            var products = await _db.Products
                .Where(p => p.Discount == 0)
                .ToListAsync();

            return Ok(products);
        }

        // Hiển thị tất cả các mặt hàng có số lượng tồn kho <= 5
        [HttpGet("stock-less-than-or-equal-5")]
        public async Task<IActionResult> GetProductsStockLessThanOrEqual5()
        {
            var products = await _db.Products
                .Where(p => p.Stock <= 5)
                .ToListAsync();

            return Ok(products);
        }

        // Hiển thị tất cả các mặt hàng có giá sau giảm <= 100000
        [HttpGet("final-price-less-than-or-equal-100000")]
        public async Task<IActionResult> GetProductsFinalPriceLessThanOrEqual100000()
        {
            var products = await _db.Products
                .Where(p => p.Price * (1 - p.Discount / 100m) <= 100000)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,
                    p.Discount,
                    FinalPrice = p.Price * (1 - p.Discount / 100m),
                    p.Stock
                })
                .ToListAsync();

            return Ok(products);
        }

        // Price <= 100000 thì tăng thêm 10%
        [HttpPut("increase-price-10")]
        public async Task<IActionResult> IncreasePrice10()
        {
            var products = await _db.Products
                .Where(p => p.Price <= 100000)
                .ToListAsync();

            foreach (var p in products)
            {
                p.Price = Math.Round(p.Price * 1.10m, 0);
            }

            await _db.SaveChangesAsync();
            return Ok(new { updated = products.Count });
        }

        // Discount <= 10% thì tăng thêm 5%
        [HttpPut("increase-discount-5")]
        public async Task<IActionResult> IncreaseDiscount5()
        {
            var products = await _db.Products
                .Where(p => p.Discount <= 10)
                .ToListAsync();

            foreach (var p in products)
            {
                p.Discount += 5;
                if (p.Discount > 100) p.Discount = 100;
            }

            await _db.SaveChangesAsync();
            return Ok(new { updated = products.Count });
        }

        // Xoá tất cả các mặt hàng có Stock là 0
        [HttpDelete("delete-out-of-stock")]
        public async Task<IActionResult> DeleteOutOfStock()
        {
            var products = await _db.Products
                .Where(p => p.Stock == 0)
                .ToListAsync();

            _db.Products.RemoveRange(products);
            var deleted = products.Count;

            await _db.SaveChangesAsync();
            return Ok(new { deleted });
        }

        // Hiển thị tất cả các mặt hàng thuộc danh mục CPU, RAM
        [HttpGet("category-cpu-ram")]
        public async Task<IActionResult> GetProductsByCpuAndRam()
        {
            var categoryNames = new[] { "CPU", "RAM" };

            var products = await _db.Products
                .Where(p => categoryNames.Contains(p.Category.CategoryName))
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    CategoryName = p.Category.CategoryName,
                    p.Price,
                    p.Discount,
                    p.Stock,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(products);
        }

        // Hiển thị danh sách các mức giảm giá của cửa hàng
        [HttpGet("discount-levels")]
        public async Task<IActionResult> GetDiscountLevels()
        {
            var result = await _db.Products
                .Select(p => p.Discount)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị xem có bao nhiêu mức giảm giá khác nhau
        [HttpGet("discount-level-count")]
        public async Task<IActionResult> GetDiscountLevelCount()
        {
            var result = await _db.Products
                .Select(p => p.Discount)
                .Distinct()
                .CountAsync();

            return Ok(new { SoMucGiamGia = result });
        }

        // Hiển thị xem có bao nhiêu mức giảm giá khác nhau và số lượng mặt hàng có mức giảm giá đó
        [HttpGet("discount-statistics")]
        public async Task<IActionResult> GetDiscountStatistics()
        {
            var result = await _db.Products
                .GroupBy(p => p.Discount)
                .Select(g => new
                {
                    MucGiamGia = g.Key,
                    SoLuongMatHang = g.Count()
                })
                .OrderBy(x => x.MucGiamGia)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị xem có bao nhiêu mức giảm giá khác nhau và số lượng mặt hàng có mức giảm giá đó
        // Sắp xếp theo số lượng giảm dần
        [HttpGet("discount-statistics-desc")]
        public async Task<IActionResult> GetDiscountStatisticsDesc()
        {
            var result = await _db.Products
                .GroupBy(p => p.Discount)
                .Select(g => new
                {
                    MucGiamGia = g.Key,
                    SoLuongMatHang = g.Count()
                })
                .OrderByDescending(x => x.SoLuongMatHang)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị xem có bao nhiêu mức giảm giá khác nhau và số lượng mặt hàng có mức giảm giá đó
        // Chỉ hiển thị các mức giảm giá có số lượng >= 5 và sắp xếp theo số lượng tăng dần
        [HttpGet("discount-statistics-min5")]
        public async Task<IActionResult> GetDiscountStatisticsMin5()
        {
            var result = await _db.Products
                .GroupBy(p => p.Discount)
                .Select(g => new
                {
                    MucGiamGia = g.Key,
                    SoLuongMatHang = g.Count()
                })
                .Where(x => x.SoLuongMatHang >= 5)
                .OrderBy(x => x.SoLuongMatHang)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các mặt hàng cùng với CategoryName
        [HttpGet("products-with-category")]
        public async Task<IActionResult> GetProductsWithCategory()
        {
            var result = await _db.Products
                .Join(_db.Categories,
                    p => p.CategoryId,
                    c => c.CategoryId,
                    (p, c) => new
                    {
                        p.ProductName,
                        c.CategoryName
                    })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các mặt hàng cùng với SupplierName
        [HttpGet("products-with-supplier")]
        public async Task<IActionResult> GetProductsWithSupplier()
        {
            var result = await _db.Products
                .Join(_db.Suppliers,
                    p => p.SupplierId,
                    s => s.SupplierId,
                    (p, s) => new
                    {
                        p.ProductName,
                        s.SupplierName
                    })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các mặt hàng cùng với thông tin chi tiết của Category và Supplier
        [HttpGet("products-category-supplier")]
        public async Task<IActionResult> GetProductsCategorySupplier()
        {
            var result = await _db.Products
                .Join(_db.Categories,
                    p => p.CategoryId,
                    c => c.CategoryId,
                    (p, c) => new { p, c })
                .Join(_db.Suppliers,
                    pc => pc.p.SupplierId,
                    s => s.SupplierId,
                    (pc, s) => new
                    {
                        pc.p.ProductName,
                        pc.c.CategoryName,
                        s.SupplierName
                    })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị danh mục và số lượng sản phẩm (INNER JOIN + GROUP BY)
        [HttpGet("categories-product-count")]
        public async Task<IActionResult> GetCategoriesProductCount()
        {
            var result = await _db.Categories
                .Join(_db.Products,
                    c => c.CategoryId,
                    p => p.CategoryId,
                    (c, p) => new { c, p })
                .GroupBy(x => x.c.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    SoLuongMatHang = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("products-not-sold")]
        public async Task<IActionResult> GetProductsNotSold()
        {
            var result = await _db.Products
                .Where(p => !_db.OrderDetails.Any(od => od.ProductId == p.ProductId))
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,
                    p.Discount,
                    p.Stock
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}