# SOSGame
This project is graded based on the development of the SOS game. Two Players aim to create the sequence "S-O-S" on an (n x n) grid system within the games GUI. Players will have the option to either play against as Player vs Player, Player vs AI, or select an a Vulnerable Game State which allows the exploitation of a Use After Free memory issue. The final implementation will contain two game modes: **Simple Game** and **General Game**. 

## Game Features
- Simple Game Mode - End game condition is determined by the first SOS sequence.
- General Game Mode - Final score is dictated by the collective number of SOS sequences completed by each player.
- Human vs Human Player Mode
- Player vs AI Mode
- **New Feature [Software Security]:** Vulnerable Game State - Allow a bad actor to dispose a stream to or introduce a race condition where it is freed but still accessible.

## Requirements
- Develop game with an object-oriented programming language (C#)
- Utilize unit testing frameworks for testing (xUnit)
- GUI Library to build the graphical interface of the game (AvaloniaUI)

## Game Setup
**Running from source code - Development Mode:**
1. Clone the repository to your local machine e.g., git clone https://github.com/kimbrow-slice/SOSGame.git
2. Open the game file in Visual Studio Code or your choosen IDE.
3. Ensure the project dependencies are installed by running `dotnet restore`
4. To run the game in debug mode, in Visual Studio press `F5` or Click `Start`
5. Follow the instructions on your screen to start your game!

## References: 
 - [AvaloniaUI Documentation](https://docs.avaloniaui.net/docs/)
 - [AvaloniaUI Styling Guide](https://docs.avaloniaui.net/docs/0.10.x/styling/styles#pseudoclasses)
 - [C# Style Guide](https://google.github.io/styleguide/csharp-style.html)


### To Do for Sprint 3:
1. **New Feature[LOGIC]:** Complete Win Condition checks for both SIMPLE and GENERAL game modes
2. **New Feature[GUI]:** Create a dynamic scoreboard into the GUI
3. **New Feature[GUI]:GUI** Update for SOS sequence completion
4. **New Feature[Code Readability]:** Create a dedicated Styles.axaml file for all GUI styling
5. **New Feature[Code Readability]:** Add a .gitignore for all build files
