# PrettyConsole

## Description

An abstraction over System.Console that adds new input methods and output methods, and generally makes everything prettier. and all that, without using advanced ansi protocols to allow support on older machines

## Installation

you can download this package to use in these two methods:

* The last stable release will be available in the releases section in a .dll format.
* Get it from nuget package manager

## Target

This package can be very useful to anyone who is interested in creating beautiful cli (command-line) tools in .net.  
It not only provides the default input and output methods, but also has very intuitive color customization options, and  
more advanced input/output combination options like selections, multi-selection and even a tree-menu.  
And whats more is that by using this package you can write less code, that performs better.

## Usage

The best way to use this package is to add this using statement:  
`using static PrettyConsole.Console;`  
that will allow you to use the methods directly, which means it won't interfere with `System.Console`.  
for convenience, all methods that are abstractions on top of `System.Console` carry the same names, such as: `Write()`, `WriteLine()`, `ReadLine()` and so on.  
For more information of these methods and the added ones, check the Wiki.

## Source Code

from the point of public release, the master branch will only contain stable and tested code, so
to get the source code you can clone the master branch.
