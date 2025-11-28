//using System;
//using Microsoft.AspNetCore.Mvc;

//namespace SFCTOFC.DailySalesPlanManagement.Server.UI.Controllers;
//public class MobileController : Controller
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SalesmanDailyPlanController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public SalesmanDailyPlanController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("{userId}")]
//        public async Task<IActionResult> GetDailyPlan(int userId, [FromQuery] DateTime? date)
//        {
//            var selectedDate = date?.Date ?? DateTime.Today;

//            // 1. Get all distributor outlets of this salesman
//            var outlets = await _context.DistributorOutlet
//                .Where(o => o.Salesman == userId)
//                .ToListAsync();

//            // 2. Load existing daily plans for this salesman
//            var existingPlans = await _context.SalesmanDailyPlans
//                .Where(p => p.UserId == userId)
//                .ToListAsync();

//            foreach (var outlet in outlets)
//            {
//                // Last plan for this outlet (if exists)
//                var lastPlan = existingPlans
//                    .Where(p => p.OutletId == outlet.Id && p.PlanDate != null)
//                    .OrderByDescending(p => p.PlanDate)
//                    .FirstOrDefault();

//                bool shouldGenerate = false;

//                if (lastPlan == null)
//                {
//                    // No previous plan → generate immediately
//                    shouldGenerate = true;
//                }
//                else
//                {
//                    shouldGenerate = ShouldGeneratePlan(
//                        outlet.Frequency,
//                        lastPlan?.PlanDate ?? DateTime.MinValue,
//                        selectedDate,
//                        outlet.Route,
//                        outlet.CreatedAt
//                    );

//                }

//                if (shouldGenerate)
//                {
//                    // Avoid duplicate if already exists for selected date
//                    bool existsToday = existingPlans.Any(
//                        p => p.OutletId == outlet.Id
//                          && p.PlanDate.Value.Date == selectedDate
//                    );

//                    if (!existsToday)
//                    {
//                        var newPlan = new SalesmanDailyPlans
//                        {
//                            UserId = userId,
//                            OutletId = outlet.Id,
//                            PlanDate = selectedDate,
//                            Status = "Pending",
//                            Frequency = outlet.Frequency,
//                            TargetSales = lastPlan?.TargetSales,
//                            TargetQty = lastPlan?.TargetQty,
//                            CreatedAt = DateTime.Now
//                        };

//                        _context.SalesmanDailyPlans.Add(newPlan);
//                    }
//                }
//            }

//            await _context.SaveChangesAsync();



//            var plans = await (
//                                      from plan in _context.SalesmanDailyPlans
//                                      join outlet in _context.DistributorOutlet
//                                          on plan.OutletId equals outlet.Id
//                                      join task in _context.OutletTasks
//                                          on plan.Id equals task.SalesmanDailyPlanId into taskGroup
//                                      from tasks in taskGroup.DefaultIfEmpty()
//                                      where plan.UserId == userId
//                                          && plan.PlanDate != null
//                                          && plan.PlanDate.Value.Date == selectedDate
//                                      group tasks by new
//                                      {
//                                          plan.Id,
//                                          plan.OutletId,
//                                          outlet.Name,
//                                          outlet.Channel,
//                                          outlet.Owner,
//                                          outlet.PhoneNumber,
//                                          outlet.Address,
//                                          outlet.Region,
//                                          outlet.Province,
//                                          outlet.City,
//                                          outlet.Baranggay,
//                                          outlet.Route,
//                                          plan.Frequency,
//                                          plan.Status,
//                                          plan.PlanDate,
//                                          plan.CheckedIn,
//                                          plan.CheckedOut,
//                                          plan.CallTime,
//                                          outlet.Image,
//                                          plan.TargetSales,
//                                          plan.ActualSales,
//                                          plan.TargetQty,
//                                          outlet.Comments,
//                                          outlet.CallSequence,
//                                      } into g
//                                      select new
//                                      {
//                                          Id = g.Key.Id,
//                                          OutletId = g.Key.OutletId,
//                                          StoreName = g.Key.Name,
//                                          ChannelType = g.Key.Channel,
//                                          Owner = g.Key.Owner,
//                                          PhoneNumber = g.Key.PhoneNumber,
//                                          Address = g.Key.Address,
//                                          Region = g.Key.Region,
//                                          Province = g.Key.Province,
//                                          City = g.Key.City,
//                                          Baranggay = g.Key.Baranggay,

