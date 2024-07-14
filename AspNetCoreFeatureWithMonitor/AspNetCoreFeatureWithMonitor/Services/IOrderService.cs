using AspNetCoreFeatureWithMonitor.Models.Request;
using AspNetCoreFeatureWithMonitor.Models.Response;

namespace AspNetCoreFeatureWithMonitor.Services;

public interface IOrderService
{
    bool AddOrder(List<AddOrderRequest> addOrderRequests, Guid userId);
    IEnumerable<GetOrderDetailsResponse> GetOrderDetails(Guid userId);
}