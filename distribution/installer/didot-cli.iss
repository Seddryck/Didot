#define AppName "Didot"
#define AppPublisher "Seddryck"
#define AppExeName "didot.exe"

#ifndef AppVersion
  #define AppVersion "0.0.0-local"
#endif

#ifndef RuntimeIdentifier
    #define RuntimeIdentifier "win-x64"
#endif

#ifndef TargetFramework
    #define TargetFramework "net10.0"
#endif

[Setup]
AppId={{9F6D1A4C-5E73-4A91-BB8F-2F3D7C8E6A42}}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL=https://github.com/Seddryck/Didot
AppSupportURL=https://github.com/Seddryck/Didot/issues
AppUpdatesURL=https://github.com/Seddryck/Didot/releases
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=..\..\.publish\
OutputBaseFilename=Didot-{#AppVersion}-{#RuntimeIdentifier}-setup
Compression=lzma
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
UninstallDisplayIcon={app}\{#AppExeName}

[Files]
Source: "..\..\.publish\{#TargetFramework}\{#RuntimeIdentifier}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Didot Command Prompt"; Filename: "{cmd}"; Parameters: "/K cd /d ""{app}"""

[Registry]
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; \
    ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}"; \
    Check: NeedsAddPath(ExpandConstant('{app}'))

[Code]
function NeedsAddPath(Path: string): Boolean;
var
  CurrentPath: string;
begin
  if not RegQueryStringValue(
    HKEY_LOCAL_MACHINE,
    'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
    'Path',
    CurrentPath
  ) then
    CurrentPath := '';

  Result := Pos(';' + Uppercase(Path) + ';', ';' + Uppercase(CurrentPath) + ';') = 0;
end;