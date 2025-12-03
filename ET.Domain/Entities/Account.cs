using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ET.Domain.Entities;

public partial class Account
{
    [Key]
    public int AccountId { get; set; }

    [StringLength(100)]
    public string AccountName { get; set; } = null!;

    [StringLength(50)]
    public string? AccountType { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
