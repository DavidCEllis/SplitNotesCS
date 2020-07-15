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

Inspired by (but otherwise unassociated with) the original splitnotes: https://github.com/joeloskarsson/SplitNotes

I also released a previous version written in Python: https://github.com/DavidCEllis/SplitNotes-2
The goal of this version is to reduce the distributable size (and to learn some C#). The python version when distributed was a somewhat ridiculous 300MB.
