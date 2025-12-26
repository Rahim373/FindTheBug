using FindTheBug.WebAPI.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[BasicAuth]
public class DataSyncController : BaseApiController
{
    
}
