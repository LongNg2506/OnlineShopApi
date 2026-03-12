using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly OnlineShopDataContext _db;

        public CustomersController(OnlineShopDataContext db)
        {
            _db = db;
        }

        // Hiển thị tất cả khách hàng có địa chỉ ở Quận Hải Châu, Thanh Khê
        [HttpGet("hai-chau")]
        public async Task<IActionResult> GetCustomersInHaiChau()
        {
            var customers = await _db.Customers
                .Where(c => c.Address.Contains("Hải Châu"))
                .Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Phone,
                    c.Address,
                    c.BirthDate
                })
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("hai-chau-thanh-khe")]
        public async Task<IActionResult> GetCustomersHaiChauOrThanhKhe()
        {
            var districts = new[] { "Hải Châu", "Thanh Khê" };

            var customers = await _db.Customers
                .Where(c => districts.Any(d => c.Address.Contains(d)))
                .Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Phone,
                    c.Address
                })
                .ToListAsync();

            return Ok(customers);
        }

        // Hiển thị tất cả các khách hàng có năm sinh 1990
        [HttpGet("birthyear-1990")]
        public async Task<IActionResult> GetCustomersBorn1990()
        {
            var customers = await _db.Customers
                .Where(c => c.BirthDate.HasValue && c.BirthDate.Value.Year == 1990)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Phone,
                    c.Address,
                    c.BirthDate
                })
                .ToListAsync();

            return Ok(customers);
        }

        // Hiển thị tất cả các khách hàng có tuổi trên 60
        [HttpGet("age-over-60")]
        public async Task<IActionResult> GetCustomersAgeOver60()
        {
            int currentYear = DateTime.Now.Year;

            var customers = await _db.Customers
                .Where(c => c.BirthDate.HasValue &&
                       (currentYear - c.BirthDate.Value.Year) > 60)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Phone,
                    c.Address,
                    c.BirthDate,
                    Age = currentYear - c.BirthDate.Value.Year
                })
                .ToListAsync();

            return Ok(customers);
        }

        // Hiển thị tất cả các khách hàng có tuổi từ 20 đến 30
        [HttpGet("age-20-30")]
        public async Task<IActionResult> GetCustomersAge20To30()
        {
            int currentYear = DateTime.Now.Year;

            var customers = await _db.Customers
                .Where(c => c.BirthDate.HasValue &&
                       (currentYear - c.BirthDate.Value.Year) >= 20 &&
                       (currentYear - c.BirthDate.Value.Year) <= 30)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Phone,
                    c.Address,
                    c.BirthDate,
                    Age = currentYear - c.BirthDate.Value.Year
                })
                .ToListAsync();

            return Ok(customers);
        }

        // Hiển thị tất cả các khách hàng có sinh nhật hôm nay
        [HttpGet("birthday-today")]
        public async Task<IActionResult> GetCustomersBirthdayToday()
        {
            int todayMonth = DateTime.Today.Month;
            int todayDay = DateTime.Today.Day;

            var customers = await _db.Customers
                .Where(c => c.BirthDate.HasValue &&
                       c.BirthDate.Value.Month == todayMonth &&
                       c.BirthDate.Value.Day == todayDay)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Phone,
                    c.Address,
                    c.BirthDate
                })
                .ToListAsync();

            return Ok(customers);
        }

        // Hiển thị xem có bao nhiêu mức tuổi khác nhau của khách hàng và số lượng khách hàng có mức tuổi đó,
        // sắp xếp theo số lượng khách hàng tăng dần
        [HttpGet("age-statistics-asc")]
        public async Task<IActionResult> GetCustomerAgeStatisticsAsc()
        {
            int currentYear = DateTime.Today.Year;

            var result = await _db.Customers
                .Where(c => c.BirthDate.HasValue)
                .Select(c => new
                {
                    Tuoi = currentYear - c.BirthDate.Value.Year
                })
                .GroupBy(x => x.Tuoi)
                .Select(g => new
                {
                    Tuoi = g.Key,
                    SoLuongKhachHang = g.Count()
                })
                .OrderBy(x => x.SoLuongKhachHang)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị top 5 các khách hàng mua hàng với tổng số tiền mua được từ cao đến thấp trong khoảng từ ngày đến ngày
        [HttpGet("top-5-customers-total-by-date-range")]
        public async Task<IActionResult> GetTop5CustomersTotalByDateRange(DateTime fromDate, DateTime toDate)
        {
            var result = await _db.Customers
                .Join(_db.Orders.Where(o => o.OrderDate.Date >= fromDate.Date && o.OrderDate.Date <= toDate.Date),
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { c, o })
                .Join(_db.OrderDetails,
                    co => co.o.OrderId,
                    od => od.OrderId,
                    (co, od) => new
                    {
                        co.c.CustomerId,
                        co.c.FullName,
                        co.c.Phone,
                        co.c.Address,
                        co.c.BirthDate,
                        ThanhTien = od.Quantity * od.UnitPrice * (1 - od.Discount / 100m)
                    })
                .GroupBy(x => new
                {
                    x.CustomerId,
                    x.FullName,
                    x.Phone,
                    x.Address,
                    x.BirthDate
                })
                .Select(g => new
                {
                    g.Key.CustomerId,
                    g.Key.FullName,
                    g.Key.Phone,
                    g.Key.Address,
                    g.Key.BirthDate,
                    TongSoTienMuaDuoc = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongSoTienMuaDuoc)
                .Take(5)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng mua hàng với tổng số tiền mua hàng
        // gồm các fields: Id, Name, Address, PhoneNumber, Total
        // Dùng INNER JOIN + GROUP BY với lệnh SUM
        [HttpGet("customers-total")]
        public async Task<IActionResult> GetCustomersTotal()
        {
            var result = await _db.Customers
                .Join(_db.Orders,
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { c, o })
                .Join(_db.OrderDetails,
                    co => co.o.OrderId,
                    od => od.OrderId,
                    (co, od) => new
                    {
                        co.c.CustomerId,
                        co.c.FullName,
                        co.c.Address,
                        co.c.Phone,
                        ThanhTien = od.Quantity * od.UnitPrice * (1 - od.Discount / 100m)
                    })
                .GroupBy(x => new
                {
                    x.CustomerId,
                    x.FullName,
                    x.Address,
                    x.Phone
                })
                .Select(g => new
                {
                    Id = g.Key.CustomerId,
                    Name = g.Key.FullName,
                    Address = g.Key.Address,
                    PhoneNumber = g.Key.Phone,
                    Total = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng có địa chỉ ở @Address
        [HttpGet("customers-by-address")]
        public async Task<IActionResult> GetCustomersByAddress(string Address)
        {
            var result = await _db.Customers
                .Where(c => c.Address.Contains(Address))
                .Select(c => new
                {
                    Id = c.CustomerId,
                    Name = c.FullName,
                    Address = c.Address,
                    PhoneNumber = c.Phone,
                    BirthDate = c.BirthDate
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng có năm sinh @YearOfBirth
        [HttpGet("customers-by-year-of-birth")]
        public async Task<IActionResult> GetCustomersByYearOfBirth(int YearOfBirth)
        {
            var result = await _db.Customers
                .Where(c => c.BirthDate.HasValue && c.BirthDate.Value.Year == YearOfBirth)
                .Select(c => new
                {
                    Id = c.CustomerId,
                    Name = c.FullName,
                    Address = c.Address,
                    PhoneNumber = c.Phone,
                    BirthDate = c.BirthDate
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng có tuổi từ @MinAge đến @MaxAge
        [HttpGet("customers-by-age-range")]
        public async Task<IActionResult> GetCustomersByAgeRange(int MinAge, int MaxAge)
        {
            int currentYear = DateTime.Now.Year;

            var result = await _db.Customers
                .Where(c => c.BirthDate.HasValue &&
                       (currentYear - c.BirthDate.Value.Year) >= MinAge &&
                       (currentYear - c.BirthDate.Value.Year) <= MaxAge)
                .Select(c => new
                {
                    Id = c.CustomerId,
                    Name = c.FullName,
                    Address = c.Address,
                    PhoneNumber = c.Phone,
                    BirthDate = c.BirthDate,
                    Age = currentYear - c.BirthDate.Value.Year
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng có sinh nhật là @Date
        [HttpGet("customers-by-birthday")]
        public async Task<IActionResult> GetCustomersByBirthday(DateTime Date)
        {
            var result = await _db.Customers
                .Where(c => c.BirthDate.HasValue &&
                            c.BirthDate.Value.Month == Date.Month &&
                            c.BirthDate.Value.Day == Date.Day)
                .Select(c => new
                {
                    Id = c.CustomerId,
                    Name = c.FullName,
                    Address = c.Address,
                    PhoneNumber = c.Phone,
                    BirthDate = c.BirthDate
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng mua hàng với tổng số tiền
        // trong khoảng từ ngày @FromDate đến ngày @ToDate
        [HttpGet("customers-total-by-date-range-detail")]
        public async Task<IActionResult> GetCustomersTotalByDateRangeDetail(DateTime FromDate, DateTime ToDate)
        {
            var result = await _db.Customers
                .Join(_db.Orders.Where(o => o.OrderDate.Date >= FromDate.Date
                                         && o.OrderDate.Date <= ToDate.Date),
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { c, o })
                .Join(_db.OrderDetails,
                    co => co.o.OrderId,
                    od => od.OrderId,
                    (co, od) => new
                    {
                        co.c.CustomerId,
                        co.c.FullName,
                        co.c.Address,
                        co.c.Phone,
                        ThanhTien = od.Quantity * od.UnitPrice * (1 - od.Discount / 100m)
                    })
                .GroupBy(x => new
                {
                    x.CustomerId,
                    x.FullName,
                    x.Address,
                    x.Phone
                })
                .Select(g => new
                {
                    Id = g.Key.CustomerId,
                    Name = g.Key.FullName,
                    Address = g.Key.Address,
                    PhoneNumber = g.Key.Phone,
                    Total = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            return Ok(result);
        }

        //Function Fullname
        [HttpGet("get-fullname")]
        public async Task<IActionResult> GetFullName(string firstName, string lastName)
        {
            var result = await _db.Database
                .SqlQuery<string>($"SELECT dbo.udf_GetFullName({firstName}, {lastName}) AS Value")
                .FirstOrDefaultAsync();

            return Ok(new
            {
                FirstName = firstName,
                LastName = lastName,
                FullName = result
            });
        }

        //Function CalculateAge
        [HttpGet("calculate-age")]
        public async Task<IActionResult> CalculateAge(int birthYear)
        {
            var result = await _db.Database
                .SqlQuery<int>($"SELECT dbo.udf_CalculateAge({birthYear}) AS Value")
                .FirstOrDefaultAsync();

            return Ok(new
            {
                BirthYear = birthYear,
                Age = result
            });
        }

        //Function RemoveUnicode
        [HttpGet("remove-unicode")]
        public async Task<IActionResult> RemoveUnicode(string text)
        {
            var result = await _db.Database
                .SqlQuery<string>($"SELECT dbo.udf_ConvertUnicodeToNonUnicode({text}) AS Value")
                .FirstOrDefaultAsync();

            return Ok(new
            {
                Original = text,
                Converted = result
            });
        }
    }
}