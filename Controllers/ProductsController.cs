using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageWarehouseAPI.Data;
using BeverageWarehouseAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BeverageWarehouseAPI.Controllers
{
    /// <summary>
    /// Контролер за управление на продуктите.
    /// Поддържа CRUD операции за продуктите.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly BeverageWarehouseDbContext _context;

        /// <summary>
        /// Конструктор на контролера.
        /// </summary>
        /// <param name="context">DB Context за достъп до базата данни.</param>
        public ProductsController(BeverageWarehouseDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получава списък с продукти с възможност за странициране и търсене по име или марка.
        /// </summary>
        /// <param name="pageNumber">Номер на страницата (по подразбиране 1).</param>
        /// <param name="pageSize">Брой елементи на страница (по подразбиране 10).</param>
        /// <param name="search">Търсена стойност за филтриране по име или марка на продукта.</param>
        /// <returns>Списък от продукти.</returns>
        [HttpGet]
        public IActionResult GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            var products = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(products);
        }

        /// <summary>
        /// Получава конкретен продукт по идентификатор.
        /// </summary>
        /// <param name="id">Идентификатор на продукта.</param>
        /// <returns>Данните за продукта, ако съществува.</returns>
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Създава нов продукт.
        /// </summary>
        /// <param name="product">Обект, съдържащ данни за продукта.</param>
        /// <returns>Създаденият продукт с неговия идентификатор.</returns>
        [HttpPost]
        public IActionResult PostProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        /// <summary>
        /// Актуализира съществуващ продукт.
        /// </summary>
        /// <param name="id">Идентификатор на продукта, който се актуализира.</param>
        /// <param name="product">Обект, съдържащ актуализирани данни за продукта.</param>
        /// <returns>Статус код, указващ резултата от операцията.</returns>
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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
        /// Изтрива продукт по идентификатор.
        /// </summary>
        /// <param name="id">Идентификатор на продукта, който трябва да бъде изтрит.</param>
        /// <returns>Статус код, указващ резултата от операцията.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Проверява дали даден продукт съществува в базата данни.
        /// </summary>
        /// <param name="id">Идентификатор на продукта.</param>
        /// <returns>True ако продуктът съществува, в противен случай false.</returns>
        private bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }
    }
}
