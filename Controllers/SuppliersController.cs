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

        // Hiển thị tất cả nhà cung cấp với số lượng hàng hóa mỗi nhà cung cấp
        // gồm các fields: Id, Name, Address, PhoneNumber, NumberOfProducts
        // Dùng INNER JOIN + GROUP BY với lệnh COUNT
        [HttpGet("suppliers-product-count-detail")]
        public async Task<IActionResult> GetSuppliersProductCountDetail()
        {
            var result = await _db.Suppliers
                .Join(_db.Products,
                    s => s.SupplierId,
                    p => p.SupplierId,
                    (s, p) => new { s, p })
                .GroupBy(x => new
                {
                    x.s.SupplierId,
                    x.s.SupplierName,
                    x.s.Address,
                    x.s.Phone
                })
                .Select(g => new
                {
                    Id = g.Key.SupplierId,
                    Name = g.Key.SupplierName,
                    Address = g.Key.Address,
                    PhoneNumber = g.Key.Phone,
                    NumberOfProducts = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các nhà cung cấp không bán được
        // gồm các fields: Id, Name, Address, PhoneNumber
        [HttpGet("suppliers-not-sold")]
        public async Task<IActionResult> GetSuppliersNotSold()
        {
            var result = await _db.Suppliers
                .Where(s => !_db.Products
                    .Where(p => p.SupplierId == s.SupplierId)
                    .Join(_db.OrderDetails,
                        p => p.ProductId,
                        od => od.ProductId,
                        (p, od) => od)
                    .Any())
                .Select(s => new
                {
                    Id = s.SupplierId,
                    Name = s.SupplierName,
                    Address = s.Address,
                    PhoneNumber = s.Phone
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các mặt hàng không bán được trong khoảng từ ngày @FromDate đến ngày @ToDate
        [HttpGet("products-not-sold-by-date-range")]
        public async Task<IActionResult> GetProductsNotSoldByDateRange(DateTime FromDate, DateTime ToDate)
        {
            var result = await _db.Products
                .Where(p => !_db.OrderDetails
                    .Join(_db.Orders,
                        od => od.OrderId,
                        o => o.OrderId,
                        (od, o) => new { od, o })
                    .Any(x => x.od.ProductId == p.ProductId
                           && x.o.OrderDate.Date >= FromDate.Date
                           && x.o.OrderDate.Date <= ToDate.Date))
                .Select(p => new
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Price = p.Price,
                    Discount = p.Discount,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier.SupplierName
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}