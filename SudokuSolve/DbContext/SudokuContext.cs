﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
<<<<<<< HEAD
<<<<<<< HEAD
using System.Runtime.InteropServices;
=======
>>>>>>> 81c5a6e7d34ec0b5d44dc23f99b96a8a1204582a
=======
>>>>>>> 81c5a6e (feat: db code first, dxDataGrid DevExtreme)

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
