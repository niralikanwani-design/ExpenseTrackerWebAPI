using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ET.Domain.Entities;

public partial class Transaction
{
    [Key]
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(20)]
    public string Type { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime TransactionDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("title")]
    [StringLength(400)]
    public string? Title { get; set; }

    public int? AccountId { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Transactions")]
    public virtual Account? Account { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Transactions")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Transactions")]
    public virtual User User { get; set; } = null!;
}
