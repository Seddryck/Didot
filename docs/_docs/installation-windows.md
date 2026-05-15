---
title: Install on Windows from installer available GitHub Releases
tags: [quick-start, installation]
---
## Installation

### Step 1: Download the installer from the GitHub Release

1. Navigate to the **GitHub repository** of [the project](https://github.com/Seddryck/Didot).
2. Go to the **Releases** section, usually found under the "Code" tab.
3. Download the `-setup.exe` file containing the installer from the desired release.

Example:

   ```powershell
   https://github.com/Seddryck/Didot/releases/download/v0.33.0/Didot-0.33.0-win-x64-setup.exe
   ```

### Step 2: Run the installer

1. Double-click the downloaded `.exe` file.
2. Select the directory to install the application
3. Select the Start Menu folder
4. Click on Install

The executable can be run from any location in the command line, as instalation folder has been added to your system's PATH.

### Step 3: Verify Installation

1. Open **Command Prompt** (CMD).
2. Type the name of the executable (e.g., `didot.exe`) and hit Enter.
3. If everything is set up correctly, the program should run.

## Installation options

### /SILENT

```powershell
Didot-setup.exe /SILENT
```
Install without wizard pages but progress window still visible

### /VERYSILENT

```powershell
Didot-setup.exe /VERYSILENT
```
Install almost completely hidden, preferred for automation

### /NORESTART

```powershell
Didot-setup.exe /NORESTART
```
Prevents automatic reboot.

### Very common in CI and WinGet

```powershell
Didot-setup.exe /VERYSILENT /NORESTART
```

## Uninstallation

1. Go to "Add or remove programs"
2. Find Didot in the list
3. Click on "..." and select "Uninstall"