//                                          Frequency = g.Key.Frequency,
//                                          Route = g.Key.Route,
//                                          Status = g.Key.Status,
//                                          PlanDate = g.Key.PlanDate,
//                                          CheckedIn = g.Key.CheckedIn,
//                                          CheckedOut = g.Key.CheckedOut,
//                                          CallTime = g.Key.CallTime,
//                                          Image = g.Key.Image,
//                                          TargetSales = g.Key.TargetSales,
//                                          ActualSales = g.Key.ActualSales,
//                                          TargetQty = g.Key.TargetQty,
//                                          Comments = g.Key.Comments,
//                                          CallSequence = g.Key.CallSequence,

//                                          // NEW FIELDS
//                                          TotalTasks = g.Count(x => x != null),
//                                          CompletedTasks = g.Count(x => x != null && x.IsCompleted == true)
//                                      }
//                                  ).ToListAsync();

//            plans = plans.OrderBy(p => p.CallSequence).ToList();

//            if (!plans.Any())
//                return NotFound(new { message = "No daily plans found." });

//            return Ok(plans);
//        }

//        private bool ShouldGeneratePlan(string freq, DateTime lastPlanDate, DateTime selectedDate, string routeDay, DateTime? outletCreated)
//        {
//            if (outletCreated.HasValue && selectedDate < outletCreated.Value.Date)
//                return false; // don't generate plan before outlet existed

//            if (!Enum.TryParse<DayOfWeek>(routeDay, true, out var routeDayEnum))
//                return false; // invalid route

//            if (selectedDate.DayOfWeek != routeDayEnum)
//                return false; // not the assigned route day

//            switch (freq)
//            {
//                case "F1": // monthly
//                    return lastPlanDate.Month != selectedDate.Month;

//                case "F2": // bi-weekly
//                    return (selectedDate - lastPlanDate).TotalDays >= 14;

//                case "F4": // weekly
//                    return true; // already validated route day

//                default:
//                    return false;
//            }
//        }

//        [HttpPost("submit-order")]
//        public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrderRequest request)
//        {
//            var plan = await _context.SalesmanDailyPlans
//                .FirstOrDefaultAsync(p => p.Id == request.DailyPlanId && p.PlanDate.Value.Date == DateTime.Today);

//            if (plan == null)
//                return NotFound(new { message = "Plan not found" });

//            plan.ActualSales = request.ActualSales;
//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                message = "Order submitted successfully",
//                ActualSales = plan.ActualSales
//            });
//        }

//        [HttpPost("upload-selfie")]
//        public async Task<IActionResult> UploadSelfie([FromForm] SelfieUploadRequest request)
//        {
//            var plan = await _context.SalesmanDailyPlans
//                .FirstOrDefaultAsync(p => p.OutletId == request.OutletId
//                    && p.PlanDate.Value.Date == DateTime.Today);

//            if (plan == null)
//                return NotFound(new { message = "Plan not found" });

//            if (request.Selfie == null)
//                return BadRequest(new { message = "Selfie required" });

//            var fileName = $"{Guid.NewGuid()}_{request.Selfie.FileName}";
//            var directoryPath = Path.Combine("wwwroot", "selfies");
//            Directory.CreateDirectory(directoryPath);

//            var filePath = Path.Combine(directoryPath, fileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await request.Selfie.CopyToAsync(stream);
//            }

//            plan.SelfiePath = fileName;
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Selfie uploaded successfully" });
//        }


//        [HttpPost("checkin")]
//        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
//        {
//            var plan = await _context.SalesmanDailyPlans
//                .FirstOrDefaultAsync(p => p.OutletId == request.OutletId
//                    && p.PlanDate.Value.Date == DateTime.Today);

//            if (plan == null)
//                return NotFound(new { message = "Plan not found" });

//            plan.CheckedIn = DateTime.Now;
//            plan.Status = "CheckedIn";
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Checked in successfully", CheckedIn = plan.CheckedIn, Status = plan.Status });
//        }


