using Microsoft.AspNetCore.Mvc;

namespace YDeveloper.Controllers.Api
{
    [Route("api/session")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        [HttpPost("ping")]
        public IActionResult Ping()
        {
            // Bu endpoint sadece session'ı yenilemek için kullanılır
            // Herhangi bir işlem yapmaya gerek yok, istek geldiğinde session otomatik yenilenir
            return Ok(new { success = true, message = "Session renewed" });
        }
    }
}
