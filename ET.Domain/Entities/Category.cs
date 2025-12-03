using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ET.Domain.Entities;

public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string Type { get; set; } = null!;

    public int UserId { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    [ForeignKey("UserId")]
    [InverseProperty("Categories")]
    public virtual User User { get; set; } = null!;
}
