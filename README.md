# SOSGame
This project is graded based on the development of the SOS game. Two Players aim to create the sequence "S-O-S" on an (n x n) grid system within the games GUI. Players will have the option to either play against as Player vs Player, Player vs Computer, or Computer vs Computer. The final release version of The SOS Game will contain three game modes: **Simple Game**, **General Game**, and **Vulnerable Game** which showcases a crafted LOLBin exploitation path via mshta.exe. 

## Game Features
- Simple Game Mode - End game condition is determined by the first SOS sequence.
- General Game Mode - Final score is dictated by the collective number of SOS sequences completed by each player.
- Human vs Human  Mode
- Player vs Computer Mode
- Computer vs Computer Mode
- **Software Security:** Triggers execution of a hidden, encrypted payload by abusing mshta.exe, a trusted Windows binary (LOLBAS). When a specific condition ```(gridSize == 13)``` is met, AES and RSA-wrapped resources ```(payload.enc, aeskey.enc, private_key.xml)``` are decrypted, dropped into ```%TEMP%```, and executed via ```mshta.exe```, simulating code execution tactics often used by bad actors and within red team operations.


<details>
  <summary> MITRE ATT&CK Mapping</summary>

  
| Technique | Description | ID |
|----------|-------------|----|
| **Signed Binary Proxy Execution: MSHTA** | Executes HTA content through `mshta.exe` | [T1218.005](https://attack.mitre.org/techniques/T1218/005/) |
| **Obfuscated Files or Information** | Encrypts payloads with AES+RSA to avoid detection | [T1027](https://attack.mitre.org/techniques/T1027/) |
| **Ingress Tool Transfer** | Drops payload to local file system (`%TEMP%`) | [T1105](https://attack.mitre.org/techniques/T1105/) |
| **Scripting: VBScript** | Executes script logic via `.hta` format | [T1059.005](https://attack.mitre.org/techniques/T1059/005/) |
| **User Execution (Logic Flaw Trigger)** | Custom logic-based trigger on `gridSize == 13` | [T1204](https://attack.mitre.org/techniques/T1204/) |

</details>



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
