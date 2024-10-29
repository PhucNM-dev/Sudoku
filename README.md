# SudokuSolve: Full-Stack Application  

SudokuSolve is a full-stack application designed to solve Sudoku puzzles efficiently and provide a user-friendly interface for managing puzzles.  

## Features  
- **Solve Sudoku puzzles**: Input numbers and solve puzzles directly in the application.  
- **Import Sudoku puzzles from Excel files**: Easily upload puzzles in Excel format for solving.  
- **Display previously solved puzzles**: View a list of solved puzzles in a paginated data grid.  
- **Success notifications**: Receive popup notifications upon successfully solving a Sudoku puzzle.  

## Technologies Used  

### Frontend  
- **Vue.js**: A progressive JavaScript framework for building user interfaces.  
- **DevExtreme**: A set of responsive web development components, including the DxDataGrid for displaying data.  
- **XLSX.js**: A library for reading and writing Excel files, enabling easy import of Sudoku puzzles.  

### Backend  
- **ASP.NET Core**: A robust web framework for building backend services and APIs.  
- **Entity Framework Core**: An Object-Relational Mapper (ORM) for database operations, simplifying data access.  

### Database  
- **SQL Server**: A relational database management system used to store solved puzzles and related data.  

## Architecture  
The application follows a client-server architecture:  

- **Frontend**: Built with Vue.js and DevExtreme components, allowing users to input Sudoku numbers, import puzzles from Excel files, and solve them. Solved puzzles are displayed in a paginated data grid for easy access.  
  
- **Backend**: An ASP.NET Core service that manages CRUD operations for Sudoku puzzles and interacts with the SQL Server database using Entity Framework Core.  
  
- **Database**: Stores solved Sudoku puzzles along with their respective timestamps for record-keeping.  

## Getting Started

### Prerequisites  
Ensure you have the following installed:  
- .NET SDK 6  or higher  
- Nodejs: v18.20.4
- vuejs: v3.2.13
- SQL Server  

### 1. Installation:  
1. Clone the repository:
    ```bash  
    git clone https://github.com/PhucNM-dev/Sudoku.git
    ```

2. Run the frontend application:

   ```bash
   cd Sudoku/Sudoku-ui
   npm install
   ```

4. Visit `http://localhost:8080` in your browser by start the application.

## Usage
1. Open your browser and navigate to http://localhost:8080.

2. Use the Sudoku grid to input numbers or import a puzzle from an Excel file.

3. Click the "Solve Sudoku" button to solve the puzzle.

4. View the results and previously solved puzzles in the data grid.
