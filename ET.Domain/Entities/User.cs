using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ET.Domain.Entities;

[Index("Email", Name = "UQ__Users__A9D105346E0AB49B", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalBalance { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MaxLimit { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    [InverseProperty("User")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
