using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> PostSudoku([FromBody] JsonObject requestBody)
    {

        if (requestBody == null)
        {
            return BadRequest("Request is invalid, please !");
        }
        // requestBody
        //sudoku.SolvedAt = DateTime.UtcNow;
        //_context.Sudokus.Add(sudoku);
        //await _context.SaveChangesAsync();
        return Ok("sudoku solve saved on database.");
    }
}
