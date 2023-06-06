using DoAn4.Services.SearchService;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService) 
        {
            _searchService = searchService;
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody] string keyword)
        { 
            var result = await _searchService.Search(keyword);
            if (result == null) 
            {
                return NotFound();
            }
            return Ok(result);

        }
    }
}
