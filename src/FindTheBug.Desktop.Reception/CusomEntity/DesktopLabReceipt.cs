using FindTheBug.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindTheBug.Desktop.Reception.CusomEntity
{
    [Table("LabReceipts")]
    public class DesktopLabReceipt : LabReceipt, IPushableEntity
    {
        bool IPushableEntity.IsDirty { get; set; }
    }

    public interface IPushableEntity
    {
        bool IsDirty { get; set; }
    }
}
