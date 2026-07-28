namespace eCommerce.API.Controllers;

public record UserDTO(Guid UserID, string? Email, string? PersonName, string Gender);