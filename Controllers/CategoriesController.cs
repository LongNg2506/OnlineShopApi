using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShopApi.Dtos;

namespace OnlineShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly OnlineShopDataContext _db;

        public CategoriesController(OnlineShopDataContext db)
        {
            _db = db;
        }

        // Hiển thị tất cả danh mục với tổng số tiền bán được trong mỗi danh mục
        [HttpGet("categories-total-sales")]
        public async Task<IActionResult> GetCategoriesTotalSales()
        {
            var result = await _db.Categories
                .Join(_db.Products,
                    c => c.CategoryId,
                    p => p.CategoryId,
                    (c, p) => new { c, p })
                .Join(_db.OrderDetails,
                    cp => cp.p.ProductId,
                    od => od.ProductId,
                    (cp, od) => new
                    {
                        cp.c.CategoryId,
                        cp.c.CategoryName,
                        ThanhTien = od.Quantity * od.UnitPrice * (1 - od.Discount / 100m)
                    })
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.CategoryName
                })
                .Select(g => new
                {
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả danh mục với số lượng hàng hóa trong mỗi danh mục
        // gồm các fields: Id, Name, Description, NumberOfProducts
        // Dùng INNER JOIN + GROUP BY với lệnh COUNT
        [HttpGet("categories-product-count-detail")]
        public async Task<IActionResult> GetCategoriesProductCountDetail()
        {
            var result = await _db.Categories
                .Join(_db.Products,
                    c => c.CategoryId,
                    p => p.CategoryId,
                    (c, p) => new { c, p })
                .GroupBy(x => new
                {
                    x.c.CategoryId,
                    x.c.CategoryName,
                    x.c.Description
                })
                .Select(g => new
                {
                    Id = g.Key.CategoryId,
                    Name = g.Key.CategoryName,
                    Description = g.Key.Description,
                    NumberOfProducts = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("category-prices")]
        public async Task<IActionResult> GetCategoryPrices(int categoryId)
        {
            var result = await _db.Database
                .SqlQuery<CategoryPriceDto>($"""
            SELECT *
            FROM dbo.udf_Category_GetCategoryPrices({categoryId})
        """)
                .ToListAsync();

            return Ok(result);
        }
    }
}