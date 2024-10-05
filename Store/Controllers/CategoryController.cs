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
            //List<Category> categories = unitOfWork.Category.GetAll(IncludeProperites: "Products").ToList();
            //GeneralResponse generalResponse = new GeneralResponse();
            //if (categories != null)
            //{
            //    List<CategoryWithListofProductsDTO> Categorieswithproductsname = new();
            //    foreach (Category category in categories)
            //    {
            //        CategoryWithListofProductsDTO categoryWith = new CategoryWithListofProductsDTO();
            //        categoryWith.Id = category.Id;
            //        categoryWith.Name = category.Name;
            //        if (category.Products != null)
            //        {
            //            foreach (var item in category.Products)
            //            {
            //                categoryWith.productNames.Add(item.Name);
            //            }
            //        }
            //        Categorieswithproductsname.Add(categoryWith);
            //    }

            //    generalResponse.IsSuccess = true;
            //    generalResponse.Data = Categorieswithproductsname;
            //}
            //else
            //{
            //    generalResponse.IsSuccess = false;
            //    generalResponse.Data = "The Data is invalid";
            //}
            //return generalResponse;
            var categories = unitOfWork.Category.GetAll(IncludeProperites: "Products").ToList();

            var generalResponse = new GeneralResponse();

            if (categories != null && categories.Any())
            {
                generalResponse.IsSuccess = true;
                generalResponse.Data = categories.Select(category => new CategoryWithListofProductsDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    productNames = category.Products?.Select(p => p.Name).ToList() ?? new List<string>()
                }).ToList();
            }
            else
            {
                generalResponse.IsSuccess = false;
                generalResponse.Data = "The Data is invalid"; // Since this is an error, you can return a string here
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
                foreach (var item in category.Products)
                {
                    categories.productNames.Add(item.Name);
                }
                generalResponse.IsSuccess = true;
                generalResponse.Data = categories;
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
    }
}
