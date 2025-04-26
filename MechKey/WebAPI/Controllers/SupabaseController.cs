using Application.Interfaces.IApiClient.Supabase;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SupabaseController : ControllerBase
    {
        private readonly ISupabaseService _supabaseService;

        public SupabaseController(ISupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateImage([FromBody] UploadFileModel model)
        {
            var img = model.Base64String.Split("data:image/png;base64,");
            byte[] imageBytes = Convert.FromBase64String(img[1]);

            var result = await _supabaseService.UploadImage(imageBytes);

            return Ok(result);
        }

        public class UploadFileModel
        {
            public string Base64String { get; set; }
        }
    }
}
