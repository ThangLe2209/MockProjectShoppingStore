using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticRepository _statisticRepository;
        private readonly IMapper _mapper;
        const int maxStatisticsPageSize = 4;
        public StatisticsController(IStatisticRepository statisticRepository, IMapper mapper)
        {
            _statisticRepository = statisticRepository ?? throw new ArgumentNullException(nameof(statisticRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var statisticEntities = await _statisticRepository.GetStatisticsAsync();
            //var result = _mapper.Map<IEnumerable<BrandDto>>(brandEntities);
            return Ok(statisticEntities);
        }

        [HttpGet("getByStartEndDate")]
        public async Task<IActionResult> GetStatistics(DateTime startDate, DateTime endDate)
        {
            var statisticEntities = await _statisticRepository.GetStatisticsByStartEndDateAsync(startDate, endDate);
            //var result = _mapper.Map<IEnumerable<BrandDto>>(brandEntities);
            return Ok(statisticEntities);
        }
    }
}
