<!-- SudokuSolver.vue -->
<template>
    <div class="sudoku-solver">
        <h1>{{ title }}</h1>
        <div class="sudoku-grid">
            <div v-for="(row, rowIndex) in grid"
                 :key="rowIndex"
                 class="sudoku-row">
                <input v-for="(cell, colIndex) in row"
                       :key="colIndex"
                       type="number"
                       min="1"
                       max="9"
                       v-model.number="grid[rowIndex][colIndex]"
                       class="sudoku-cell" />
            </div>
        </div>
        <button @click="solveSudoku" class="solve-button">{{ buttonText }}</button>
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    </div>
</template>
<script lang="ts">
    import { defineComponent, ref, PropType } from 'vue';
    export default defineComponent({
        name: 'SudokuSolver',
        props: {
            title: {
                type: String as PropType<string>,
                default: 'Sudoku Solver'
            },
            buttonText: {
                type: String as PropType<string>,
                default: 'Solve'
            }
        },
        setup() {
            const grid = ref<Array<Array<number | null>>>(
                Array.from({ length: 9 }, () => Array(9).fill(null))
            );
            const errorMessage = ref<string>('');

            const solveSudoku = () => {
                const board = grid.value.map(row => row.slice());
                errorMessage.value = '';
                let isSolve = false;
                if (isValidInput(board)) {
                    if (solve(board)) {
                        grid.value = board;
                        isSolve = true;
                        //emit('solved', grid.value);
                    } else {
                        errorMessage.value = 'This Sudoku cannot be solved.';
                        //emit('error', errorMessage.value);
                    }
                } else {
                    errorMessage.value = 'Invalid input. Please check your numbers.';
                    //emit('error', errorMessage.value);
                }
                if (isSolve) {
                    // request to server 
                    fetch('https://localhost:7152/api/sudoku', {
                        mode: 'no-cors',
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ sudoku: grid.value })
                    })
                        .then(response => {
                            if (!response.ok) {
                                throw new Error('Network response was not ok');
                            }
                            return response.json();
                        })
                        .then(data => {
                            console.log('Success:', data);
                            // Handle response data here if necessary
                        })
                        .catch(error => {
                            console.error('Error:', error);
                            errorMessage.value = 'Error contacting the server.';
                        });
                }
            };

            const isValidInput = (board: (number | null)[][]): boolean => {
                for (let i = 0; i < 9; i++) {
                    for (let j = 0; j < 9; j++) {
                        const value = board[i][j];
                        if (value !== null) {
                            board[i][j] = null; // Temporarily remove the value
                            if (!isSafe(board, i, j, value)) {
                                board[i][j] = value; // Restore the value
                                return false;
                            }
                            board[i][j] = value; // Restore the value
                        }
                    }
                }
                return true;
            };

            const solve = (board: (number | null)[][]): boolean => {
                for (let i = 0; i < 9; i++) {
                    for (let j = 0; j < 9; j++) {
                        if (board[i][j] === null) {
                            for (let num = 1; num <= 9; num++) {
                                if (isSafe(board, i, j, num)) {
                                    board[i][j] = num;
                                    if (solve(board)) {
                                        return true;
                                    }
                                    board[i][j] = null; // Backtrack
                                }
                            }
                            return false; // No solution found
                        }
                    }
                }
                return true; // Solved
            };

            const isSafe = (board: (number | null)[][], row: number, col: number, num: number): boolean => {
                for (let x = 0; x < 9; x++) {
                    if (
                        board[row][x] === num ||
                        board[x][col] === num ||
                        board[3 * Math.floor(row / 3) + Math.floor(x / 3)][3 * Math.floor(col / 3) + (x % 3)] === num
                    ) {
                        return false;
                    }
                }
                return true;
            };

            const resetGrid = () => {
                grid.value = Array.from({ length: 9 }, () => Array(9).fill(null));
                errorMessage.value = '';
                //emit('reset');
            };

            return {
                grid,
                errorMessage,
                solveSudoku,
                resetGrid
            };
        }
    });
</script>

<style scoped>
    .sudoku-solver {
        text-align: center;
        max-width: 500px;
        margin: 0 auto;
        padding: 20px;
    }

    .sudoku-grid {
        display: inline-block;
        margin: 20px 0;
        border: 2px solid #333;
        padding: 2px;
    }

    .sudoku-row {
        display: flex;
    }

        .sudoku-row:nth-child(3n) {
            border-bottom: 2px solid #333;
        }

        .sudoku-row:last-child {
            border-bottom: none;
        }

    .sudoku-cell {
        width: 40px;
        height: 40px;
        text-align: center;
        margin: 1px;
        font-size: 18px;
        border: 1px solid #ccc;
    }

        .sudoku-cell:nth-child(3n) {
            border-right: 2px solid #333;
        }

        .sudoku-cell:last-child {
            border-right: 1px solid #ccc;
        }

        .sudoku-cell::-webkit-inner-spin-button,
        .sudoku-cell::-webkit-outer-spin-button {
            -webkit-appearance: none;
            margin: 0;
        }

    .solve-button {
        padding: 10px 20px;
        font-size: 16px;
        background-color: #4CAF50;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        margin: 10px 0;
    }

        .solve-button:hover {
            background-color: #45a049;
        }

    .error {
        color: red;
        margin-top: 10px;
    }
</style>