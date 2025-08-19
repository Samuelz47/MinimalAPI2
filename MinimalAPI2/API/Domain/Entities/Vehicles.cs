using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPI2.Domain.Entities;

public class Vehicles
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = default!;
    [Required]
    [StringLength(100)]
    public string Marca { get; set; } = default!;
    [Required]
    public int Ano { get; set; } = default!;
}
