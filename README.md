# SOSGame
This project is graded based on the development of the SOS game. Two Players aim to create the sequence "S-O-S" on an (n x n) grid system within the games GUI. Players will have the option to either play against as Player vs Player, Player vs AI model, or select an a Vulnerable Game State which allows the exploitation of a Use After Free memory issue. The final implementation will contain two game modes: Simple Game and General Game. 

## Game Features
- Simple Game Mode - End game condition is determined by the first SOS sequence.
- General Game Mode - Final score is dictated by the collective number of SOS sequences completed by each player.
- Human vs Human Player Mode
- Player vs Computer Mode
- feat[Software Security]: Vulnerable Game State - Allow a bad actor to dispose a stream to or introduce a race condition where it is freed but still accessible.

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

**Running the Released Version**:
1. Clone the repository to your local machine e.g., git clone https://github.com/kimbrow-slice/SOSGame.git
2. Open a command line interface and navigate to the directory where the project was downloaded, e.g., `cd C:\Users\USRENAMEHERE\Downloads\SOSGAME`
3. Once in the directory, type `dotnet publish -c Release -r win-x64 --self-contained true` 
**NOTE: This command is specific to Windows-64 bit, if running linux change to `linux64` or `osx-64` and run the game with `./SOSGame` while in the projects directory.**
4. Navigate to the build output folder `cd cd bin\Release\net9.0\win-x64\publish`
5. Run the executable file `SOSGame.exe`

## References: 
 - [AvaloniaUI Documentation](https://docs.avaloniaui.net/docs/)
 - [AvaloniaUI Styling Guide](https://docs.avaloniaui.net/docs/0.10.x/styling/styles#pseudoclasses)
 - [C# Style Guide](https://google.github.io/styleguide/csharp-style.html)


### To Do for Sprint 3:
1. feat[LOGIC]:Complete Win Condition checks for both SIMPLE and GENERAL game modes
2. feat[GUI]:Create a dynamic scoreboard into the GUI
3. feat[GUI]:GUI update for SOS sequence completion
4. feat[Code Readability]:Create a dedicated Styles.axaml file for all GUI styling
5. feat[Code Readability]:Add a .gitignore for all build files
