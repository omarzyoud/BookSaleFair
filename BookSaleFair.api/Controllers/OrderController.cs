using BookSaleFair.api.Data;
using BookSaleFair.api.Domain.DTOs;
using BookSaleFair.api.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookSaleFair.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly BSFDbContext bSFDbContext;
        private readonly BSFAuthDbContext bsfAuthDbContext;
        private readonly UserManager<IdentityUser> userManager;
        public OrderController(BSFDbContext bSFDbContext, BSFAuthDbContext bsfAuthDbContext, UserManager<IdentityUser> userManager)
        {
            this.bSFDbContext = bSFDbContext;
            this.bsfAuthDbContext = bsfAuthDbContext;
            this.userManager = userManager;
        }
        [HttpPost]
        [Route("CreateOrder")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO model)
        {
            try
            {
                // Validate the incoming user ID
                var user = await bSFDbContext.Users.FindAsync(model.UserId);
                if (user == null)
                {
                    return BadRequest("Invalid User ID.");
                }

                // Create the order
                var order = new Order
                {
                    UserId = model.UserId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending" // You can set the initial status as required
                };

                // Add order items
                foreach (var item in model.OrderItems)
                {
                    var book = await bSFDbContext.Books.FindAsync(item.BookId);
                    if (book == null)
                    {
                        return BadRequest($"Invalid Book ID: {item.BookId}");
                    }

                    var orderItem = new OrderItem
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        Order = order
                    };

                    order.OrderItems.Add(orderItem);
                }

                // Save the order and order items
                bSFDbContext.Orders.Add(order);
                await bSFDbContext.SaveChangesAsync();

                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("AddBooksToOrder")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddBooksToOrder(int orderId, [FromBody] List<OrderItemDTO> items)
        {
            var order = await bSFDbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.Status != "Pending")
            {
                return BadRequest("Invalid order or order is not in a pending state.");
            }

            foreach (var item in items)
            {
                var book = await bSFDbContext.Books.FindAsync(item.BookId);
                if (book == null)
                {
                    return BadRequest($"Invalid Book ID: {item.BookId}");
                }

                var orderItem = new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    OrderId = order.OrderId
                };

                order.OrderItems.Add(orderItem);
            }

            await bSFDbContext.SaveChangesAsync();
            return Ok("Books added to order successfully.");
        }
        [HttpDelete]
        [Route("DeleteBookFromOrder")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteBookFromOrder(int orderId, int bookId)
        {
            var order = await bSFDbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.Status != "Pending")
            {
                return BadRequest("Invalid order or order is not in a pending state.");
            }

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.BookId == bookId);
            if (orderItem == null)
            {
                return BadRequest("Book not found in the order.");
            }

            order.OrderItems.Remove(orderItem);
            await bSFDbContext.SaveChangesAsync();
            return Ok("Book removed from order successfully.");
        }
        [HttpPut]
        [Route("ChangeBookQuantityInOrder")]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> ChangeBookQuantityInOrder(int orderId, int bookId, int newQuantity)
        {
            var order = await bSFDbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.Status != "Pending")
            {
                return BadRequest("Invalid order or order is not in a pending state.");
            }

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.BookId == bookId);
            if (orderItem == null)
            {
                return BadRequest("Book not found in the order.");
            }

            orderItem.Quantity = newQuantity;
            await bSFDbContext.SaveChangesAsync();
            return Ok("Book quantity updated successfully.");
        }

        [HttpDelete]
        [Route("DeleteOrder")]
        //[Authorize]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await bSFDbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.Status != "Pending")
            {
                return BadRequest("Invalid order or order is not in a pending state.");
            }

            bSFDbContext.Orders.Remove(order);
            await bSFDbContext.SaveChangesAsync();
            return Ok("Order deleted successfully.");
        }
        [HttpGet]
        [Route("GetUserOrders")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await bSFDbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for this user.");
            }

            var ordersDto = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemReturnDTO
                {
                    BookId = oi.BookId,
                    Title = oi.Book.Title,
                    Quantity = oi.Quantity,
                    Price = oi.Book.Price
                }).ToList()
            }).ToList();

            return Ok(ordersDto);
        }
        [HttpPost]
        [Route("ChangeOrderStatus")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, string newStatus)
        {
            
            var order = await bSFDbContext.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }
         
            order.Status = newStatus;            
            await bSFDbContext.SaveChangesAsync();
            return Ok("Order status updated successfully.");
        }
        [HttpGet]
        [Route("GetOrderStatus")]
        [Authorize]
        public async Task<IActionResult> GetOrderStatus(int orderId)
        {
            var order = await bSFDbContext.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }
           
            return Ok(new { Status = order.Status });
        }
        [HttpGet]
        [Route("GetAllOrders")]
        //[Authorize(Roles ="Admin,Employee")]
        public async Task<IActionResult> GetAllOrders(DateTime? date = null)
        {
            IQueryable<Order> ordersQuery = bSFDbContext.Orders
       .Include(o => o.User)
       .Include(o => o.OrderItems)
           .ThenInclude(oi => oi.Book);

            if (date.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == date.Value.Date);
            }

            var orders = await ordersQuery.ToListAsync();

            if (orders.Count == 0)
            {
                return NotFound("No orders found.");
            }

            var orderDTOs = orders.Select(o => new OrderGetDTO
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                UserEmail = o.User.Email,
                OrderDate = o.OrderDate,
                Status = o.Status,

                OrderItems = o.OrderItems.Select(oi => new OrderItemGetDTO
                {
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    Quantity = oi.Quantity,
                    Price = oi.Book.Price

                }).ToList()
            }).ToList();

            return Ok(orderDTOs);
        }
        [HttpGet]
        [Route("SearchBooks")]
       // [Authorize]
        public async Task<IActionResult> SearchBooks(string title, string sortByPrice = "asc")
        {
            // Validate the input title
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title cannot be empty.");
            }

            // Query the database for books that match the title
            var booksQuery = bSFDbContext.Books
                .Where(b => b.Title.Contains(title))
                .AsQueryable();

            // Apply sorting if requested
            if (sortByPrice.ToLower() == "asc")
            {
                booksQuery = booksQuery.OrderBy(b => b.Price);
            }
            else if (sortByPrice.ToLower() == "desc")
            {
                booksQuery = booksQuery.OrderByDescending(b => b.Price);
            }

            // Execute the query and project the results to a DTO
            var books = await booksQuery.Select(b => new BookDTO
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
                Price = b.Price,
                Subject = b.Subject,
                QuantityAvailable = b.QuantityAvailable
            }).ToListAsync();

            // Return the result
            return Ok(books);
        }


    }
}
