using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageWarehouseAPI.Data;
using BeverageWarehouseAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BeverageWarehouseAPI.Controllers
{
    /// <summary>
    /// Контролер за управление на категориите.
    /// Поддържа CRUD операции за категориите.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly BeverageWarehouseDbContext _context;

        /// <summary>
        /// Конструктор на контролера.
        /// </summary>
        /// <param name="context">DB Context за достъп до базата данни.</param>
        public CategoriesController(BeverageWarehouseDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получава списък с категории с възможност за странициране и търсене по име.
        /// </summary>
        /// <param name="pageNumber">Номер на страницата (по подразбиране 1).</param>
        /// <param name="pageSize">Брой елементи на страница (по подразбиране 10).</param>
        /// <param name="search">Търсена стойност за филтриране по име на категория.</param>
        /// <returns>Списък от категории.</returns>
        [HttpGet]
        public IActionResult GetCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search));
            }

            var categories = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(categories);
        }

        /// <summary>
        /// Получава конкретна категория по идентификатор.
        /// </summary>
        /// <param name="id">Идентификатор на категорията.</param>
        /// <returns>Данните за категорията, ако съществува.</returns>
        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        /// <summary>
        /// Създава нова категория.
        /// </summary>
        /// <param name="category">Обект, съдържащ данни за категорията.</param>
        /// <returns>Създадената категория с нейния идентификатор.</returns>
        [HttpPost]
        public IActionResult PostCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        /// <summary>
        /// Актуализира съществуваща категория.
        /// </summary>
        /// <param name="id">Идентификатор на категорията, която се актуализира.</param>
        /// <param name="category">Обект, съдържащ актуализирани данни за категорията.</param>
        /// <returns>Статус код, указващ резултата от операцията.</returns>
        [HttpPut("{id}")]
        public IActionResult PutCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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
        /// Изтрива категория по идентификатор.
        /// </summary>
        /// <param name="id">Идентификатор на категорията, която трябва да бъде изтрита.</param>
        /// <returns>Статус код, указващ резултата от операцията.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Проверява дали дадена категория съществува в базата данни.
        /// </summary>
        /// <param name="id">Идентификатор на категорията.</param>
        /// <returns>True ако категорията съществува, в противен случай false.</returns>
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
