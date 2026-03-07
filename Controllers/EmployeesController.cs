using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly OnlineShopDataContext _db;

        public EmployeesController(OnlineShopDataContext db)
        {
            _db = db;
        }

        // Hiển thị tất cả các nhân viên có sinh nhật là tháng này
        [HttpGet("birthday-this-month")]
        public async Task<IActionResult> GetEmployeesBirthdayThisMonth()
        {
            int currentMonth = DateTime.Today.Month;

            var employees = await _db.Employees
                .Where(e => e.BirthDate.HasValue && e.BirthDate.Value.Month == currentMonth)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FullName,
                    e.Phone,
                    e.Address,
                    e.BirthDate,
                    e.HireDate
                })
                .ToListAsync();

            return Ok(employees);
        }

        // Hiển thị xem có bao nhiêu mức tuổi khác nhau của nhân viên và số lượng nhân viên có mức tuổi đó,
        // sắp xếp theo số lượng nhân viên giảm dần
        [HttpGet("age-statistics-desc")]
        public async Task<IActionResult> GetEmployeeAgeStatisticsDesc()
        {
            int currentYear = DateTime.Today.Year;

            var result = await _db.Employees
                .Where(e => e.BirthDate.HasValue)
                .Select(e => new
                {
                    Tuoi = currentYear - e.BirthDate.Value.Year
                })
                .GroupBy(x => x.Tuoi)
                .Select(g => new
                {
                    Tuoi = g.Key,
                    SoLuongNhanVien = g.Count()
                })
                .OrderByDescending(x => x.SoLuongNhanVien)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các nhân viên bán hàng với tổng số tiền bán được
        [HttpGet("employees-total-sales")]
        public async Task<IActionResult> GetEmployeesTotalSales()
        {
            var result = await _db.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FullName,
                    TongDoanhSo = _db.Orders
                        .Where(o => o.EmployeeId == e.EmployeeId)
                        .Join(_db.OrderDetails,
                            o => o.OrderId,
                            od => od.OrderId,
                            (o, od) => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                        .Sum()
                })
                .OrderByDescending(x => x.TongDoanhSo)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị top 3 các nhân viên bán hàng với tổng số tiền bán được từ cao đến thấp trong khoảng từ ngày đến ngày
        [HttpGet("top-3-employees-sales-by-date-range")]
        public async Task<IActionResult> GetTop3EmployeesSalesByDateRange(DateTime fromDate, DateTime toDate)
        {
            var result = await _db.Employees
                .Join(_db.Orders.Where(o => o.OrderDate.Date >= fromDate.Date && o.OrderDate.Date <= toDate.Date),
                    e => e.EmployeeId,
                    o => o.EmployeeId,
                    (e, o) => new { e, o })
                .Join(_db.OrderDetails,
                    eo => eo.o.OrderId,
                    od => od.OrderId,
                    (eo, od) => new
                    {
                        eo.e.EmployeeId,
                        eo.e.FullName,
                        eo.e.Phone,
                        eo.e.Address,
                        eo.e.BirthDate,
                        eo.e.HireDate,
                        ThanhTien = od.Quantity * od.UnitPrice * (1 - od.Discount / 100m)
                    })
                .GroupBy(x => new
                {
                    x.EmployeeId,
                    x.FullName,
                    x.Phone,
                    x.Address,
                    x.BirthDate,
                    x.HireDate
                })
                .Select(g => new
                {
                    g.Key.EmployeeId,
                    g.Key.FullName,
                    g.Key.Phone,
                    g.Key.Address,
                    g.Key.BirthDate,
                    g.Key.HireDate,
                    TongSoTienBanDuoc = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongSoTienBanDuoc)
                .Take(3)
                .ToListAsync();

            return Ok(result);
        }
    }
}