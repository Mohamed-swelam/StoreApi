using Store.Models.DTOS.Product;

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
    public ActionResult<GeneralResponse> GetAllProduct()
    {
        GeneralResponse response = new GeneralResponse();
        List<ProductWithCategoryNameDTO> productsDto = new List<ProductWithCategoryNameDTO>();
        List<Product> products = unitOfWork.Product.GetAll(IncludeProperites: "Category").ToList();
        foreach (var item in products)
        {
            ProductWithCategoryNameDTO product = new ProductWithCategoryNameDTO();
            product.Id = item.Id;
            product.Name = item.Name;
            product.Description = item.Description;
            product.Price = item.Price;
            product.Quantity = item.Quantity;
            if (item.Category != null)
                product.CategoryName = item.Category.Name;
            productsDto.Add(product);
        }
        response.IsSuccess = true;
        response.Data = productsDto;
        return Ok(response);
    }

    [HttpGet("{id}")]
    public ActionResult<GeneralResponse> GetProductById(int id)
    {
        GeneralResponse generalResponse = new GeneralResponse();
        Product product = unitOfWork.Product.Get(i => i.Id == id, IncludeProperites: "Category");
        if (product != null)
        {
            ProductWithCategoryNameDTO productDto = new ProductWithCategoryNameDTO();
            productDto.Id = id;
            productDto.Name = product.Name;
            productDto.Description = product.Description;
            productDto.Price = product.Price;
            productDto.Quantity = product.Quantity;
            if (product.Category != null)
                productDto.CategoryName = product.Category.Name;
            generalResponse.IsSuccess = true;
            generalResponse.Data = productDto;
            return Ok(generalResponse);
        }
        generalResponse.IsSuccess = false;
        generalResponse.Data = "the id is not valid";
        return NotFound(generalResponse);
    }

    [HttpPost]
    public IActionResult AddProduct(ProductTobeaddedDTO product)
    {
        if (product != null)
        {
            Product newproduct = new Product();
            newproduct.Name = product.Name;
            newproduct.Description = product.Description;
            newproduct.Price = product.Price;
            newproduct.Quantity = product.Quantity;
            newproduct.CategoryId = product.CategoryId;
            unitOfWork.Product.Add(newproduct);
            unitOfWork.Save();
            return CreatedAtAction("GetProductById", new { id = newproduct.Id }, product);
        }
        return BadRequest();
    }

    [HttpPut]
    public IActionResult EditProduct(ProductTobeupdatedDTO productdto)
    {
        var productfromdb = unitOfWork.Product.Get(i => i.Id == productdto.Id);
        if (productfromdb != null)
        {
            productfromdb.Name = productdto.Name;
            productfromdb.Price = productdto.Price;
            productfromdb.Quantity = productdto.Quantity;
            productfromdb.Description = productdto.Description;
            productfromdb.CategoryId = productdto.CategoryId;
            unitOfWork.Save();
            return Ok(productdto);
        }
        return NotFound();
    }

    [HttpDelete("{id}")]
    public ActionResult<GeneralResponse> DeleteProduct(int id)
    {
        GeneralResponse response = new GeneralResponse();
        Product? product = unitOfWork.Product.GetById(id);
        if (product != null)
        {
            unitOfWork.Product.Delete(product);
            unitOfWork.Save();
            response.IsSuccess = true;
            response.Data = "the product is deleted succesfuly";
            return Ok(response);
        }
        response.IsSuccess = false;
        response.Data = "invalid id";
        return NotFound(response);
    }

    [HttpGet("Filtering")]
    public ActionResult Filtering([FromQuery]ProductFilteringDTO ProductDto)
    {
        var productsFromDb= unitOfWork.Product.GetAll(IncludeProperites:"Category").ToList();
        if (ModelState.IsValid) 
        {
            if(ProductDto.CategoryName != null)
                productsFromDb = unitOfWork.Product.GetAll(e=>e.Category.Name == ProductDto.CategoryName
                ,IncludeProperites:"Category").ToList();

            if (ProductDto.minPrice != 0)
                productsFromDb = unitOfWork.Product.GetAll(e => e.Price > ProductDto.minPrice,IncludeProperites:"Category").ToList();

            if (ProductDto.maxPrice != 0)
                productsFromDb = unitOfWork.Product.GetAll(e => e.Price < ProductDto.maxPrice, IncludeProperites: "Category").ToList();

            if (ProductDto.KeyWord != null)
                productsFromDb = unitOfWork.Product.GetAll(e => e.Name.Contains(ProductDto.KeyWord), IncludeProperites: "Category").ToList();

            if (productsFromDb.Any())
            {
               List<ProductWithCategoryNameDTO> products = new List<ProductWithCategoryNameDTO>();
                foreach(var item in productsFromDb)
                {
                    ProductWithCategoryNameDTO product = new();
                    product.Name = item.Name;
                    product.Price = item.Price;
                    product.Description = item.Description;
                    product.CategoryName = item.Category.Name;
                    product.Price = item.Price;
                    product.Quantity = item.Quantity;
                    products.Add(product);
                }
                return Ok(products);
            }
            return NoContent();
        }
        return BadRequest();

    }
}
