using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Store.Models.DTOS.Cart;
using Store.Models.DTOS.Order;
using Store.Models.DTOS.OrderDetail;
using Stripe.Checkout;
using Stripe;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private string GetUserId()
        {
            var accountid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return accountid;
        }
        [Authorize]
        [HttpPost("CreateOrder")]
        public ActionResult CreateOrder()
        {
            var carts = unitOfWork.ShoppingCart.GetAll(e => e.UserId == GetUserId()
            , IncludeProperites: "Product").ToList();

            var Neworder = new Order()
            {
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                UserId = GetUserId(),
                TotalPrice = carts.Sum(e => e.Product.Price * e.Quantity)
            };

            unitOfWork.Order.Add(Neworder);
            unitOfWork.Save();

            foreach (var cart in carts)
            {
                var orderdetail = new OrderDetail()
                {
                    OrderId = Neworder.Id,
                    Price = cart.Product.Price,
                    ProductId = cart.productId,
                    Quantity = cart.Quantity
                };
                unitOfWork.OrderDetail.Add(orderdetail);
            }
            unitOfWork.ShoppingCart.RemoveRange(carts);
            unitOfWork.Save();

            return RedirectToAction("StartStripePayment", new { orderId = Neworder.Id });
        }

        [HttpGet]
        [Authorize]
        public ActionResult StartStripePayment(int orderId)
        {
            Order? order = unitOfWork.Order.Get(e => e.Id == orderId, IncludeProperites: "OrderDetails,OrderDetails.Product");

            if (order == null)
                return BadRequest("there is no order");

            var domain = "http://localhost:5064";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"/api/Order/PaymentConfirmation/{order.Id}",
                CancelUrl = domain + $"/api/Order/GetOrderDetails/{order.Id}",
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", order.Id.ToString() }
                }
            };


            foreach (var cart in order.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.Name
                        }
                    },
                    Quantity = cart.Quantity
                };

                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            order.sessionId = session.Id;
            unitOfWork.Save();
            return Ok(session.Url);
        }

        [HttpPost("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    "whsec_07a0d2d0f39509cf21dea2634a3635451e15562b9ccf4626a4adf73ddc7b0efa",
                    throwOnApiVersionMismatch: false
                );

                
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var orderId = int.Parse(session.Metadata["OrderId"]); 
                    
                    var order = unitOfWork.Order.Get(o => o.Id == orderId);
                    if (order != null)
                    {
                        order.OrderStatus = "Paid";
                        unitOfWork.Order.Update(order);
                        unitOfWork.Save();
                    }
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Stripe error: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return BadRequest(new { error = "Something went wrong." });
            }
        }
        [Authorize]
        [HttpGet("My-Orders")]
        public ActionResult GetUserOrders()
        {
            var orders = unitOfWork.Order.GetAll(e => e.UserId == GetUserId(), IncludeProperites: "OrderDetails");

            List<OrderDto> ordersdto = new List<OrderDto>();
            foreach (var order in orders)
            {
                List<Orderdetaildto>? orderdetails = new List<Orderdetaildto>();

                if (order.OrderDetails != null)
                {
                    foreach (var item in order.OrderDetails)
                    {
                        Orderdetaildto orderdetaildto = new Orderdetaildto()
                        {
                            OrderId = order.Id,
                            Price = item.Price,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        };
                        orderdetails.Add(orderdetaildto);
                    }
                }

                OrderDto orderDto = new OrderDto()
                {
                    OrderDate = order.OrderDate,
                    OrderId = order.Id,
                    OrderStatus = order.OrderStatus,
                    TotalPrice = order.TotalPrice,
                    OrderDetails = orderdetails
                };

                ordersdto.Add(orderDto);
            }

            return Ok(ordersdto);
        }
        [Authorize]
        [HttpPut("Cancel/{orderId}")]
        public ActionResult CancelOrder(int orderId)
        {
            var order = unitOfWork.Order.Get(e => e.Id == orderId && e.UserId == GetUserId());

            if (order == null)
                return BadRequest("No order With This Id");
            if (order.OrderStatus != "Pending")
                return BadRequest("Sorry your order can not be canceled");

            order.OrderStatus = "Canceled";
            unitOfWork.Save();
            return Ok("Your order Is Deleted succefuly");
        }

        [HttpGet("{orderId}")]
        [Authorize]
        public ActionResult GetOrderDetails(int orderId)
        {
            var order = unitOfWork.Order.Get(e => e.Id == orderId && e.UserId == GetUserId());

            List<Orderdetaildto> Orderdetails = new List<Orderdetaildto>();
            if (order.OrderDetails == null)
                return NotFound("there is no orderdetails");

            foreach (var item in order.OrderDetails)
            {
                Orderdetaildto orderDto = new Orderdetaildto();
                orderDto.Price = item.Price;
                orderDto.OrderId = item.OrderId;
                orderDto.ProductId = item.ProductId;
                orderDto.Quantity = item.Quantity;

                Orderdetails.Add(orderDto);
            }
            return Ok(Orderdetails);
        }

        [HttpPost("PaymentConfirmation/{orderId}")]
        public ActionResult PaymentConfirmation(int orderId)
        {
            
            var order = unitOfWork.Order.Get(e => e.Id == orderId, IncludeProperites: "OrderDetails");
            if (order == null)
            {
                return NotFound();
            }
            var service = new SessionService();
            Session session = service.Get(order.sessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                if (order.OrderStatus != "Paid")
                {
                    order.OrderStatus = "Paid";
                    unitOfWork.Save();
                }
                return Ok(new
                {
                    Message = "Payment completed successfully",
                    OrderId = order.Id,
                    OrderTotal = order.TotalPrice
                });
            }
            else
            {
                return BadRequest(new { Message = "Payment not completed", OrderId = order.Id });
            }
        }
    }
}
