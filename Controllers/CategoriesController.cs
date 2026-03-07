using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

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
    }
}