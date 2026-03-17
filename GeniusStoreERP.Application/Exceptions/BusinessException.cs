namespace GeniusStoreERP.Application.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string Mamessage) : base(Mamessage) { }

}
public class NotFoundException : Exception
{
    public NotFoundException(object key) :
        base($"العنصر صاحب الكود ({key}) غير موجود.")
    { }

}
