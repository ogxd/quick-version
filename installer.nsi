!define VERSION "1.0"

; The name of the installer
Name "Quick Version"

; The file to write
OutFile "QuickVersion-${VERSION}-win64.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\Quick Version"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "SOFTWARE\Quick Version" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

; The stuff to install
Section "Quick Version (required)"

  SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  File "Tools\ComTool.exe"
  File "QuickVersion\bin\QuickVersion.dll"
  File "QuickVersion\bin\SharpShell.dll"

  Exec '"$INSTDIR\ComTool.exe" install register "$INSTDIR\QuickVersion.dll"'

  ; Write the installation path into the registry
  WriteRegStr HKLM "SOFTWARE\Quick Version" "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Quick Version" "DisplayName" "Quick Version"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Quick Version" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Quick Version" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Quick Version" "NoRepair" 1
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
SectionEnd

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Quick Version"
  DeleteRegKey HKLM "SOFTWARE\Quick Version"

  ; Unregister COM server
  ExecWait '"$INSTDIR\ComTool.exe" uninstall unregister "$INSTDIR\QuickVersion.dll"'

  ; Remove directories used
  RMDir "$SMPROGRAMS\Quick Version"
  RMDir /r "$INSTDIR"

SectionEnd