using SFCTOFC.DailySalesPlanManagement.Domain.Identity;

namespace SFCTOFC.DailySalesPlanManagement.Domain.Events;

    public class LoginAuditCreatedEvent : DomainEvent
    {
        public LoginAuditCreatedEvent(LoginAudit item)
        {
            Item = item;
        }

        public LoginAudit Item { get; }
    }

