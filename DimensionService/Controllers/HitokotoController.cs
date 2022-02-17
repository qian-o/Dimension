using DimensionService.Models;
using DimensionService.Service.Hitokoto;
using Microsoft.AspNetCore.Mvc;

namespace DimensionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HitokotoController : ControllerBase
    {
        private readonly IHitokotoService _hitokoto;

        public HitokotoController(IHitokotoService hitokoto)
        {
            _hitokoto = hitokoto;
        }

        /// <summary>
        /// 获取一言
        /// </summary>
        /// <returns></returns>
        [Route("GetHitokoto")]
        [HttpGet]
        public WebResultModel GetHitokoto()
        {
            WebResultModel webResult = new()
            {
                State = true,
                Data = _hitokoto.GetHitokoto(),
                Message = "获取成功"
            };
            return webResult;
        }
    }
}