//        [HttpPost("checkout")]
//        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
//        {
//            var plan = await _context.SalesmanDailyPlans
//                .FirstOrDefaultAsync(p => p.OutletId == request.OutletId && p.PlanDate.Value.Date == DateTime.Today);

//            if (plan == null)
//                return NotFound(new { message = "Plan not found" });

//            plan.CheckedOut = DateTime.Now;
//            plan.Status = "CheckedOut";

//            if (plan.CheckedIn.HasValue)
//            {
//                plan.CallTime = plan.CheckedOut.Value - plan.CheckedIn.Value;
//            }
//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                message = "Checked out successfully",
//                CheckedOut = plan.CheckedOut,
//                Status = plan.Status,
//                CallTime = plan.CallTime?.ToString()
//            });
//        }

//        [HttpPost("skip")]
//        public async Task<IActionResult> Skip([FromBody] SkipRequest request)
//        {
//            var plan = await _context.SalesmanDailyPlans
//                           .FirstOrDefaultAsync(p => p.OutletId == request.OutletId && p.PlanDate.Value.Date == DateTime.Today);

//            if (plan == null)
//                return NotFound(new { message = "Plan not found" });

//            plan.Skipped = DateTime.Now;
//            plan.Status = "Skipped";
//            plan.SkippedRemarks = request.SkippedRemarks;
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Checked out successfully", Skipped = plan.Skipped, Status = plan.Status, SkippedRemarks = plan.SkippedRemarks });
//        }

//        [HttpPost("add-subroute")]
//        public async Task<IActionResult> AddSubroute([FromBody] AddSubrouteRequest req)
//        {
//            if (req.UserId <= 0)
//                return BadRequest(new { message = "Invalid UserId" });

//            var selectedDate = req.PlanDate?.Date ?? DateTime.Today;
//            int outletId;

//            // ---------------------------------------------
//            // CASE 1 -> ADD NEW OUTLET TO DistributorOutlet
//            // ---------------------------------------------
//            if (req.ExistingOutletId == null)
//            {
//                var newOutlet = new DistributorOutlet
//                {
//                    Name = req.StoreName,
//                    Channel = req.Channel,
//                    Owner = req.Owner,
//                    PhoneNumber = req.PhoneNumber,
//                    Address = req.Address,
//                    Region = req.Region,
//                    Province = req.Province,
//                    City = req.City,
//                    Baranggay = req.Baranggay,
//                    Salesman = req.UserId,
//                    Frequency = req.Frequency,
//                    Comments = req.Comments,
//                    CreatedAt = DateTime.UtcNow
//                };

//                _context.DistributorOutlet.Add(newOutlet);
//                await _context.SaveChangesAsync();

//                outletId = newOutlet.Id; // use new outlet ID
//            }
//            // ---------------------------------------------
//            // CASE 2 -> USE EXISTING OUTLET
//            // ---------------------------------------------
//            else
//            {
//                var outlet = await _context.DistributorOutlet.FindAsync(req.ExistingOutletId);

//                if (outlet == null)
//                    return NotFound(new { message = "Existing outlet not found" });

//                outletId = outlet.Id;
//            }

//            // ---------------------------------------------
//            // PREVENT DUPLICATE DAILY PLAN
//            // ---------------------------------------------
//            bool hasSamePlan = await _context.SalesmanDailyPlans
//                .AnyAsync(p =>
//                    p.UserId == req.UserId &&
//                    p.OutletId == outletId &&
//                    p.PlanDate.Value.Date == selectedDate
//                );

//            if (hasSamePlan)
//                return Conflict(new { message = "Daily plan already exists for this outlet on selected date." });

//            // ---------------------------------------------
//            // CREATE NEW DAILY PLAN
//            // ---------------------------------------------
//            var newPlan = new SalesmanDailyPlans
//            {
//                UserId = req.UserId,
//                OutletId = outletId,
//                PlanDate = selectedDate,
//                Status = "Pending",
//                Frequency = req.Frequency,
//                TargetSales = 0,
//                TargetQty = 0,
//                CreatedAt = DateTime.UtcNow
//            };

