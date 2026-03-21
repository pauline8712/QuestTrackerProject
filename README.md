# QuestTracker 

QuestTracker is a C# console-based RPG-style task management system designed for "Guild Heroes." It combines traditional productivity tracking with immersive elements like Two-Factor Authentication (2FA) for login and a persistent SQL Server database for quest storage.

##  Features

- **Hero Authentication**: Secure login system with Twilio-powered 2FA.
- **Quest Management**: Create, update, complete, and track missions with priorities (High/Medium/Low).
- **Persistent Storage**: Full SQL Server integration to ensure your quests are never lost.
- **Visual Feedback**: Interactive 2FA input screens and color-coded console outputs.
- **Notification Service**: Automatic reminders for missions nearing their deadline.
- **Guild Reports**: Summaries of completed vs. pending missions.

##  Technical Stack

- **Language**: C# / .NET 8.0
- **Database**: SQL Server (Express)
- **APIs**: Twilio (for SMS 2FA)
- **Libraries**: 
  - `Microsoft.Data.SqlClient` (Database connectivity)
  - `dotenv.net` (Environment variable management)
  - `Newtonsoft.Json` (User data persistence)

---

##  Project Structure

```text
QuestTracker/
├── QuestTracker.sln            # Visual Studio Solution file
└── QuestTracker/               # Main Project Directory
    ├── Authenticator.cs        # Handles User registration, login, and 2FA logic
    ├── GuildHelperAI.cs        # (Planned/In-progress) AI-driven mission guidance
    ├── MenuHelper.cs           # Console-based UI menus and navigation
    ├── NotificationService.cs  # Monitors and alerts for upcoming quest deadlines
    ├── Program.cs              # Main entry point and database connectivity tests
    ├── Quest.cs                # Quest data model and console input handling
    ├── QuestManager.cs         # Core logic for SQL CRUD operations (Add/Update/Delete)
    ├── User.cs                 # User data model for hero accounts
    ├── QuestTracker.csproj     # Project configuration and NuGet dependencies
    ├── .env                    # Environment variables (Twilio SID, Tokens) - [IGNORED BY GIT]
    └── users.json              # Local persistence for user credentials
```

---

##  Setup Instructions

### 1. Prerequisites
- **Visual Studio 2022** or **VS Code** with .NET 8 SDK.
- **SQL Server Express** installed and running locally.
- A **Twilio Account** (for 2FA functionality).

### 2. Database Setup
Run the following SQL script in your SQL Server Management Studio (SSMS) to create the necessary table:

```sql
CREATE DATABASE QuestDB;
GO

USE QuestDB;
GO

CREATE TABLE Quests (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Status NVARCHAR(50),
    DueDate DATETIME,
    Priority NVARCHAR(50)
);
```

### 3. Environment Configuration
The project uses a `.env` file for sensitive credentials. **Twilio is required** if you want to use the Two-Factor Authentication (2FA) feature.

1.  Create a file named `.env` in the `QuestTracker/QuestTracker` directory.
2.  Log in to your [Twilio Console](https://www.twilio.com/console).
3.  Copy your **Account SID**, **Auth Token**, and **Twilio Phone Number**.
4.  Paste them into the `.env` file using this format:

```env
TWILIO_ACCOUNT_SID=ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_AUTH_TOKEN=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_PHONE_NUMBER=+1234567890
```

*Note: If you do not wish to use Twilio, you must modify `Authenticator.cs` to skip the `SendSms2FA` method calls.*

### 4. Installation & Packages
The project relies on NuGet packages for SQL connectivity and Twilio integration. These are automatically managed by the `.csproj` file.

1.  Open a terminal in the project root.
2.  Run the following command to download and install all required libraries (including Twilio and SqlClient):
    ```bash
    dotnet restore
    ```
3.  Build the project to ensure everything is linked correctly:
    ```bash
    dotnet build
    ```

### 5. Database Connection
Update the `connectionString` variable in `Program.cs` and `QuestManager.cs` if your SQL Server instance uses a specific instance name or port:
- **Default (Local DB)**: `Server=.\SQLEXPRESS;Database=QuestDB;Trusted_Connection=True;TrustServerCertificate=True;`
- **Full SQL Server**: Replace `.\SQLEXPRESS` with your server name (e.g., `LOCALHOST` or your machine name).

### 5. Running the App
Press `F5` in Visual Studio or use the CLI:
```bash
dotnet run --project QuestTracker/QuestTracker
```

---

##  How to Use
1. **Register/Login**: Start the app and create a "Hero" account.
2. **2FA**: Enter the code sent to your phone using the visual verification interface.
3. **Manage Quests**: Use the Guild Menu to add new missions, mark them as completed, or view your progress report.
4. **Deadlines**: Keep an eye on the console for notifications regarding urgent missions!

##  License
This project is part of a training exercise for C# development.
