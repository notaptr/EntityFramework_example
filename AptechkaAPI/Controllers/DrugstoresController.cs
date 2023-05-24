using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AptechkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrugstoresController : ControllerBase
    {
        private readonly AptechkaContext _context;

        public DrugstoresController(AptechkaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Запрос всех аптек, имеющихся в базе
        /// </summary>
        /// <returns></returns>
        // GET: api/Drugstores
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Drugstore>>> GetDrugstores()
        {
          if (_context.Drugstores == null)
          {
              return NotFound();
          }
            return await _context.Drugstores.Select(d => toDTO(d)).ToListAsync();
        }

        /// <summary>
        /// Запрос аптеки по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор аптеки</param>
        /// <returns></returns>
        // GET: api/Drugstores/5
        [HttpGet("GetById")]
        public async Task<ActionResult<Drugstore>> GetDrugstore(int id)
        {
          if (_context.Drugstores == null)
          {
              return NotFound();
          }

            var drugstore = await _context.Drugstores.FindAsync(id);

            if (drugstore == null)
            {
                return NotFound();
            }

            return toDTO(drugstore);
        }

        /// <summary>
        /// Преобразование аптеки к сокращённому виду в целях
        /// защиты информации. Смысла делать отдельный контроллер нет,
        /// в приложении отсутствует авторизация.
        /// </summary>
        /// <param name="drg">Аптека</param>
        /// <returns></returns>
        private static Drugstore toDTO(Drugstore drg) =>
            new Drugstore { Id = drg.Id, Name = drg.Name };

    }
}
