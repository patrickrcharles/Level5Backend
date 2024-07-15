using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using level5Server.Models;
using level5Server.Models.level5;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;
using Microsoft.AspNetCore.Cors;

namespace level5Server.Controllers
{
    [EnableCors("ApiCors")]
    [Route("api/servermessages")]
    [ApiController]
    public class ServerMessagesApi : Controller
    {
        private readonly Level5Context _context;
        public ServerMessagesApi(Level5Context context)
        {
            _context = context;
        }

        //--------------------- HTTP GET ---------------------------------------------------

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<ServerMessage>>> GetAllVersions()
        {
            return await _context.ServerMessages.OrderByDescending(x => x.Id).Take(5).ToListAsync();
        }  
    }
}