//            _context.SalesmanDailyPlans.Add(newPlan);
//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                message = "Subroute added successfully.",
//                outletId = outletId,
//                dailyPlanId = newPlan.Id
//            });
//        }


//        public class AddSubrouteRequest
//        {
//            public int UserId { get; set; }
//            public DateTime? PlanDate { get; set; }

//            // If existing outlet → send existing OutletId
//            public int? ExistingOutletId { get; set; }

//            // If new outlet → send these fields
//            public string? StoreName { get; set; }
//            public string? Channel { get; set; }
//            public string? Owner { get; set; }
//            public string? PhoneNumber { get; set; }
//            public string? Address { get; set; }
//            public string? Region { get; set; }
//            public string? Province { get; set; }
//            public string? City { get; set; }
//            public string? Baranggay { get; set; }
//            public string? Frequency { get; set; }
//            public string? Comments { get; set; }
//        }

//        public class SubmitOrderRequest
//        {
//            public int DailyPlanId { get; set; }
//            public decimal? ActualSales { get; set; }
//        }

//        public class CheckInRequest
//        {
//            public int OutletId { get; set; }
//        }

//        public class CheckOutRequest
//        {
//            public int OutletId { get; set; }
//        }

//        public class SkipRequest
//        {
//            public int OutletId { get; set; }
//            public string? SkippedRemarks { get; set; }
//        }

//        public class SelfieUploadRequest
//        {
//            [FromForm]
//            public IFormFile Selfie { get; set; }

//            [FromForm]
//            public int OutletId { get; set; }
//        }




//    }

//    [Route("api/[controller]")]
//    [ApiController]
//    public class OutletSalesOrderController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public OutletSalesOrderController(AppDbContext context)
//        {
//            _context = context;
//        }


//        [HttpGet("{dailyPlanId}")]
//        public async Task<IActionResult> GetOutletOrders(int dailyPlanId)
//        {

//            var orders = await (from order in _context.OutletSalesOrder
//                                join product in _context.Products
//                                    on order.ProductId equals product.Id
//                                join plan in _context.SalesmanDailyPlans
//                                    on order.DailyPlanId equals plan.Id
//                                join outlet in _context.DistributorOutlet
//                                    on order.OutletId equals outlet.Id
//                                where order.DailyPlanId == dailyPlanId
//                                let prevOrder = (from o in _context.OutletSalesOrder
//                                                 join p in _context.SalesmanDailyPlans
//                                                     on o.DailyPlanId equals p.Id
//                                                 where o.OutletId == order.OutletId
//                                                       && o.ProductId == order.ProductId
//                                                       && p.PlanDate < plan.PlanDate
//                                                 orderby p.PlanDate descending
//                                                 select o
//                                                ).FirstOrDefault()
//                                select new
//                                {
//                                    order.Id,
//                                    order.OutletId,
//                                    order.ProductId,
//                                    order.DailyPlanId,
//                                    Comments = outlet.Comments,
//                                    Address = outlet.Address,
//                                    //Region = outlet.Region,
//                                    //Province = outlet.Province,
//                                    //City = outlet.City,
//                                    //Baranggay = outlet.Baranggay,
//                                    Principal = product.Principal,
//                                    ProductName = product.Description,
//                                    Brand = product.Brand,
//                                    ProductUnitPrice = product.UnitPrice,
//                                    Rationalization = product.Rationalization,
//                                    SRPCase = product.SRPCase,
//                                    SRPPack = product.SRPPack,
//                                    Image = product.Image,

//                                    LastDeliveredQty = prevOrder != null ? prevOrder.SuggestedNextOrderQty : 0,
//                                    LastDeliveredUOM = prevOrder != null ? prevOrder.SuggestedNextOrderUOM : null,
//                                    order.Inventory,
//                                    order.InventoryUOM,
//                                    order.SuggestedOrderQty,
//                                    order.SuggestedOrderUOM,
//                                    order.ActualOrderQty,
//                                    order.ActualOrderUOM,
//                                    order.UnitPrice,
//                                    order.TotalPrice,
//                                    order.SuggestedNextOrderQty,
//                                    order.SuggestedNextOrderUOM,
//                                    order.IsDeleted,
//                                    order.CreatedAt,
//                                    order.UpdatedAt

//                                }).ToListAsync();


