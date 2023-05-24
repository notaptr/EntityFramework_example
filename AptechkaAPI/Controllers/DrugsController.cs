using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AptechkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrugsController : ControllerBase
    {
        private readonly AptechkaContext _context;

        public DrugsController(AptechkaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Запрос всех лекарств, имеющихся в базе
        /// </summary>
        /// <returns></returns>
        // GET: api/Drugs
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Drug>>> GetDrugs()
        {
            if (_context.Drugs == null)
            {
                return NotFound();
            }

            Task<List<Drug>> drugs = _context.Drugs
                .Join(_context.Producers, d => d.ProducerId, p => p.Id, (d, p) => new Drug() { 
                    Id = d.Id, 
                    Name = d.Name, 
                    Price = d.Price,
                    ProducerId = d.ProducerId,
                    DateOfManufacture = d.DateOfManufacture,
                    BestBeforeDate = d.BestBeforeDate,
                    ProducerName = p.Name
                })
                .ToListAsync();

            return await drugs;
        }

        /// <summary>
        /// Запрос лекарства по идентификатору
        /// </summary>
        /// <param name="id">идентификатор лекарства</param>
        /// <returns></returns>
        // GET: api/Drugs/5
        [HttpGet("GetById")]
        public async Task<ActionResult<Drug>> GetDrug(int id)
        {
            if (_context.Drugs == null)
            {
                return NotFound();
            }
            var drug = await _context.Drugs.FindAsync(id);

            if (drug == null)
            {
                return NotFound();
            }

            Producer? prd = await _context.Producers.FindAsync(drug.ProducerId);
            if (prd != null) { drug.ProducerName = prd.Name; }

            return drug;
        }

        private bool DrugExists(int id)
        {
            return (_context.Drugs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
