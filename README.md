# WinDock v0.6-alpha

WinDock is a graphical application launcher written in C#/WPF for Windows&copy; in the style of [Docky](http://wiki.go-docky.com) or Cairo-Dock (http://glx-dock.org).
It is currently a work-in-progress and is not ready for general use.

## Features

* Full shell integration. This includes JumpLists (the list of items shown when a Taskbar button is right-clicked), Notification Area (the area on the right of the taskbar used for notifications and background applications), Live Thumbnail Previews, progress indicators, and Aero Peek.
* Extensibility. A new type of dock item can be created simply by extending the DockItem class and implementing a few methods.
* Customisability. Any number of docks can be created each with its own theme. If you don't like the built-in icon for an application, a new one can be selected.
* Functionality. WinDock attempts to provide all the functionality of the standard taskbar while enabling added functionality through custom widgets and styling.

## Building WinDock

Building WinDock should be easy, though I haven't yet tested it on many environments. It requires a minimum of Microsoft&copy; .NET Framwork v4 and Windows 7.