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
                       maxlength="1"
                       min="1"
                       max="9"
                       v-model.number="grid[rowIndex][colIndex]"
                       @input="validateInput(rowIndex, colIndex)"
                       :class="{'wrong-number': isWrongNumber(rowIndex, colIndex)}"
                       class="sudoku-cell" />
            </div>
        </div>
        <div class="button-container">
            <button @click="solveSudoku" class="solve-button">{{ buttonText }}</button>
        </div>
        <div class="button-container">
            <input type="file" id="file-input" @change="importExcel" class="file-input-real" />
            <label for="file-input" class="file-input-label">Choose File</label>
        </div>
        <div class="button-container">
            <button @click="resetGrid" class="reset-button">{{ buttonReset }}</button>
        </div>
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
        <!-- Display the data in DxDataGrid -->
        <DxDataGrid :dataSource="sudokuData" showBorders="true" :paging="{ enabled: true, pageSize: 5 }">
            <DxColumn dataField="id" caption="ID" />
            <DxColumn dataField="solvedPuzzle" caption="Solved Puzzle" />
            <DxColumn dataField="solvedAt" caption="Solved At" dataType="date" />
            <DxPager :allowedPageSizes="[5, 10, 20]" showPageSizeSelector="true" />
            <DxPaging :pageSize="5" />
        </DxDataGrid>
        <!-- Success Modal -->
        <div v-if="showSuccessPopup" class="modal">
            <div class="modal-content">
                <span class="close" @click="closePopup">&times;</span>
                <p>{{ successMessage }}</p>
            </div>
        </div>
    </div>
</template>


