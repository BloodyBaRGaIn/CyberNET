This program is a “breach protocol” puzzle solver from the game “Cyberpunk 2077”.
How to use:
1.	Launch the game, if it’s not launched already;
2.	Enter the puzzle;
3.	Run the program “CyberHacker.exe”. Make sure you hear a beeps melody. Right after it starts you may proceed to the next step;
4.	Press the printscreen (PrtScr) key on your keyboard. Before that it’s desirable that the cursor is in the upper left corner of the screen;
5.	The program will move the cursor to zero positions. If after that the position is different from zero, move it manually to the upper left corner of the screen (this is necessary for the precise input of the solution by the program);
6.	Wait while the program analyzes the graphical input data and calculates the ideal solution (this takes about five seconds on average and at this time, there may be a decrease in the number of frames per second);
7.	The program will move the cursor and select the matrix elements that match the solution by clicking them. At this moment it is undesirable to switch program windows and manually use the mouse;
8.	If the program proceeds without internal errors, you will hear another short melody made of beeps. Otherwise, you will get an error message. In addition, you can find the resulting files in the LastSolution folder: graphical and textual representations of the solution. In the last file, you can view the extended log information about the time taken to complete the operations and the raw view of the solution;
9.	Press the printscreen key to run the program again or press the End key to exit it.
Remarks:
The program runs correctly only with FullHD (1920x1080px) monitor and the same graphics resolution parameter of the game.
Used resources:
1.	AutoItX library: https://www.autoitscript.com/site/;
2.	Searching for a Bitmap algorithm: https://stackoverflow.com/questions/28586793/c-sharp-search-for-a-bitmap-within-another-bitmap;
3.	Finding solutions algorithm was taken from GitHub and was modified with additional sorting features and code simplifying;
