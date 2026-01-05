using FindTheBug.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindTheBug.Desktop.Reception.CusomEntity
{
    public class DesktopLabReceipt : LabReceipt
    {
        public bool IsDirty { get; set; }
    }
}
