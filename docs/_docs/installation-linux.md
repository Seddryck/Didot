---
title: Install from GitHub Releases on Linux
tags: [installation]
---
## Step 1: Download the ZIP from the GitHub Release

1. Navigate to the **GitHub repository** of [Didot](https://github.com/Seddryck/Didot).
2. Go to the **Releases** section, typically found under the "Code" tab.
3. Download the `.zip` file for the desired release.

Example:

```bash
wget https://github.com/Seddryck/Didot/releases/download/v0.13.0/Didot-0.13.0-net7.0-linux-x64.zip
```

## Step 2: Extract the ZIP File

1. Open a terminal and navigate to the directory where you downloaded the `.zip` file.
2. Use the `unzip` command to extract the contents. You can choose any directory for extraction (e.g., `/usr/local/bin/Didot`).

```bash
unzip didot.zip -d /usr/local/bin/Didot
```

> **Tip**: Ensure the directory you extract to is accessible and easy to remember.

## Step 3: Add the Executable to the System PATH

To run the executable from any location in the terminal, you need to add its folder to your system's PATH.

1. Open your `~/.bashrc` (or `~/.zshrc` depending on your shell) file with a text editor.

```bash
nano ~/.bashrc
```

2. Add the following line at the end of the file to add the Didot directory to your PATH:

```bash
export PATH="/usr/local/bin/Didot:$PATH"
```

3. Save the file and reload your shell configuration:

```bash
source ~/.bashrc
```

## Step 4: Verify Installation

1. Open a new terminal.
2. Type the name of the executable (e.g., `didot`) and hit Enter.
3. If everything is set up correctly, the program should run.
