using Microsoft.AspNetCore.Http.HttpResults;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpGet]
        public ActionResult<GeneralResponse> GetAllCategories()
        {
            List<Category> categories = unitOfWork.Category.GetAll(IncludeProperites: "Products").ToList();
            GeneralResponse generalResponse = new GeneralResponse();
            if (categories != null)
            {
                List<CategoryWithListofProductsDTO> Categorieswithproductsname = new();
                foreach (Category category in categories)
                {
                    CategoryWithListofProductsDTO categoryWith = new CategoryWithListofProductsDTO();
                    categoryWith.Id = category.Id;
                    categoryWith.Name = category.Name;
                    if (category.Products != null)
                    {
                        foreach (var item in category.Products)
                        {
                            categoryWith.productNames.Add(item.Name);
                        }
                    }
                    Categorieswithproductsname.Add(categoryWith);
                }

                generalResponse.IsSuccess = true;
                generalResponse.Data = Categorieswithproductsname;
            }
            else
            {
                generalResponse.IsSuccess = false;
                generalResponse.Data = "The Data is invalid";
            }
            return generalResponse;

        }

        [HttpGet("{id}")]
        public ActionResult<GeneralResponse> GetCategory(int id)
        {
            Category category = unitOfWork.Category.Get(e => e.Id == id, IncludeProperites: "Products");
            GeneralResponse generalResponse = new GeneralResponse();
            if (category != null)
            {
                CategoryWithListofProductsDTO categories = new();
                categories.Id = id;
                categories.Name = category.Name;
                if (category.Products != null)
                {
                    foreach (var item in category.Products)
                    {
                        categories.productNames.Add(item.Name);
                    }
                    generalResponse.IsSuccess = true;
                    generalResponse.Data = categories;
                }
            }
            else
            {
                generalResponse.IsSuccess = false;
                generalResponse.Data = "The Id is invalid";
            }
            return generalResponse;
        }
        [HttpPost]
        public ActionResult<GeneralResponse> AddCategory(Category category)
        {
            unitOfWork.Category.Add(category);
            unitOfWork.Save();
            GeneralResponse generalResponse = new GeneralResponse();
            generalResponse.IsSuccess = true;
            generalResponse.Data = category;
            return generalResponse;
        }
        [HttpPut]
        public ActionResult<GeneralResponse> EditCategory(CategoryUpdateDto category)
        {
            GeneralResponse generalResponse = new GeneralResponse();
            Category? categoryfromdb = unitOfWork.Category.GetById(category.Id);
            if (categoryfromdb != null)
            {
                categoryfromdb.Name = category.Name;
                unitOfWork.Category.Update(categoryfromdb);
                unitOfWork.Save();
                generalResponse.IsSuccess = true;
                generalResponse.Data = category;
                return Ok(generalResponse);
            }
            generalResponse.IsSuccess = false;
            generalResponse.Data = "the id is not valid";
            return NotFound(generalResponse);
        }
    }
}
