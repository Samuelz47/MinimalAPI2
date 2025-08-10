using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPI2.Domain.Entities;

public class Administrator
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(255)]
    public string Email { get; set; } = default!;
    [StringLength(50)]
    public string Senha { get; set; } = default!;
    [StringLength(50)]
    public string Perfil { get; set; } = default!;
}
