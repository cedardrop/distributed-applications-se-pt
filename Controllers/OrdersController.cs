using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageWarehouseAPI.Data;
using BeverageWarehouseAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BeverageWarehouseAPI.Controllers
{
    /// <summary>
    /// Контролер за управление на поръчките.
    /// Поддържа CRUD операции за поръчките.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly BeverageWarehouseDbContext _context;

        /// <summary>
        /// Конструктор на контролера.
        /// </summary>
        /// <param name="context">DB Context за достъп до базата данни.</param>
        public OrdersController(BeverageWarehouseDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получава списък с поръчки с възможност за странициране и търсене по номер на поръчка или име на клиент.
        /// </summary>
        /// <param name="pageNumber">Номер на страницата (по подразбиране 1).</param>
        /// <param name="pageSize">Брой елементи на страница (по подразбиране 10).</param>
        /// <param name="search">Търсена стойност за филтриране по номер на поръчка или име на клиент.</param>
        /// <returns>Списък от поръчки.</returns>
        [HttpGet]
        public IActionResult GetOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.OrderNumber.Contains(search) || o.CustomerName.Contains(search));
            }

            var orders = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(orders);
        }

        /// <summary>
        /// Получава конкретна поръчка по идентификатор.
        /// </summary>
        /// <param name="id">Идентификатор на поръчката.</param>
        /// <returns>Данните за поръчката, ако съществува.</returns>
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        /// <summary>
        /// Създава нова поръчка.
        /// </summary>
        /// <param name="order">Обект съдържащ данни за поръчката.</param>
        /// <returns>Създадената поръчка с нейния идентификатор.</returns>
        [HttpPost]
        public IActionResult PostOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        /// <summary>
        /// Актуализира съществуваща поръчка.
        /// </summary>
        /// <param name="id">Идентификатор на поръчката, която се актуализира.</param>
        /// <param name="order">Обект съдържащ актуализирани данни за поръчката.</param>
        /// <returns>Статус код, указващ резултата от операцията.</returns>
        [HttpPut("{id}")]
        public IActionResult PutOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Изтрива поръчка по идентификатор.
        /// </summary>
        /// <param name="id">Идентификатор на поръчката, която трябва да бъде изтрита.</param>
        /// <returns>Статус код, указващ резултата от операцията.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Проверява дали дадена поръчка съществува в базата данни.
        /// </summary>
        /// <param name="id">Идентификатор на поръчката.</param>
        /// <returns>True ако поръчката съществува, в противен случай false.</returns>
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(o => o.OrderId == id);
        }
    }
}
