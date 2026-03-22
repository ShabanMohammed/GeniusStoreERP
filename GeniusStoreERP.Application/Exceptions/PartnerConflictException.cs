using GeniusStoreERP.Domain.Entities.Partners;

namespace GeniusStoreERP.Application.Exceptions;

public class EntityConflictException : Exception
{
    public object Entity { get; }
    public EntityConflictException(object entity) : base()
    {
        Entity = entity;
    }
}