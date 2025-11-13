using SFCTOFC.DailySalesPlanManagement.Domain.Common.Entities;

namespace SFCTOFC.DailySalesPlanManagement.Domain.Common.Events;

public class UpdatedEvent<T> : DomainEvent where T : IEntity
{
    public UpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}