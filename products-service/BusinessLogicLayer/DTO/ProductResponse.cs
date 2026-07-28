using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO;

public record ProductResponse(Guid ProductId,
                              string ProductName,
                              CategoryOption Category,
                              double? UnitPrice,
                              int? QuantityInStock)
{
    public ProductResponse() : this(default, string.Empty, default, default, default)
    {

    }
}
