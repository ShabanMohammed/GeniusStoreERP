namespace GeniusStoreERP.Application.Exceptions
{
    public class EntityDeletedException : Exception
    {
        public object Entity { get; }
        public EntityDeletedException(object entity) : base()
        {
            Entity = entity;
        }
    }
}