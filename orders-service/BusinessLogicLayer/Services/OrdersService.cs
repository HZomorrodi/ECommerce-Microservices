using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.HttpClients;
using eCommerce.OrdersMicroservice.BusinessLogicLayer.DTO;
using eCommerce.OrdersMicroservice.BusinessLogicLayer.ServiceContracts;
using eCommerce.OrdersMicroservice.DataAccessLayer.Entities;
using eCommerce.OrdersMicroservice.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;

namespace eCommerce.ordersMicroservice.BusinessLogicLayer.Services;

public class OrdersService(IOrdersRepository ordersRepository,
                           IMapper mapper,
                           IValidator<OrderAddRequest> orderAddRequestValidator,
                           IValidator<OrderItemAddRequest> orderItemAddRequestValidator,
                           IValidator<OrderUpdateRequest> orderUpdateRequestValidator,
                           IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator,
                           UsersMicroserviceClient usersMicroserviceClient,
                           ProductsMicroserviceClient prouductsMicroserviceClient) : IOrdersService
{
    private readonly IValidator<OrderAddRequest> _orderAddRequestValidator = orderAddRequestValidator;
    private readonly IValidator<OrderItemAddRequest> _orderItemAddRequestValidator = orderItemAddRequestValidator;
    private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator = orderUpdateRequestValidator;
    private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
    private readonly UsersMicroserviceClient usersMicroserviceClient = usersMicroserviceClient;
    private readonly ProductsMicroserviceClient prouductsMicroserviceClient = prouductsMicroserviceClient;
    private readonly IMapper _mapper = mapper;
    private IOrdersRepository _ordersRepository = ordersRepository;

    public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
    {
        //Check for null parameter
        ArgumentNullException.ThrowIfNull(orderAddRequest);


        //Validate OrderAddRequest using Fluent Validations
        ValidationResult orderAddRequestValidationResult = await _orderAddRequestValidator.ValidateAsync(orderAddRequest);
        if (!orderAddRequestValidationResult.IsValid)
        {
            string errors = string.Join(", ", orderAddRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
            throw new ArgumentException(errors);
        }
        List<ProductDTO> products = [];
        //Validate order items using Fluent Validation
        foreach (OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
        {
            ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);

            if (!orderItemAddRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ", orderItemAddRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }
            ProductDTO productDTO = await prouductsMicroserviceClient.GetProductsByProuductID(orderItemAddRequest.ProductID) ?? throw new ArgumentException("Invalid Product ID");
            products.Add(productDTO);
        }

        //TO DO: Add logic for checking if UserID exists in Users microservice
        UserDTO user = await usersMicroserviceClient.GetUserByUserID(orderAddRequest.UserID) ?? throw new ArgumentException("Invalid User ID");


        //Convert data from OrderAddRequest to Order
        Order orderInput = _mapper.Map<Order>(orderAddRequest); //Map OrderAddRequest to 'Order' type (it invokes OrderAddRequestToOrderMappingProfile class)

        //Generate values
        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);

        //Invoke repository
        Order? addedOrder = await _ordersRepository.AddOrder(orderInput);

        if (addedOrder == null)
        {
            return null;
        }

        OrderResponse addedOrderResponse = _mapper.Map<OrderResponse>(addedOrder); //Map addedOrder ('Order' type) into 'OrderResponse' type (it invokes OrderToOrderResponseMappingProfile).
        if (addedOrderResponse is not null)
        {
            foreach (OrderItemResponse OrderItemResponse in addedOrderResponse.OrderItems)
            {
                ProductDTO? productDTO = products.FirstOrDefault(x => x.ProductId == OrderItemResponse.ProductID);
                if (productDTO is not null)
                {
                    _mapper.Map(productDTO, OrderItemResponse);
                }
            }
            _mapper.Map(user, addedOrderResponse);
        }
        return addedOrderResponse;
    }



    public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
    {
        //Check for null parameter
        ArgumentNullException.ThrowIfNull(orderUpdateRequest);


        //Validate OrderAddRequest using Fluent Validations
        ValidationResult orderUpdateRequestValidationResult = await _orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);
        if (!orderUpdateRequestValidationResult.IsValid)
        {
            string errors = string.Join(", ", orderUpdateRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
            throw new ArgumentException(errors);
        }
        List<ProductDTO> products = [];

        //Validate order items using Fluent Validation
        foreach (OrderItemUpdateRequest orderItemUpdateRequest in orderUpdateRequest.OrderItems)
        {
            ValidationResult orderItemUpdateRequestValidationResult = await _orderItemUpdateRequestValidator.ValidateAsync(orderItemUpdateRequest);

            if (!orderItemUpdateRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ", orderItemUpdateRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }
            ProductDTO productDTO = await prouductsMicroserviceClient.GetProductsByProuductID(orderItemUpdateRequest.ProductID) ?? throw new ArgumentException("Invalid Product ID");
            products.Add(productDTO);
        }

        //TO DO: Add logic for checking if UserID exists in Users microservice
        UserDTO user = await usersMicroserviceClient.GetUserByUserID(orderUpdateRequest.UserID) ?? throw new ArgumentException("Invalid User ID");

        //Convert data from OrderUpdateRequest to Order
        Order orderInput = _mapper.Map<Order>(orderUpdateRequest); //Map OrderUpdateRequest to 'Order' type (it invokes OrderUpdateRequestToOrderMappingProfile class)

        //Generate values
        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);


        //Invoke repository
        Order? updatedOrder = await _ordersRepository.UpdateOrder(orderInput);

        if (updatedOrder == null)
        {
            return null;
        }

        OrderResponse updatedOrderResponse = _mapper.Map<OrderResponse>(updatedOrder); //Map updatedOrder ('Order' type) into 'OrderResponse' type (it invokes OrderToOrderResponseMappingProfile).
        if (updatedOrderResponse is not null)
        {
            foreach (OrderItemResponse OrderItemResponse in updatedOrderResponse.OrderItems)
            {
                ProductDTO? productDTO = products.FirstOrDefault(x => x.ProductId == OrderItemResponse.ProductID);
                if (productDTO is not null)
                {
                    _mapper.Map(productDTO, OrderItemResponse);
                }
            }
            _mapper.Map(user, updatedOrderResponse);
        }
        return updatedOrderResponse;
    }


    public async Task<bool> DeleteOrder(Guid orderID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, orderID);
        Order? existingOrder = await _ordersRepository.GetOrderByCondition(filter);

        if (existingOrder == null)
        {
            return false;
        }


        bool isDeleted = await _ordersRepository.DeleteOrder(orderID);
        return isDeleted;
    }


    public async Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter)
    {
        Order? order = await _ordersRepository.GetOrderByCondition(filter);
        if (order == null)
            return null;

        OrderResponse orderResponse = _mapper.Map<OrderResponse>(order);
        if (orderResponse is not null)
        {
            foreach (OrderItemResponse OrderItemResponse in orderResponse.OrderItems)
            {
                ProductDTO? productDTO = await prouductsMicroserviceClient.GetProductsByProuductID(OrderItemResponse.ProductID);
                if (productDTO is not null)
                {
                    _mapper.Map(productDTO, OrderItemResponse);
                }
            }
            UserDTO? user = await usersMicroserviceClient.GetUserByUserID(orderResponse.UserID);
            if (user is not null)
            {
                _mapper.Map(user, orderResponse);
            }
        }
        return orderResponse;
    }


    public async Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
    {
        IEnumerable<Order?> orders = await _ordersRepository.GetOrdersByCondition(filter);


        IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);
        foreach (OrderResponse? orderResponse in orderResponses)
        {
            if (orderResponse is not null)
            {
                foreach (OrderItemResponse OrderItemResponse in orderResponse.OrderItems)
                {
                    ProductDTO? productDTO = await prouductsMicroserviceClient.GetProductsByProuductID(OrderItemResponse.ProductID);
                    if (productDTO is not null)
                    {
                        _mapper.Map(productDTO, OrderItemResponse);
                    }
                }
                UserDTO? user = await usersMicroserviceClient.GetUserByUserID(orderResponse.UserID);
                if (user is not null)
                {
                    _mapper.Map(user, orderResponse);
                }
            }
        }

        return orderResponses.ToList();
    }


    public async Task<List<OrderResponse?>> GetOrders()
    {
        IEnumerable<Order?> orders = await _ordersRepository.GetOrders();

        IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);
        foreach (OrderResponse? orderResponse in orderResponses)
        {
            if (orderResponse is not null)
            {
                foreach (OrderItemResponse OrderItemResponse in orderResponse.OrderItems)
                {
                    ProductDTO? productDTO = await prouductsMicroserviceClient.GetProductsByProuductID(OrderItemResponse.ProductID);
                    if (productDTO is not null)
                    {
                        _mapper.Map(productDTO, OrderItemResponse);
                    }
                }
                UserDTO? user = await usersMicroserviceClient.GetUserByUserID(orderResponse.UserID);
                if (user is not null)
                {
                    _mapper.Map(user, orderResponse);
                }
            }


        }
        return orderResponses.ToList();
    }
}