//            if (orders == null || !orders.Any())
//                return NotFound(new { message = "No sales orders found for this outlet." });

//            return Ok(orders);
//        }


//        [HttpGet("available-products")]
//        public async Task<IActionResult> GetAvailableProducts()
//        {
//            var products = await _context.Products
//                .Select(p => new
//                {
//                    p.Id,
//                    p.Description,
//                    p.Brand,
//                    p.SRPCase,
//                    p.SRPPack,
//                    p.Image
//                })
//                .ToListAsync();

//            return Ok(products);
//        }


//        [HttpPost("add-product")]
//        public async Task<IActionResult> AddProductToOutlet([FromBody] OutletSalesOrderRequest request)
//        {
//            if (request == null || !request.ProductId.HasValue || !request.OutletId.HasValue || !request.DailyPlanId.HasValue)
//                return BadRequest(new { message = "Invalid request" });

//            var exists = await _context.OutletSalesOrder
//                .AnyAsync(o => o.ProductId == request.ProductId && o.OutletId == request.OutletId && o.DailyPlanId == request.DailyPlanId);

//            if (exists)
//                return Conflict(new { message = "Product already exists for this outlet" });

//            var order = new OutletSalesOrder
//            {
//                OutletId = request.OutletId.Value,
//                ProductId = request.ProductId.Value,
//                DailyPlanId = request.DailyPlanId.Value,
//                ActualOrderQty = request.ActualOrderQty ?? 0,
//                ActualOrderUOM = request.ActualOrderUOM ?? "case",
//                SuggestedNextOrderQty = request.SuggestedNextOrderQty ?? 0,
//                SuggestedNextOrderUOM = request.SuggestedNextOrderUOM ?? "case",
//                UnitPrice = request.UnitPrice ?? 0,
//                Inventory = request.Inventory ?? 0,
//                InventoryUOM = request.InventoryUOM ?? "case",
//                TotalPrice = (request.UnitPrice ?? 0) * (request.ActualOrderQty ?? 0),
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };

//            _context.OutletSalesOrder.Add(order);
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Product added successfully", order });
//        }


//        [HttpPost("order")]
//        public async Task<IActionResult> SaveOrUpdateOrder([FromBody] OutletSalesOrderRequest request)
//        {
//            if (request == null)
//                return BadRequest(new { message = "Invalid request" });

//            var existingOrder = await _context.OutletSalesOrder
//                .FirstOrDefaultAsync(o => o.DailyPlanId == request.DailyPlanId && o.ProductId == request.ProductId);

//            if (existingOrder == null)
//            {

//                var newOrder = new OutletSalesOrder
//                {
//                    OutletId = request.OutletId,
//                    ProductId = request.ProductId,
//                    DailyPlanId = request.DailyPlanId,
//                    Inventory = request.Inventory,
//                    InventoryUOM = request.InventoryUOM,
//                    LastDeliveredQty = request.LastDeliveredQty,
//                    LastDeliveredUOM = request.LastDeliveredUOM,
//                    SuggestedOrderQty = request.SuggestedOrderQty,
//                    SuggestedOrderUOM = request.SuggestedOrderUOM,
//                    ActualOrderQty = request.ActualOrderQty,
//                    ActualOrderUOM = request.ActualOrderUOM,
//                    UnitPrice = request.UnitPrice,

//                    TotalPrice = request.UnitPrice * request.ActualOrderQty,
//                    SuggestedNextOrderQty = request.SuggestedNextOrderQty,
//                    SuggestedNextOrderUOM = request.SuggestedNextOrderUOM,
//                    CreatedAt = DateTime.Now,
//                    UpdatedAt = DateTime.Now
//                };

