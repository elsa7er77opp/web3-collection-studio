#define AppName "CyberBank Web3 Collection Studio"
#define AppVersion "1.0.0"
#define AppPublisher "CyberBank Labs"
#define AppExeName "Web3CollectionStudio.Desktop.exe"
#define ProductFolder "CyberBank\Web3 Collection Studio"
#define BuildRoot "..\artifacts\Web3CollectionStudio-Final-win-x64"
#define BrandIcon "..\src\Web3CollectionStudio.Desktop\Assets\Brand\collection-studio-app-icon.ico"

[Setup]
AppId={{8CFA2D13-3CD2-4FCF-9C8A-CB6DA03E8917}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\Programs\{#ProductFolder}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir=..\artifacts\installer
OutputBaseFilename=CyberBank-Web3-Collection-Studio-Final-Setup-win-x64
SetupIconFile={#BrandIcon}
UninstallDisplayIcon={app}\{#AppExeName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=lowest
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "{#BuildRoot}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"; IconFilename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; IconFilename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent
