using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Models.DTOS.Cart;
using Store.Models.DTOS.OrderDetail;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        private string GetUserId()
        {
            var accountid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return accountid;
        }
        [HttpPost]
        public ActionResult AddToCart(AddToCartDTO cartDto)
        {
            var product = unitOfWork.Product.GetById(cartDto.productId);
            if (product == null)
            {
                return NotFound("this product id is invalid");
            }
            var cartfromDb = unitOfWork.ShoppingCart.Get(e => e.UserId == GetUserId() && e.productId == cartDto.productId);
            if (cartfromDb != null)
            {
                cartfromDb.Quantity += cartDto.Quantity;
            }
            else
            {
                ShoppingCart cart = new ShoppingCart();
                cart.productId = cartDto.productId;
                cart.Quantity = cartDto.Quantity;
                cart.UserId = GetUserId();
                unitOfWork.ShoppingCart.Add(cart);
            }
            unitOfWork.Save();
            GeneralResponse response = new GeneralResponse();
            response.IsSuccess = true;
            response.Data = "the Product is added succesfully";
            return Ok(response);
        }
        [HttpGet]
        public ActionResult GetCartItems()
        {
            var userId = GetUserId();
            
            var cart = unitOfWork.ShoppingCart.GetAll(e=>e.UserId == userId,IncludeProperites:"Product").ToList();
            
            if(cart == null)
            {
                return NotFound("Your Cart is empty");
            }
            else
            {
                List<Orderdetaildto> CartItems = new List<Orderdetaildto>();
                
                foreach(var item in cart)
                {
                    Orderdetaildto cartdto = new Orderdetaildto()
                    {
                        ProductId = item.productId,
                        Price = item.Product.Price,
                        Quantity = item.Quantity
                    };
                    CartItems.Add(cartdto);
                }
                
                return Ok(CartItems);
            }
        }
        [HttpDelete("{ProductId}")]
        public ActionResult<GeneralResponse> RemoveFromcart(int ProductId)
        {
            var product = unitOfWork.Product.GetById(ProductId);
            if (product == null)
            {
                return NotFound("this product id is invalid");
            }
            var userId = GetUserId();   
            
            var cart = unitOfWork.ShoppingCart.Get(e=>e.productId == ProductId && e.UserId== userId);
            
            if (cart == null)
            {
                return NotFound("this product is not in the cart");
            }

            if (cart.Quantity > 1)
                cart.Quantity -= 1;
            else
                unitOfWork.ShoppingCart.Delete(cart);
            unitOfWork.Save();

            return Ok("Product removed from the cart");
        }
        [HttpPut]
        public ActionResult UpdateCartItem (AddToCartDTO cartitem)
        {
            var product = unitOfWork.Product.GetById(cartitem.productId);
            if (product == null)
            {
                return NotFound("this product id is invalid");
            }
            var userId = GetUserId();
            var cart = unitOfWork.ShoppingCart.Get(e=>e.productId == cartitem.productId &&e.UserId == userId);
            if(cart == null)
                 return NotFound("cart not found");
            cart.Quantity = cartitem.Quantity;
            unitOfWork.Save();
            return Ok("cart updated succesfuly");
        }
    }
}
