using DimensionService.Filter.Authorized;
using Microsoft.AspNetCore.Mvc;

namespace DimensionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedActionFilter]
    public class CallController : ControllerBase
    {
    }
}
