# Solving Solo Chess
This project is part of my bachelor thesis "Solving Solo Chess", which is made as a completion of my Bachelor education in Artificial Intelligence
at Utrecht University. For more information. The paper is available *here*.

## How It Works
The program is a Windows Forms App (.NET Framework). When running *Program.cs*, a GUI for the game of Solo Chess is shown. The game can be played manually

The following interactive components are available:
* By clicking on the panel containing the chess board, the user can play the game manually. Clicked pieces are highlighted in green and the corresponding options for possible attacks are highlighted in red.
* *Generate*-Button: randomly generates a solvable puzzle. The desired number of pieces can be specified using the *NumericUpDown*-Control.
* *Solve*-Button: solves the puzzle and visualizes the solution using animation. The desired heuristic function used can be specified by the *ComboBox*-Control. The full solution, including the number of backtracks it took to find a solution, is also displayed in the console.
* *Restart*-Button: restores the start configuration of the puzzle and restarts the game.
* *Input*-Button: provides the possibility to open puzzle instances from textfiles. 

## Screenshot
![screenshot](/screenshot.jpg)

## Author
* Stan Verlaan
