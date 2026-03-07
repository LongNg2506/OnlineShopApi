using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly OnlineShopDataContext _db;

        public SuppliersController(OnlineShopDataContext db)
        {
            _db = db;
        }

        // Hiển thị tất cả các nhà cung cấp có tên là: SONY, SAMSUNG, TOSHIBA, APPLE
        [HttpGet("brand-list")]
        public async Task<IActionResult> GetSuppliersByNames()
        {
            var supplierNames = new[] { "SONY", "SAMSUNG", "TOSHIBA", "APPLE" };

            var suppliers = await _db.Suppliers
                .Where(s => supplierNames.Contains(s.SupplierName))
                .Select(s => new
                {
                    s.SupplierId,
                    s.SupplierName,
                    s.Phone,
                    s.Address
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        // Hiển thị tất cả các nhà cung cấp không có tên là: SAMSUNG, APPLE
        [HttpGet("not-samsung-apple")]
        public async Task<IActionResult> GetSuppliersNotSamsungApple()
        {
            var excludedNames = new[] { "SAMSUNG", "APPLE" };

            var suppliers = await _db.Suppliers
                .Where(s => !excludedNames.Contains(s.SupplierName))
                .Select(s => new
                {
                    s.SupplierId,
                    s.SupplierName,
                    s.Phone,
                    s.Address
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        // Hiển thị tất cả các nhà cung cấp có địa chỉ ở Quận Hải Châu và Quận Thanh Khê
        [HttpGet("hai-chau-and-thanh-khe")]
        public async Task<IActionResult> GetSuppliersHaiChauAndThanhKhe()
        {
            var suppliers = await _db.Suppliers
                .Where(s => s.Address != null
                         && s.Address.Contains("Hải Châu")
                         && s.Address.Contains("Thanh Khê"))
                .Select(s => new
                {
                    s.SupplierId,
                    s.SupplierName,
                    s.Phone,
                    s.Address
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        // Hiển thị tất cả các nhà cung cấp có địa chỉ ở Quận Hải Châu hoặc Quận Thanh Khê
        [HttpGet("hai-chau-or-thanh-khe")]
        public async Task<IActionResult> GetSuppliersHaiChauOrThanhKhe()
        {
            var suppliers = await _db.Suppliers
                .Where(s => s.Address != null
                         && (s.Address.Contains("Hải Châu")
                             || s.Address.Contains("Thanh Khê")))
                .Select(s => new
                {
                    s.SupplierId,
                    s.SupplierName,
                    s.Phone,
                    s.Address
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        // Hiển thị tất cả nhà cung cấp với số lượng sản phẩm (INNER JOIN + GROUP BY)
        [HttpGet("suppliers-product-count")]
        public async Task<IActionResult> GetSuppliersProductCount()
        {
            var result = await _db.Suppliers
                .Join(_db.Products,
                    s => s.SupplierId,
                    p => p.SupplierId,
                    (s, p) => new { s, p })
                .GroupBy(x => x.s.SupplierName)
                .Select(g => new
                {
                    SupplierName = g.Key,
                    SoLuongMatHang = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("suppliers-not-sold-by-date-range")]
        public async Task<IActionResult> GetSuppliersNotSoldByDateRange(DateTime fromDate, DateTime toDate)
        {
            var result = await _db.Suppliers
                .Where(s => !_db.Products
                    .Where(p => p.SupplierId == s.SupplierId)
                    .Join(_db.OrderDetails,
                        p => p.ProductId,
                        od => od.ProductId,
                        (p, od) => new { od.OrderId })
                    .Join(_db.Orders,
                        x => x.OrderId,
                        o => o.OrderId,
                        (x, o) => o)
                    .Any(o => o.OrderDate.Date >= fromDate.Date && o.OrderDate.Date <= toDate.Date))
                .Select(s => new
                {
                    s.SupplierId,
                    s.SupplierName,
                    s.Phone,
                    s.Address
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}