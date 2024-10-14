# Didot

![Logo](https://raw.githubusercontent.com/Seddryck/Didot/main/assets/didot-logo-256.png)

Transform your structured YAML data into beautiful, fully-customized HTML pages in seconds with Didot. This command-line tool allows you to seamlessly generate HTML from YAML files using your preferred templates. Whether you're building static sites, documentation, or reporting tools, Didot makes it easy to turn raw data into polished, web-ready content.

[About][] | [Installing][] | [Quickstart][]

[About]: #about (About)
[Installing]: #installing (Installing)
[Quickstart]: #quickstart (Quickstart)

## About

**Social media:** [![website](https://img.shields.io/badge/website-seddryck.github.io/Didot-fe762d.svg)](https://seddryck.github.io/Didot)
[![twitter badge](https://img.shields.io/badge/twitter%20Didot-@Seddryck-blue.svg?style=flat&logo=twitter)](https://twitter.com/Seddryck)

**Releases:** [![nuget](https://img.shields.io/nuget/v/Didot.svg)](https://www.nuget.org/packages/Didot/) <!-- [![GitHub Release Date](https://img.shields.io/github/release-date/seddryck/Didot.svg)](https://github.com/Seddryck/Didot/releases/latest) --> [![licence badge](https://img.shields.io/badge/License-Apache%202.0-yellow.svg)](https://github.com/Seddryck/Didot/blob/master/LICENSE)

**Dev. activity:** [![GitHub last commit](https://img.shields.io/github/last-commit/Seddryck/Didot.svg)](https://github.com/Seddryck/Didot/commits)
![Still maintained](https://img.shields.io/maintenance/yes/2024.svg)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/Seddryck/Didot)

**Continuous integration builds:** [![Build status](https://ci.appveyor.com/api/projects/status/na3dklqjsuv1lbfv?svg=true)](https://ci.appveyor.com/project/Seddryck/Didot/)
[![Tests](https://img.shields.io/appveyor/tests/seddryck/Didot.svg)](https://ci.appveyor.com/project/Seddryck/Didot/build/tests)
[![CodeFactor](https://www.codefactor.io/repository/github/seddryck/Didot/badge)](https://www.codefactor.io/repository/github/seddryck/Didot)
[![codecov](https://codecov.io/github/Seddryck/Didot/branch/main/graph/badge.svg?token=YRA8IRIJYV)](https://codecov.io/github/Seddryck/Didot)
<!-- [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSeddryck%2FDidot.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FSeddryck%2FDidot?ref=badge_shield) -->

**Status:** [![stars badge](https://img.shields.io/github/stars/Seddryck/Didot.svg)](https://github.com/Seddryck/Didot/stargazers)
[![Bugs badge](https://img.shields.io/github/issues/Seddryck/Didot/bug.svg?color=red&label=Bugs)](https://github.com/Seddryck/Didot/issues?utf8=%E2%9C%93&q=is:issue+is:open+label:bug+)
[![Top language](https://img.shields.io/github/languages/top/seddryck/Didot.svg)](https://github.com/Seddryck/Didot/search?l=C%23)

## Installing

```bash
dotnet tool install -g Didot-cli
```

## QuickStart

The command is named `didot`. Don't forget to append the following required arguments:

```bash
  -t, --Template    Required. Path to the template file.
  -s, --Source      Required. Path to the source file.
  -o, --Output      Required. Path to the generated file
```
