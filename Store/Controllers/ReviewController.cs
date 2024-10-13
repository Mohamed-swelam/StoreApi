using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Models.DTOS.Review;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        private string GetUserId()
        {
            var accountid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return accountid;
        }
        [HttpPost()]
        public ActionResult<GeneralResponse> AddReview(ReviewDTO reviewDTO)
        {
            GeneralResponse response = new GeneralResponse();
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                var reviewfromdb = unitOfWork.Review.Get(e => e.ProductId == reviewDTO.ProductId && e.UserId == userId);
                if (reviewfromdb == null)
                {
                    var review = new Review()
                    {
                        UserId = userId,
                        ProductId = reviewDTO.ProductId,
                        Comment = reviewDTO.Comment,
                        CreatedDate = DateTime.Now,
                        Rating = reviewDTO.Rating,
                    };
                    unitOfWork.Review.Add(review);
                    unitOfWork.Save();
                    response.IsSuccess = true;
                    response.Data = "review added succesfuly";
                    return Ok(response);
                }
                response.IsSuccess = false;
                response.Data = "You have a review in this product already";
                return Ok(response);
            }
            return BadRequest(ModelState);
        }
        [AllowAnonymous]
        [HttpGet("{productId}")]
        public ActionResult GetReviewsForProduct(int productId)
        {
            var product = unitOfWork.Product.Get(e => e.Id == productId);

            if (product == null)
                return BadRequest();

            var reviews = unitOfWork.Review.GetAll(e => e.ProductId == productId
                                    , IncludeProperites: "User").ToList();

            if (reviews.Count == 0)
                return NoContent();

            List<GetReviewDTO> result = new List<GetReviewDTO>();
            foreach (var item in reviews)
            {
                var review = new GetReviewDTO()
                {
                    Comment = item.Comment,
                    Rating = item.Rating,
                    UserName = item.User.UserName
                };
                result.Add(review);
            }
            return Ok(result);
        }

        [HttpPut]
        public ActionResult<GeneralResponse> UpdateReview(ReviewDTO reviewDTO)
        {
            GeneralResponse response = new GeneralResponse();
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                
                Review? reviewFromDb = unitOfWork.Review.Get(e => e.ProductId == reviewDTO.ProductId && e.UserId == userId);
                if (reviewFromDb == null)
                {
                    response.IsSuccess = false;
                    response.Data = "There is no review for this product";
                    return Ok(response);
                }
                reviewFromDb.CreatedDate = DateTime.Now;
                reviewFromDb.UserId = userId;
                reviewFromDb.ProductId = reviewDTO.ProductId;
                reviewFromDb.Comment = reviewDTO.Comment;
                reviewFromDb.Rating = reviewDTO.Rating;
                unitOfWork.Review.Update(reviewFromDb);
                unitOfWork.Save();
                response.IsSuccess = true;
                response.Data = "Review Updated Succesfuly";
                return Ok(response);
            }
            response.IsSuccess = false;
            response.Data = ModelState.Values;
            return BadRequest(response);
        }
        [HttpDelete("{ProductId}")]
        public ActionResult Delete(int ProductId)
        {
            var product = unitOfWork.Product.Get(e=>e.Id == ProductId);
            if (product == null)
                return NotFound();

            var userId = GetUserId();

            var review = unitOfWork.Review.Get(e=>e.ProductId == ProductId && e.UserId ==userId);
            
            if(review == null)
                return NotFound();

            unitOfWork.Review.Delete(review);
            unitOfWork.Save();
            return Ok("Product deleted successfuly");
        }

    }
}
