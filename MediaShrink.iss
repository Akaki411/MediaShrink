; Inno Setup script: packs bin\Release into bin\Setup\MediaShrink-Setup-<version>.exe
; Build first (build.bat), then run pack.bat (or open this file in Inno Setup and press F9).

#define AppName "MediaShrink"
#define AppExe "MediaShrink.exe"
#define Src "bin\Release"
#define AppVer GetVersionNumbersString(SourcePath + Src + "\" + AppExe)

[Setup]
AppId={{B5C1F8E2-6D0A-4C55-9C3E-2E7A1F4D9A31}
AppName={#AppName}
AppVersion={#AppVer}
AppPublisher={#AppName}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir=bin\Setup
OutputBaseFilename={#AppName}-Setup-{#AppVer}
UninstallDisplayIcon={app}\{#AppExe}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
MinVersion=6.1sp1
PrivilegesRequiredOverridesAllowed=dialog
#if Ver >= 0x06030000
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#else
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
#endif

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[CustomMessages]
russian.NeedNet=Для работы {#AppName} нужен .NET Framework 4.8. Установите его и запустите установку снова.
english.NeedNet={#AppName} needs .NET Framework 4.8. Install it and run this setup again.
russian.DesktopIcon=Создать значок на рабочем столе
english.DesktopIcon=Create a desktop icon

[Tasks]
Name: "desktopicon"; Description: "{cm:DesktopIcon}"; Flags: unchecked

[Files]
; Only the x64 Magick native library is needed: the program is 64-bit only (its ffmpeg is x64).
Source: "{#Src}\*"; DestDir: "{app}"; Excludes: "*.pdb,Magick.Native-Q8-arm64.dll,Magick.Native-Q8-x86.dll"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; ffmpeg is unpacked here on the first start.
Type: filesandordirs; Name: "{localappdata}\{#AppName}"

[Code]
function InitializeSetup(): Boolean;
var
  Release: Cardinal;
begin
  Result := True;
  if not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release)
     or (Release < 528040) then
  begin
    MsgBox(CustomMessage('NeedNet'), mbError, MB_OK);
    Result := False;
  end;
end;
