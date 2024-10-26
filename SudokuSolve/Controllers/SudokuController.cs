using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SudokuController : ControllerBase
{
    private readonly SudokuContext _context;

    public SudokuController(SudokuContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> PostSudoku([FromBody] JsonObject bodyRequest)
    {
        if (bodyRequest == null)
        {
            return BadRequest("Request is invalid, please provide Sudoku data.");
        }
        var sudokuArray = bodyRequest["sudoku"].AsArray();

        // Convert the JsonArray to a jagged array
        int[][] jaggedArray = new int[9][];
        for (int i = 0; i < 9; i++)
        {
            jaggedArray[i] = new int[9];
            for (int j = 0; j < 9; j++)
            {
                jaggedArray[i][j] = sudokuArray[i][j].GetValue<int>();
            }
        }

        // Flatten the jagged array and convert to a single string
        string result = string.Join("", jaggedArray.SelectMany(arr => arr));

        Sudoku sudoku = new Sudoku
        {
            SolvedAt = DateTime.Now,
            SolvedPuzzle = result//sudokuString
        };
        _context.Sudokus.Add(sudoku);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Sudoku solve saved in database.", bodyRequest });
    }
}
