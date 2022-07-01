# Solving Solo Chess
This project is part of my bachelor thesis "Solving Solo Chess", which is made as a completion of my Bachelor education in Artificial Intelligence
at Utrecht University. The paper is available [here](/BachelorThesis_StanVerlaan6813526.pdf).

## About the Research
In this thesis project, we study a simplified puzzle variant of the game of chess, termed
Solo Chess. Solo Chess is a single player puzzle game which can be found on [chess.com](https://www.chess.com). While the game is proven to be NP-complete, and an efficient algorithm for solving it does not seem to exist, we propose an algorithmic approach
for solving the game by formulating the problem as a Bounded Classical Planning Problem and designing and applying a general backtracking algorithm.
In order to investigate which tactics and approaches work best when playing
Solo Chess, we incorporate knowledge and intuition about the game into the
algorithm in the form of heuristics that guide the search to a goal by picking
which moves to consider first. Results from testing on 11, 000 puzzles generated
ourselves show that a heuristic based on a ranking of the piece types is consistently effective on all kinds of puzzles, and that a heuristic based on moving the
pieces to the center can achieve outstanding performance under circumstances
in which there are few pieces on the board or when there are relatively more
high ranked pieces present in the start configuration.

## About Solo Chess
Given a board configuration, the object of Solo Chess is to move the
pieces one by one in such a way that they all capture each other until only one piece
remains. The rules are as follows:
1. Every move must capture a piece
2. No piece may capture more than 2 times per puzzle
3. If there is a King on the board, it must be the final piece

The difficulty increases as more and more pieces are added to the puzzles, and one
must navigate the maze of captures and find the path that leads to a lone remaining
piece. Note that a specific instance may have more than one solution. [Try it out here](https://www.chess.com/solo-chess).


## How the Program Works
The program is a Windows Forms App (.NET Framework). When running *Program.cs*, a GUI for the game of Solo Chess is shown. The user can play the game manually or experiment with the heuristics and let puzzles be solved automatically.

The following interactive components are available:
* By clicking on the panel containing the chess board, the user can play the game manually. Clicked pieces are highlighted in green and the corresponding options for possible attacks are highlighted in red.
* *Generate*-Button: randomly generates a solvable puzzle. The desired number of pieces can be specified using the *NumericUpDown*-Control.
* *Solve*-Button: solves the puzzle and visualizes the solution using animation. The desired heuristic function used can be specified by the *ComboBox*-Control. The full solution, including the number of backtracks it took to find a solution, is also displayed in the console.
* *Restart*-Button: restores the start configuration of the puzzle and restarts the game.
* *Input*-Button: provides the possibility to open puzzle instances from textfiles. 

## Screenshot
![screenshot](/screenshot.jpg)

## Other Files
The following files are also available in this repository:
* [Inputs](https://github.com/Sverlaan/SoloChess/tree/main/SoloChess/SoloChess/inputs): the input textfiles containing the randomly generated puzzle instances used in testing. This folder contains 1000 puzzles for each difficulty level between 2 and 14, each of which can be opened and played using our C# program.
* [Results](https://github.com/Sverlaan/SoloChess/tree/main/SoloChess/SoloChess/results): the results from testing. This contains the full data, including the number of backtracks for each heuristic approach on all inputs, as well as the Jupyter Notebook containing the python code used for the statistical analysis of the results.

## Author
* Stan Verlaan

## Acknowledgements
Acknowledgements and references can be found in the paper and code.