//                _context.OutletSalesOrder.Add(newOrder);
//            }
//            else
//            {
//                existingOrder.Inventory = request.Inventory;
//                existingOrder.InventoryUOM = request.InventoryUOM;
//                existingOrder.LastDeliveredQty = request.LastDeliveredQty;
//                existingOrder.LastDeliveredUOM = request.LastDeliveredUOM;
//                existingOrder.SuggestedOrderQty = request.SuggestedOrderQty;
//                existingOrder.SuggestedOrderUOM = request.SuggestedOrderUOM;
//                existingOrder.ActualOrderQty = request.ActualOrderQty;
//                existingOrder.ActualOrderUOM = request.ActualOrderUOM;
//                existingOrder.UnitPrice = request.UnitPrice;
//                existingOrder.TotalPrice = request.UnitPrice * request.ActualOrderQty;
//                existingOrder.SuggestedNextOrderQty = request.SuggestedNextOrderQty;
//                existingOrder.SuggestedNextOrderUOM = request.SuggestedNextOrderUOM;
//                existingOrder.UpdatedAt = DateTime.Now;
//            }

//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Order saved successfully." });
//        }


//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteOrder(int id)
//        {
//            var order = await _context.OutletSalesOrder.FindAsync(id);

//            if (order == null)
//                return NotFound(new { message = "Order not found" });

//            order.IsDeleted = true;
//            order.DeletedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Order deleted successfully." });
//        }

//        [HttpPost("cancel/{id}")]
//        public async Task<IActionResult> CancelOrder(int id)
//        {
//            var order = await _context.OutletSalesOrder.FindAsync(id);

//            if (order == null)
//                return NotFound(new { message = "Order not found" });

//            order.IsDeleted = false;
//            order.DeletedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Order delete canceled successfully." });
//        }



//        public class OutletSalesOrderRequest
//        {
//            public int? OutletId { get; set; }
//            public int? ProductId { get; set; }
//            public int? DailyPlanId { get; set; }
//            public int? Inventory { get; set; }
//            public string? InventoryUOM { get; set; }
//            public int? LastDeliveredQty { get; set; }
//            public string? LastDeliveredUOM { get; set; }
//            public int? SuggestedOrderQty { get; set; }
//            public string? SuggestedOrderUOM { get; set; }
//            public int? ActualOrderQty { get; set; }

//            public string? ActualOrderUOM { get; set; }
//            public decimal? UnitPrice { get; set; }
//            public decimal? TotalPrice { get; set; }
//            public int? SuggestedNextOrderQty { get; set; }
//            public string? SuggestedNextOrderUOM { get; set; }
//        }
//    }


//    [ApiController]
//    [Route("api/[controller]")]
//    public class OutletTasksController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        public OutletTasksController(AppDbContext context) => _context = context;

//        // GET: api/OutletTasks/{planId}/tasks?assignedTo=5&createdBy=2
//        [HttpGet("{planId}/tasks")]
//        public async Task<IActionResult> GetTasks(int planId, [FromQuery] int? assignedTo, [FromQuery] int? createdBy)
//        {
//            var query = _context.OutletTasks
//                .Where(t => t.SalesmanDailyPlanId == planId)
//                .AsQueryable();

//            if (assignedTo.HasValue)
//                query = query.Where(t => t.AssignedToUserId == assignedTo.Value);

//            if (createdBy.HasValue)
//                query = query.Where(t => t.CreatedByUserId == createdBy.Value);


//            var tasks = await query
//                 .OrderByDescending(t => t.CreatedAt)
//                 .Select(t => new
//                 {
//                     t.Id,
//                     t.SalesmanDailyPlanId,
//                     t.TaskName,
//                     t.Notes,
//                     IsCompleted = t.IsCompleted ?? false, // <-- default false if NULL
//                     t.CompletedAt,
//                     t.CreatedAt,
//                     CreatedBy = t.CreatedBy == null ? null : new { t.CreatedBy.Id, t.CreatedBy.UserName, t.CreatedBy.FirstName, t.CreatedBy.LastName, t.CreatedBy.Position },
//                     AssignedTo = t.AssignedTo == null ? null : new { t.AssignedTo.Id, t.AssignedTo.UserName, t.AssignedTo.FirstName, t.AssignedTo.LastName, t.AssignedTo.Position }
//                 })
//                 .ToListAsync();


//            return Ok(tasks);
//        }

//        // POST: api/OutletTasks/add-task
//        [HttpPost("add-task")]
//        public async Task<IActionResult> AddTask([FromBody] AddTaskRequest request)
//        {
//            // validate plan exists
//            var plan = await _context.SalesmanDailyPlans.FindAsync(request.SalesmanDailyPlanId);
//            if (plan == null) return NotFound(new { message = "Daily plan not found" });

