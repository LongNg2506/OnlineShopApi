using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OnlineShopDataContext _db;

        public OrdersController(OnlineShopDataContext db)
        {
            _db = db;
        }

        // Hiển thị tất cả các đơn hàng có trạng thái là COMPLETED
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedOrders()
        {
            var orders = await _db.Orders
                .Where(o => o.Status == "COMPLETED")
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có trạng thái là COMPLETED trong ngày hôm nay
        [HttpGet("completed-today")]
        public async Task<IActionResult> GetCompletedOrdersToday()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            var orders = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate >= today
                         && o.OrderDate < tomorrow)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng chưa hoàn thành trong tháng này
        [HttpGet("not-completed-this-month")]
        public async Task<IActionResult> GetNotCompletedOrdersThisMonth()
        {
            int currentYear = DateTime.Today.Year;
            int currentMonth = DateTime.Today.Month;

            var orders = await _db.Orders
                .Where(o => o.Status != "COMPLETED"
                         && o.OrderDate.Year == currentYear
                         && o.OrderDate.Month == currentMonth)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có trạng thái là CANCELED
        [HttpGet("canceled")]
        public async Task<IActionResult> GetCanceledOrders()
        {
            var orders = await _db.Orders
                .Where(o => o.Status == "CANCELED")
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có trạng thái là CANCELED trong ngày hôm nay
        [HttpGet("canceled-today")]
        public async Task<IActionResult> GetCanceledOrdersToday()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            var orders = await _db.Orders
                .Where(o => o.Status == "CANCELED"
                         && o.OrderDate >= today
                         && o.OrderDate < tomorrow)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có trạng thái là COMPLETED trong tháng này
        [HttpGet("completed-this-month")]
        public async Task<IActionResult> GetCompletedOrdersThisMonth()
        {
            int currentYear = DateTime.Today.Year;
            int currentMonth = DateTime.Today.Month;

            var orders = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate.Year == currentYear
                         && o.OrderDate.Month == currentMonth)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có trạng thái là COMPLETED trong tháng 1 năm 2021
        [HttpGet("completed-jan-2021")]
        public async Task<IActionResult> GetCompletedOrdersJan2021()
        {
            var orders = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate.Year == 2021
                         && o.OrderDate.Month == 1)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có trạng thái là COMPLETED trong năm 2021
        [HttpGet("completed-2021")]
        public async Task<IActionResult> GetCompletedOrders2021()
        {
            var orders = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate.Year == 2021)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có hình thức thanh toán là CASH
        [HttpGet("payment-cash")]
        public async Task<IActionResult> GetOrdersPaymentCash()
        {
            var orders = await _db.Orders
                .Where(o => o.PaymentMethod == "CASH")
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có hình thức thanh toán là CREDIT CARD
        [HttpGet("payment-credit-card")]
        public async Task<IActionResult> GetOrdersPaymentCreditCard()
        {
            var orders = await _db.Orders
                .Where(o => o.PaymentMethod == "CREDIT CARD")
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị tất cả các đơn hàng có địa chỉ giao hàng là Hà Nội
        [HttpGet("ship-hanoi")]
        public async Task<IActionResult> GetOrdersShipToHaNoi()
        {
            var orders = await _db.Orders
                .Where(o => o.ShipAddress.Contains("Hà Nội"))
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress
                })
                .ToListAsync();

            return Ok(orders);
        }

        // Hiển thị số lượng đơn hàng theo từng ngày khác nhau sắp xếp theo số lượng đơn hàng giảm dần
        [HttpGet("count-by-day-desc")]
        public async Task<IActionResult> GetOrderCountByDayDesc()
        {
            var result = await _db.Orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    SoLuongDonHang = g.Count()
                })
                .OrderByDescending(x => x.SoLuongDonHang)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị số lượng đơn hàng theo từng tháng khác nhau sắp xếp theo số lượng đơn hàng giảm dần
        [HttpGet("count-by-month-desc")]
        public async Task<IActionResult> GetOrderCountByMonthDesc()
        {
            var result = await _db.Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoLuongDonHang = g.Count()
                })
                .OrderByDescending(x => x.SoLuongDonHang)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị số lượng đơn hàng theo từng năm khác nhau sắp xếp theo số lượng đơn hàng giảm dần
        [HttpGet("count-by-year-desc")]
        public async Task<IActionResult> GetOrderCountByYearDesc()
        {
            var result = await _db.Orders
                .GroupBy(o => o.OrderDate.Year)
                .Select(g => new
                {
                    Nam = g.Key,
                    SoLuongDonHang = g.Count()
                })
                .OrderByDescending(x => x.SoLuongDonHang)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị số lượng đơn hàng theo từng năm khác nhau sắp xếp theo số lượng đơn hàng giảm dần,
        // chỉ hiển thị các năm có số lượng đơn hàng >= 5
        [HttpGet("count-by-year-desc-min5")]
        public async Task<IActionResult> GetOrderCountByYearDescMin5()
        {
            var result = await _db.Orders
                .GroupBy(o => o.OrderDate.Year)
                .Select(g => new
                {
                    Nam = g.Key,
                    SoLuongDonHang = g.Count()
                })
                .Where(x => x.SoLuongDonHang >= 5)
                .OrderByDescending(x => x.SoLuongDonHang)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các đơn hàng cùng với thông tin chi tiết khách hàng Customer
        [HttpGet("orders-with-customer")]
        public async Task<IActionResult> GetOrdersWithCustomer()
        {
            var result = await _db.Orders
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    Customer = new
                    {
                        o.Customer.CustomerId,
                        o.Customer.FullName,
                        o.Customer.Phone,
                        o.Customer.Address,
                        o.Customer.BirthDate
                    }
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các đơn hàng cùng với thông tin chi tiết nhân viên Employee
        [HttpGet("orders-with-employee")]
        public async Task<IActionResult> GetOrdersWithEmployee()
        {
            var result = await _db.Orders
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    Employee = o.Employee == null ? null : new
                    {
                        o.Employee.EmployeeId,
                        o.Employee.FullName,
                        o.Employee.Phone,
                        o.Employee.Address,
                        o.Employee.BirthDate,
                        o.Employee.HireDate
                    }
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các đơn hàng cùng với thông tin chi tiết khách hàng Customer và nhân viên Employee
        [HttpGet("orders-with-customer-employee")]
        public async Task<IActionResult> GetOrdersWithCustomerAndEmployee()
        {
            var result = await _db.Orders
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    Customer = new
                    {
                        o.Customer.CustomerId,
                        o.Customer.FullName,
                        o.Customer.Phone,
                        o.Customer.Address,
                        o.Customer.BirthDate
                    },
                    Employee = o.Employee == null ? null : new
                    {
                        o.Employee.EmployeeId,
                        o.Employee.FullName,
                        o.Employee.Phone,
                        o.Employee.Address,
                        o.Employee.BirthDate,
                        o.Employee.HireDate
                    }
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các mặt hàng được bán trong khoảng từ ngày đến ngày, kèm ngày bán
        [HttpGet("products-sold-by-date-range")]
        public async Task<IActionResult> GetProductsSoldByDateRange(DateTime fromDate, DateTime toDate)
        {
            var result = await _db.Orders
                .Where(o => o.OrderDate.Date >= fromDate.Date && o.OrderDate.Date <= toDate.Date)
                .Join(_db.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { o, od })
                .Join(_db.Products,
                    x => x.od.ProductId,
                    p => p.ProductId,
                    (x, p) => new
                    {
                        x.o.OrderId,
                        NgayBan = x.o.OrderDate,
                        p.ProductId,
                        p.ProductName,
                        x.od.Quantity,
                        x.od.UnitPrice,
                        x.od.Discount
                    })
                .OrderBy(x => x.NgayBan)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng mua hàng trong khoảng từ ngày đến ngày, kèm ngày mua
        [HttpGet("customers-by-date-range")]
        public async Task<IActionResult> GetCustomersByDateRange(DateTime fromDate, DateTime toDate)
        {
            var result = await _db.Orders
                .Where(o => o.OrderDate.Date >= fromDate.Date && o.OrderDate.Date <= toDate.Date)
                .Join(_db.Customers,
                    o => o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new
                    {
                        o.OrderId,
                        NgayMua = o.OrderDate,
                        c.CustomerId,
                        c.FullName,
                        c.Phone,
                        c.Address,
                        c.BirthDate
                    })
                .OrderBy(x => x.NgayMua)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả các khách hàng mua hàng (với tổng số tiền) trong khoảng từ ngày đến ngày
        [HttpGet("customers-total-by-date-range")]
        public async Task<IActionResult> GetCustomersTotalByDateRange(DateTime fromDate, DateTime toDate)
        {
            var result = await _db.Orders
                .Where(o => o.OrderDate.Date >= fromDate.Date && o.OrderDate.Date <= toDate.Date)
                .Join(_db.Customers,
                    o => o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new { o, c })
                .Join(_db.OrderDetails,
                    oc => oc.o.OrderId,
                    od => od.OrderId,
                    (oc, od) => new
                    {
                        oc.c.CustomerId,
                        oc.c.FullName,
                        oc.c.Phone,
                        oc.c.Address,
                        oc.c.BirthDate,
                        oc.o.OrderDate,
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
                    TongSoTien = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongSoTien)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả đơn hàng với tổng số tiền của đơn hàng đó
        [HttpGet("orders-with-total")]
        public async Task<IActionResult> GetOrdersWithTotal()
        {
            var result = await _db.Orders
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    TongSoTien = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                })
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả đơn hàng với tổng số tiền đã giao hàng thành công trong khoảng từ ngày đến ngày
        [HttpGet("completed-orders-with-total-by-date-range")]
        public async Task<IActionResult> GetCompletedOrdersWithTotalByDateRange(DateTime fromDate, DateTime toDate)
        {
            var result = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate.Date >= fromDate.Date
                         && o.OrderDate.Date <= toDate.Date)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    TongSoTien = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                })
                .OrderByDescending(x => x.TongSoTien)
                .ToListAsync();

            return Ok(result);
        }

        // Hiển thị tất cả đơn hàng COMPLETED có tổng số tiền bán hàng nhiều nhất trong khoảng từ ngày đến ngày
        [HttpGet("completed-orders-max-total-by-date-range")]
        public async Task<IActionResult> GetCompletedOrdersMaxTotalByDateRange(DateTime fromDate, DateTime toDate)
        {
            var ordersWithTotal = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate.Date >= fromDate.Date
                         && o.OrderDate.Date <= toDate.Date)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    TongSoTien = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                })
                .ToListAsync();

            if (!ordersWithTotal.Any())
                return Ok(new List<object>());

            var maxTotal = ordersWithTotal.Max(x => x.TongSoTien);

            var result = ordersWithTotal
                .Where(x => x.TongSoTien == maxTotal)
                .ToList();

            return Ok(result);
        }

        // Hiển thị tất cả đơn hàng COMPLETED có tổng số tiền bán hàng ít nhất trong khoảng từ ngày đến ngày
        [HttpGet("completed-orders-min-total-by-date-range")]
        public async Task<IActionResult> GetCompletedOrdersMinTotalByDateRange(DateTime fromDate, DateTime toDate)
        {
            var ordersWithTotal = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate.Date >= fromDate.Date
                         && o.OrderDate.Date <= toDate.Date)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    TongSoTien = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                })
                .ToListAsync();

            if (!ordersWithTotal.Any())
                return Ok(new List<object>());

            var minTotal = ordersWithTotal.Min(x => x.TongSoTien);

            var result = ordersWithTotal
                .Where(x => x.TongSoTien == minTotal)
                .ToList();

            return Ok(result);
        }

        // Hiển thị trung bình cộng giá trị các đơn hàng COMPLETED trong khoảng từ ngày đến ngày
        [HttpGet("average-order-value-by-date-range")]
        public async Task<IActionResult> GetAverageOrderValueByDateRange(DateTime fromDate, DateTime toDate)
        {
            var orderValues = await _db.Orders
                .Where(o => o.Status == "COMPLETED"
                         && o.OrderDate.Date >= fromDate.Date
                         && o.OrderDate.Date <= toDate.Date)
                .Select(o => new
                {
                    GiaTriDonHang = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                })
                .ToListAsync();

            if (!orderValues.Any())
                return Ok(new { GiaTriTrungBinh = 0 });

            var averageValue = orderValues.Average(x => x.GiaTriDonHang);

            return Ok(new { GiaTriTrungBinh = averageValue });
        }

        // Hiển thị các đơn hàng COMPLETED có giá trị cao nhất
        [HttpGet("orders-max-value")]
        public async Task<IActionResult> GetOrdersMaxValue()
        {
            var ordersWithTotal = await _db.Orders
                .Where(o => o.Status == "COMPLETED")
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    TongSoTien = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                })
                .ToListAsync();

            if (!ordersWithTotal.Any())
                return Ok(new List<object>());

            var maxValue = ordersWithTotal.Max(x => x.TongSoTien);

            var result = ordersWithTotal
                .Where(x => x.TongSoTien == maxValue)
                .ToList();

            return Ok(result);
        }

        // Hiển thị các đơn hàng COMPLETED có giá trị thấp nhất
        [HttpGet("orders-min-value")]
        public async Task<IActionResult> GetOrdersMinValue()
        {
            var ordersWithTotal = await _db.Orders
                .Where(o => o.Status == "COMPLETED")
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.EmployeeId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.ShipAddress,
                    TongSoTien = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100m))
                })
                .ToListAsync();

            if (!ordersWithTotal.Any())
                return Ok(new List<object>());

            var minValue = ordersWithTotal.Min(x => x.TongSoTien);

            var result = ordersWithTotal
                .Where(x => x.TongSoTien == minValue)
                .ToList();

            return Ok(result);
        }
    }
}