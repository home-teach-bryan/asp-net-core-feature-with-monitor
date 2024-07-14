using AspNetCoreFeatureWithMonitor.Models.Request;
using AspNetCoreFeatureWithMonitor.Models.Response;

namespace AspNetCoreFeatureWithMonitor.Services;

public interface IProductService
{
    public bool AddProduct(AddProductRequest product);

    public bool UpdateProduct(Guid id, UpdateProductRequest product);

    public bool RemoveProduct(Guid id);
    
    public List<GetProductResponse> GetAllProducts();

    public GetProductResponse GetProduct(Guid id);
}