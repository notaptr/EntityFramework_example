using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AptechkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly AptechkaContext _context;

        public BasketController(AptechkaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Выбор всех Корзин по данной Аптеке
        /// </summary>
        /// <param name="drugstoreId">идентификатор аптеки</param>
        /// <returns>Возвращает список всех найденных корзин и содержимое</returns>
        // GET: api/Baskets
        [HttpGet("GetByDrugstore")]
        public async Task<ActionResult<IEnumerable<Basket>>> GetBaskets(int drugstoreId)
        {
            var requests = await _context.Requests
                .Where(r => r.DrugstoreId == drugstoreId && r.StatusId == 1)
                .Select(r => new Basket()
                {
                    Id = r.Id,
                    DrugstoreId = drugstoreId,
                    Rows = _context.Purchases
                        .Where(p => p.IdRequests == r.Id)
                        .Select(row => new BasketRow() { Id = row.Id, DrugId = (int)row.IdDrugs!, Count = (int)row.Count! }).ToList()
                })
                .ToListAsync();

            if (requests.Count == 0)
            {
                return NotFound();
            }

            return requests;
        }

        /// <summary>
        /// Получение корзины по идентификатору
        /// </summary>
        /// <param name="basketId">Идентификатор корзины</param>
        /// <returns>Возвращает найденную корзину или ошибку</returns>
        // GET: api/Baskets/5
        [HttpGet("GetbyId")]
        public async Task<ActionResult<Basket>> GetOneBasket(int basketId)
        {
            var requests = await _context.Requests
                .Where(r => r.Id == basketId && r.StatusId == 1)
                .Select(r => new Basket()
                {
                    Id = r.Id,
                    DrugstoreId = (int)r.DrugstoreId!,
                    Rows = _context.Purchases
                        .Where(p => p.IdRequests == r.Id)
                        .Select(row => new BasketRow() { Id = row.Id, DrugId = (int)row.IdDrugs!, Count = (int)row.Count! }).ToList()
                })
                .ToListAsync();

            if (requests.Count == 0)
            {
                return NotFound();
            }

            return requests[0];
        }

        /// <summary>
        /// Запись внесённых в корзину изменений.
        /// </summary>
        /// <param name="id">Идентификатор корзины</param>
        /// <param name="basket">Корзина</param>
        /// <returns></returns>
        // PUT: api/Baskets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update")]
        public async Task<IActionResult> PutBasket(int id, Basket basket)
        {
            if (id != basket.Id)
            {
                return BadRequest();
            }

            var basraw = await _context.Purchases
                .Where(p => p.IdRequests == id)
                .ToListAsync();

            _context.Purchases.RemoveRange(basraw);

            List<Purchase> purchases = new List<Purchase>();

            foreach (BasketRow br in basket.Rows)
            {
                bool ok = false;

                ok = DrugExist(br.DrugId);
                ok &= br.Count > 0;

                if (!ok) { return BadRequest(); }

                purchases.Add(new Purchase() { IdRequests = id, IdDrugs = br.DrugId, Count = br.Count });
            }

            await _context.Purchases.AddRangeAsync(purchases);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Добавление новой корзины в базу.
        /// </summary>
        /// <param name="basket">Корзина</param>
        /// <returns></returns>
        // POST: api/Baskets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateBasket")]
        public async Task<ActionResult<Basket>> PostBasket(Basket basket)
        {

            
            if (basket == null)
            {
                return BadRequest();
            }

            if (!DrugstoreExist(basket.DrugstoreId))
            {
                return BadRequest();
            }

            Request req = new Request() { DateIn = DateTime.Now, StatusId = 1, DrugstoreId = basket.DrugstoreId };
            _context.Requests.Add(req);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            List<Purchase> purchases = new List<Purchase>();

            foreach (BasketRow br in basket.Rows)
            {
                bool ok = false;

                ok = DrugExist(br.DrugId);
                ok &= br.Count > 0;

                if (!ok) { BadRequest(); }

                purchases.Add(new Purchase() { IdRequests = req.Id, IdDrugs = br.DrugId, Count = br.Count });
            }

            await _context.Purchases.AddRangeAsync(purchases);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            //return CreatedResult;
            //return CreatedAtAction("GetOneBasket", new { id = req.Id }, req);
            return StatusCode(201);
        }

        /// <summary>
        /// Удаление корзины.
        /// </summary>
        /// <param name="id">Идентификатор корзины</param>
        /// <returns></returns>
        // DELETE: api/Baskets/5
        [HttpDelete("DeleteBasket")]
        public async Task<IActionResult> DeleteBasket(int id)
        {

           if (!(_context.Requests?.Any(e => e.Id == id)).GetValueOrDefault())
            {
                return BadRequest();
            }

            var basraw = await _context.Purchases
                .Where(p => p.IdRequests == id)
                .ToListAsync();

            _context.Purchases.RemoveRange(basraw);

            Request? req = _context.Requests!.Find(id);

            if (req != null)
            {
                _context.Requests.Remove(req);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Функция проверяет существование медикамента
        /// </summary>
        /// <param name="id">Идентификатор медикамента</param>
        /// <returns>True - если есть. Иначе - False</returns>
        private bool DrugExist(int id)
        {
            return (_context.Drugs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Функция проверяет существование аптеки
        /// </summary>
        /// <param name="id">Идентификатор аптеки</param>
        /// <returns>True - если есть. Иначе - False</returns>
        private bool DrugstoreExist(int id)
        {
            return (_context.Drugstores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
