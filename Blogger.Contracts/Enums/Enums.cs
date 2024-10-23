using System.ComponentModel;

namespace Blogger.Contracts.Enums;

public enum EntityStatus
{
    [Description("Active")] Active = 1,
    [Description("Deleted")] Deleted = 2
}
