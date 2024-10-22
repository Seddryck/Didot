---
title: Install from GitHub Releases on Windows
tags: [quick-start, installation]
---
## Step 1: Download the ZIP from the GitHub Release

1. Navigate to the **GitHub repository** of [the project](https://github.com/Seddryck/Didot).
2. Go to the **Releases** section, usually found under the "Code" tab.
3. Download the `.zip` file containing the executable from the desired release.

Example:

   ```pwsh
   https://github.com/Seddryck/Didot/releases/download/v0.13.0/Didot-0.13.0-net7.0-win-x64.zip
   ```

## Step 2: Extract the ZIP File

1. Right-click the downloaded `.zip` file and choose **Extract All**.
2. Extract the contents to a directory of your choice, such as `C:\Program Files\Didot`.

> **Tip**: Choose a path that is easy to remember and doesn't contain special characters.

## Step 3: Add the Executable to the System PATH

To run the executable from any location in the command line, you need to add its folder to your system's PATH.

1. Open the **Start Menu** and search for **Environment Variables**.
2. Click **Edit the system environment variables**.
3. In the **System Properties** window, click **Environment Variables**.
4. In the **System Variables** section, scroll down, select **Path**, and click **Edit**.
5. In the **Edit Environment Variable** dialog, click **New** and enter the path to your extracted folder, e.g., `C:\Program Files\Didot`.
6. Click **OK** to close all windows.

## Step 4: Verify Installation

1. Open **Command Prompt** (CMD).
2. Type the name of the executable (e.g., `didot.exe`) and hit Enter.
3. If everything is set up correctly, the program should run.