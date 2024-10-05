namespace Store.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IUnitOfWork unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult GetAllProduct()
    {
        List<Product> products = unitOfWork.Product.GetAll().ToList();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        Product product = unitOfWork.Product.GetById(id);
        return Ok(product);
    }

    [HttpPost]
    public IActionResult AddProduct(Product product)
    {
        unitOfWork.Product.Add(product);
        unitOfWork.Save();
        return CreatedAtAction("GetProductById", new {id = product.Id},product);
    }


    [HttpPut]
    public IActionResult EditProduct(Product product)
    {
        unitOfWork.Product.Update(product);
        unitOfWork.Save();
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        Product product = unitOfWork.Product.GetById(id);
        unitOfWork.Product.Delete(product);
        unitOfWork.Save();
        return NoContent();
    }
}
