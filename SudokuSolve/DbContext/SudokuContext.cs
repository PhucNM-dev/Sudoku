using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class SudokuContext : DbContext
{
    public SudokuContext(DbContextOptions<SudokuContext> options) : base(options) { }

    public DbSet<Sudoku> Sudokus { get; set; }
}

public class Sudoku
{
    public int Id { get; set; }
    [StringLength(81, MinimumLength = 81, ErrorMessage = "SolvedPuzzle must be exactly 81 characters long.")]
    public string SolvedPuzzle { get; set; }
    public DateTime SolvedAt { get; set; }
}
