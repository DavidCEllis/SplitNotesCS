# SplitNotesCS #

SplitNotesCS is an application for displaying speedrun notes in sync with livesplit. Requires livesplit server to be running.

## Install/Setup ##

1. Under the Livesplit layout editor add 'LiveSplit Server' (listed under 'control')
2. Download SplitNotesCS from the releases page
3. Extract anywhere and run SplitNotesCS.exe

## Usage ##

1. Connect with livesplit by starting the livesplit server component selecting 
   'Control' and 'Start Server'
2. From the file menu select 'Open Notes' and find the file
   containing the notes you wish to use.
   
The formatting for notes basically follows the format used by the original SplitNotes 
with some additional enhancements and exceptions.

Markdown and HTML formatted notes are supported.
Text format notes will automatically have line breaks inserted in between lines.

1. Comment lines still use square brackets.
2. By default splits will break on newlines, multiple newlines are ignored.
3. The rendering is done as HTML so HTML formatting can be used.

## Configuration ##

The settings page offers some customisation and connection settings including:

* Server hostname and port
* Show previous/next N splits
* Custom split separator
* Base font size
* Default text and background colour

---

## Why? Splitnotes/SpeedGuidesLive/Splitnotes2 already exists ##

[SplitNotes](https://github.com/joeloskarsson/SplitNotes) is the original version of splitnotes.
[SplitNotes-2](https://github.com/DavidCEllis/SplitNotes-2) Was a version I remade in python using PyQt
[SpeedGuidesLive](https://www.nightgamedev.com/sgl) is a closed source livesplit component and making notes appears to require editing inside Livesplit rather than a standard text editor (I have not tried it).

The original version didn't allow for some more fancy formatting that I wanted in my notes. 
In order to allow for more formatting options the notes are now converted to HTML and rendered by a browser.
The python version ended up using a QWebEngineView - however this made the distributable overly large (~300MB).
This version uses WPF and takes advantage of the WebBrowser control which makes a much smaller distributable (~3MB).
