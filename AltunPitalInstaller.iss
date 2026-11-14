[Setup]
AppName=AltunPital
AppVersion=1.0
DefaultDirName={localappdata}\Programs\AltunPital
DefaultGroupName=AltunPital
OutputDir=Installer
OutputBaseFilename=AltunPital_Setup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
WizardStyle=modern

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\AltunPital"; Filename: "{app}\AltunPital.exe"
Name: "{autodesktop}\AltunPital"; Filename: "{app}\AltunPital.exe"

[Run]
Filename: "{app}\AltunPital.exe"; Description: "AltunPital'i Başlat"; Flags: nowait postinstall skipifsilent