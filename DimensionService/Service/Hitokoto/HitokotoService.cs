using DimensionService.Common;
using DimensionService.Models.ResultModels;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace DimensionService.Service.Hitokoto
{
    public class HitokotoService : IHitokotoService
    {
        public HitokotoModel GetHitokoto()
        {
            ClassHelper.Cache.TryGetValue("Hitokotos", out List<HitokotoModel> hitokotos);
            return hitokotos[new Random().Next(0, hitokotos.Count)];
        }
    }
}