<script lang="ts">
    import { defineComponent, ref, onMounted, PropType } from 'vue';
    import * as XLSX from 'xlsx';
    import DxDataGrid from 'devextreme-vue/data-grid';

    export default defineComponent({
        name: 'SudokuSolver',
        components: {
            DxDataGrid
        },
        props: {
            title: {
                type: String as PropType<string>,
                default: 'Sudoku Solver'
            },
            buttonText: {
                type: String as PropType<string>,
                default: 'Solve'
            },
            buttonReset: {
                type: String as PropType<string>,
                default: 'Reset'
            },
            importFile: {
                type: String as PropType<string>,
                default: 'Import File'
            }
        },
        setup(_, { emit }) {
            const grid = ref<Array<Array<number | null>>>(
                Array.from({ length: 9 }, () => Array(9).fill(null))
            );
            const errorMessage = ref<string>('');
            const sudokuData = ref([]);
            const showSuccessPopup = ref(false);
            const successMessage = ref<string>('');

            const fetchSudokuData = async () => {
                try {
                    const response = await fetch('https://localhost:7152/api/sudoku');
                    sudokuData.value = await response.json();
                } catch (error) {
                    console.error('Error fetching data:', error);
                }
            };

            onMounted(() => {
                fetchSudokuData();
            });

            const importExcel = (event: Event) => {
                const input = event.target as HTMLInputElement;
                if (input.files?.length) {
                    const file = input.files[0];
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        const data = e.target?.result;
                        const workbook = XLSX.read(data, { type: 'binary' });
                        const firstSheetName = workbook.SheetNames[0];
                        const worksheet = workbook.Sheets[firstSheetName];
                        const jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
                        populateGrid(jsonData);
                    };
                    reader.readAsBinaryString(file);
                }
            };

            const populateGrid = (data: any[]) => {
                for (let i = 0; i < 9; i++) {
                    for (let j = 0; j < 9; j++) {
                        grid.value[i][j] = data[i][j] || null;
                    }
                }
            };
            const solveSudoku = () => {
                const board = grid.value.map(row => row.slice());
                errorMessage.value = '';
                let isSolve = false;
                if (isValidInput(board)) {
                    if (solve(board)) {
                        grid.value = board;
                        isSolve = true;
                        emit('solved', grid.value);
                        showSuccessPopup.value = true;
                    } else {
                        errorMessage.value = 'This Sudoku cannot be solved.';
                        emit('error', errorMessage.value);
                    }
                } else {
                    errorMessage.value = 'Invalid input. Please check your numbers.';
                    emit('error', errorMessage.value);
                }
                if (isSolve) {
                    // request to server
                    fetch('https://localhost:7152/api/sudoku', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ sudoku: grid.value })
                    })
                        .then(response => {
                            console.log('Status:', response.status);
                            if (!response.ok) {
                                throw new Error('Network response was not ok');
                            }
                            return response.json();
                        })
                        .then(data => {
                            console.log('Success:', data);
                            successMessage.value = data.message;
                            fetchSudokuData();  // Refresh the grid data immediately
                        })
                        .catch(error => {
                            console.error('Error:', error);
                            errorMessage.value = 'Error contacting the server.';
                        });
                }
            };

            const closePopup = () => {
                showSuccessPopup.value = false;
            };

            const validateInput = (rowIndex: number, colIndex: number) => {
                const currentValue = grid.value[rowIndex][colIndex];
                if (currentValue === null || currentValue === undefined) return;

                if (currentValue < 1 || currentValue > 9) {
                    grid.value[rowIndex][colIndex] = null;
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
                emit('reset');
            };

            const isWrongNumber = (row: number, col: number) => {
                const number = grid.value[row][col];
                if (number === null) return false;
                for (let i = 0; i < 9; i++) {
                    if (i !== col && grid.value[row][i] === number) return true;
                    if (i !== row && grid.value[i][col] === number) return true;
                }
                const startRow = Math.floor(row / 3) * 3;
                const startCol = Math.floor(col / 3) * 3;
                for (let i = startRow; i < startRow + 3; i++) {
                    for (let j = startCol; j < startCol + 3; j++) {
                        if ((i !== row || j !== col) && grid.value[i][j] === number) return true;
                    }
                }
                return false;
            };

            return {
                grid,
                errorMessage,
                solveSudoku,
                resetGrid,
                isWrongNumber,
                importExcel,
                sudokuData,
                showSuccessPopup,
                closePopup,
                validateInput,
                successMessage
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

    .button-container {
        display: block; /* Ensure each button container is a block element */
        margin-bottom: 10px;
    }

    .solve-button,
    .reset-button {
        display: inline-block; /* Inline-block to prevent full width */
        padding: 10px 20px;
        font-size: 16px;
        border: none;
        cursor: pointer;
        margin: 5px; /* Add some margin to separate buttons */
    }

    .solve-button {
        background-color: #4CAF50;
        color: white;
    }

    .reset-button {
        background-color: #2196F3;
        color: white;
    }

        .solve-button:hover,
        .reset-button:hover {
            opacity: 0.8;
        }

    .wrong-number {
        background-color: #ffcccc; /* Red background for wrong numbers */
    }

    .error {
        color: red;
        margin-top: 10px;
    }

    .file-input-container {
        position: relative;
        display: inline-block;
        margin-bottom: 10px;
    }

    .file-input-real {
        display: none; /* Hide the actual file input */
    }

    .file-input-label {
        display: inline-block;
        padding: 10px 20px;
        color: white;
        background-color: #ccc; /* Blue background */
        border: none;
        cursor: pointer;
        font-size: 16px;
        border-radius: 4px;
        text-align: center;
    }

        .file-input-label:hover {
            background-color: #1E88E5; /* Darker blue on hover */
        }

        .file-input-label:active {
            background-color: #1976D2; /* Even darker blue on click */
        }

    /* Modal CSS */
    .modal {
        display: block; /* Show modal */
        position: fixed; /* Stay in place */
        z-index: 1; /* Sit on top */
        left: 0;
        top: 0;
        width: 100%; /* Full width */
        height: 100%; /* Full height */
        overflow: auto; /* Enable scroll if needed */
        background-color: rgb(0, 0, 0); /* Fallback color */
        background-color: rgba(0, 0, 0, 0.4); /* Black w/ opacity */
        padding-top: 60px;
    }

    .modal-content {
        background-color: #fefefe;
        margin: 5% auto; /* 15% from the top and centered */
        padding: 20px;
        border: 1px solid #888;
        width: 50%; /* Smaller width for modal */
        border-radius: 10px; /* Rounded corners */
        box-shadow: 0 5px 15px rgba(0,0,0,0.3); /* Nice shadow effect */
        animation: animateModal 0.4s; /* Animation */
    }

    @keyframes animateModal {
        from {
            opacity: 0;
            transform: scale(0.7);
        }

        to {
            opacity: 1;
            transform: scale(1);
        }
    }

    .close {
        color: #aaa;
        float: right;
        font-size: 28px;
        font-weight: bold;
    }

        .close:hover,
        .close:focus {
            color: black;
            text-decoration: none;
            cursor: pointer;
        }

    .modal p {
        font-size: 18px; /* Better readability */
    }

    /* General styling for DataGrid */
    .dx-datagrid {
        margin-top: 20px;
        font-family: Arial, sans-serif;
    }

    /* Pager container */
    .dx-pager {
        text-align: center;
        margin: 20px 0;
    }

    /* Pager navigation buttons */
    .dx-page, .dx-page-sizes, .dx-page-sizes-selectbox, .dx-prev-page, .dx-next-page {
        display: inline-block;
        margin: 0 5px;
    }

        /* Active page styling */
        .dx-page.dx-page-active {
            background-color: #4CAF50;
            color: white;
            border-radius: 4px;
            padding: 5px 10px;
        }

        /* Hover effects for pager buttons */
        .dx-page:hover, .dx-page-sizes:hover, .dx-prev-page:hover, .dx-next-page:hover {
            background-color: #f1f1f1;
            cursor: pointer;
        }

        /* Styling for page size selector */
        .dx-page-sizes-selectbox .dx-selectbox-container {
            display:
        }
</style>