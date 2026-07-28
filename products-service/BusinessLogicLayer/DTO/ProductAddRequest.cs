using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO;

public record ProductAddRequest(string ProductName,
                                CategoryOption Category,
                                double? UnitPrice,
                                int? QuantityInStock)
{
    public ProductAddRequest() : this(string.Empty, default, default, default)
    {

    }
}
