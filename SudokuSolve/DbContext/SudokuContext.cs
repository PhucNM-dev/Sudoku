using Microsoft.EntityFrameworkCore;

public class SudokuContext : DbContext
{
    public SudokuContext(DbContextOptions<SudokuContext> options) : base(options) { }

    public DbSet<Sudoku> Sudokus { get; set; }
}

public class Sudoku
{
    public int Id { get; set; }
    public string SolvedPuzzle { get; set; }
    public DateTime SolvedAt { get; set; }
}