//            // get creator id from claims (adjust claim name)
//            //var userIdClaim = User.FindFirst("u_id")?.Value ?? User.FindFirst("sub")?.Value;

//            //if (!int.TryParse(userIdClaim, out var creatorId))
//            //    return Unauthorized(new { message = "Invalid user" });

//            // optionally verify creator has supervisor role (add role check here)

//            var task = new MBLOutletTasks
//            {
//                SalesmanDailyPlanId = request.SalesmanDailyPlanId,
//                TaskName = request.TaskName,
//                Notes = request.Notes,
//                CreatedByUserId = request.CreatedByUserId,
//                AssignedToUserId = request.AssignedToUserId,
//                CreatedAt = DateTime.UtcNow
//            };

//            _context.OutletTasks.Add(task);
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Task added", taskId = task.Id });
//        }

//        // POST: api/OutletTasks/assign
//        [HttpPost("assign")]
//        public async Task<IActionResult> AssignTask([FromBody] AssignTaskRequest req)
//        {
//            var task = await _context.OutletTasks.FindAsync(req.TaskId);
//            if (task == null) return NotFound(new { message = "Task not found" });

//            task.AssignedToUserId = req.AssignedToUserId;
//            task.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return Ok(new { message = "Task assigned" });
//        }

//        // POST: api/OutletTasks/complete
//        [HttpPost("complete")]
//        public async Task<IActionResult> CompleteTask([FromBody] CompleteTaskRequest req)
//        {
//            var task = await _context.OutletTasks.FindAsync(req.TaskId);
//            if (task == null)
//                return NotFound(new { message = "Task not found" });

//            task.IsCompleted = req.Completed;
//            task.CompletedAt = req.Completed ? DateTime.UtcNow : null;
//            task.CompletedRemarks = req.Remarks;

//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Task completed", task.Id, task.IsCompleted, task.CompletedRemarks });
//        }


//        public class AddTaskRequest
//        {
//            public int SalesmanDailyPlanId { get; set; }
//            public int? AssignedToUserId { get; set; }
//            public int CreatedByUserId { get; set; }
//            public string TaskName { get; set; } = string.Empty;
//            public string? Notes { get; set; }
//        }

//        public class AssignTaskRequest
//        {
//            public int TaskId { get; set; }
//            public int AssignedToUserId { get; set; }
//        }

//        public class CompleteTaskRequest
//        {
//            public int TaskId { get; set; }
//            public bool Completed { get; set; } = true;
//            public string? Remarks { get; set; }
//        }
//    }

//    [ApiController]
//    [Route("api/[controller]")]
//    public class OutletListController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        public OutletListController(AppDbContext context) => _context = context;

//        [HttpGet]
//        public async Task<IActionResult> GetOutlets(int salesmanId)
//        {
//            var outlets = await _context.DistributorOutlet
//                .Where(o => o.Salesman == salesmanId)
//                .ToListAsync();

//            return Ok(outlets);
//        }


//        [HttpPost("update-outlet")]
//        public async Task<IActionResult> UpdateOutlet([FromBody] UpdateOutletRequest req)
//        {
//            var outlet = await _context.DistributorOutlet.FindAsync(req.Id);
//            if (outlet == null)
//                return NotFound(new { message = "Outlet not found" });

//            outlet.Frequency = req.Frequency;
//            outlet.CallSequence = req.CallSequence;
//            outlet.Comments = req.Comments;
//            outlet.SubRoute1 = req.SubRoute1;
//            outlet.SubRoute2 = req.SubRoute2;
//            outlet.SubRoute3 = req.SubRoute3;
//            outlet.SubRoute4 = req.SubRoute4;
//            outlet.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Outlet updated successfully" });
//        }

//        public class UpdateOutletRequest
//        {
//            public int Id { get; set; }
//            public string? Frequency { get; set; }
//            public int? CallSequence { get; set; }
//            public string? Comments { get; set; }
//            public string? SubRoute1 { get; set; }
//            public string? SubRoute2 { get; set; }
//            public string? SubRoute3 { get; set; }
//            public string? SubRoute4 { get; set; }
//        }

//    }


//}
