using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Specifications;


namespace API.Controllers;
[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unit) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
    {
        var email = User.GetEmail();
        var cart = await cartService.GetCartAsync(orderDto.CartId);
        if (cart == null) return BadRequest("Cart not found");
        if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order");
        var items = new List<OrderItem>();
        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return BadRequest($"Product with id {item.ProductId} not found");
            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = productItem.Name,
                PictureUrl = productItem.PictureUrl
            };  
            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };
            items.Add(orderItem);
        }   
        var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
        if (deliveryMethod == null) return BadRequest($"Delivery method with id {orderDto.DeliveryMethodId} not found");
        var order = new Order
        {
            BuyerEmail = email,
            ShippingAddress = orderDto.ShippingAddress,
            DeliveryMethod = deliveryMethod,
            OrderItems = items,
            PaymentSummary = orderDto.PaymentSummary,
            Subtotal = items.Sum(i => i.Price * i.Quantity),
            PaymentIntentId = cart.PaymentIntentId
        };  
        unit.Repository<Order>().Add(order);
        if (await unit.Complete())
        {
            return order;
        }
        return BadRequest("Problem creating order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetEmail());
        var orders = await unit.Repository<Order>().ListAsync(spec);
        var ordersToReturn = orders.Select(o => o.ToDto()).ToList();
        return Ok(ordersToReturn);
            
    }[HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderByIdForUser(int id)
    {
        var spec = new OrderSpecification(User.GetEmail(), id);
        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
        if (order == null) return NotFound();
        return order.ToDto();
    }

        
}
    
