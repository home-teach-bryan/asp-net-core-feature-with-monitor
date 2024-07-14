using System.Security.Claims;
using AspNetCoreFeatureWithMonitor.Models.Request;
using AspNetCoreFeatureWithMonitor.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFeatureWithMonitor.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// 訂單控制器
    /// </summary>
    /// <param name="orderService"></param>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }


    /// <summary>
    /// 成立訂單
    /// </summary>
    /// <returns>回傳執行狀態</returns>
    [HttpPost]
    [Route("")]
    public IActionResult AddOrder([FromBody] List<AddOrderRequest> addOrderRequest)
    {
        var userId = base.HttpContext.User.Claims.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest("找不到使用者ID");
        }
        var isSuccess = _orderService.AddOrder(addOrderRequest, Guid.Parse(userId.Value));
        if (!isSuccess)
        {
            return BadRequest("成立訂單失敗");
        }
        return Ok(isSuccess);

    }

    [HttpGet]
    [Route("/OrderDetails")]
    public IActionResult GetOrderDetails()
    {
        var userId = base.HttpContext.User.Claims.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest("找不到使用者ID");
        }
        var orderDetails = _orderService.GetOrderDetails(Guid.Parse(userId.Value));
        return Ok(orderDetails);

    }
}