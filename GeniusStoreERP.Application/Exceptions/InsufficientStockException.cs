using GeniusStoreERP.Application.Exceptions;

namespace GeniusStoreERP.Application.Exceptions;

public class InsufficientStockException : BusinessException
{
    public InsufficientStockException(string productName) 
        : base($"عفواً، الرصيد الحالي للمنتج ({productName}) غير كافٍ لإتمام العملية.")
    {
    }
}
