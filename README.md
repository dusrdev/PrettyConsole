# PrettyConsole

An abstraction over `System.Console` that adds new input and output methods, colors and advanced outputs like progress bars and menus. And everything is ansi supported so it works on legacy systems and terminals.

## Features

* 🚀 High performance, Low memory usage and allocation
* 🪶 Very lightweight (No extenal dependencies)
* Easy to use (no need to learn a new syntax while still writing less boilerplate code)
* 🔌 Plug and play (most of the time you don't need to change your code much)
* 💾 Supports legacy ansi terminals (like Windows 7)
* ✂ Trimming friendly (documented trim warnings and working alternatives for everything)
* Supports all platforms (Windows, Linux, Mac)
* 😎 Beautifully uniform out of the box (Default colors that can be customized)
* ⛓ Uses original output pipes, so that your cli's can be piped properly.

## ⬇ Installation

* The last stable release will be available in the releases section in a .dll format.
* Nuget   [![](https://img.shields.io/nuget/dt/PrettyConsole?label=Downloads)](https://www.nuget.org/packages/PrettyConsole/)

## 📺 Output Examples

![Github combo](https://user-images.githubusercontent.com/8972626/205509917-d47e8967-a4b6-4fdb-bae1-32f0adfb4f91.png)

## Usage

The most convinient way to use this package is to add this using statement: `using static PrettyConsole.Console;`  
Than use most of the default method signatures such as `Write`, `WriteLine`, `ReadLine` and so on, conviniently they are named exactly the same as the regular c# versions.  
For more information about these methods and the platora of additinal ones, check the [Wiki](https://github.com/dusrdev/PrettyConsole/wiki).

## Source Code

from the point of public release, the master branch will only contain stable and tested code, so
to get the source code you can clone the master branch.
