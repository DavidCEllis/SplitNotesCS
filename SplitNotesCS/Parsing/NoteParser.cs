using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Net;
using Ganss.XSS;

namespace SplitNotesCS.Parsing
{
    class NoteParser
    {
        private List<string> Notes; // The notes themselves

        public string NotePath { get; private set; }  // Source file used for notes
        public string Separator { get; private set; }  // Note separator
        readonly string fileExt;  // File extension (used to decide how to format)
        
        // If a line starts and ends with these characters it will be skipped in parsing
        private string commentStart = "[";
        private string commentEnd = "]";

        public NoteParser(string inputPath, string noteSeparator = "")
        {
            this.NotePath = inputPath;
            this.fileExt = Path.GetExtension(inputPath);

            this.Separator = noteSeparator;

            this.Notes = ReadNotes();
        }

        public string getNotes(int startIndex, int endIndex)
        {
            List<string> notes = this.Notes.GetRange(startIndex, endIndex - startIndex);
            return String.Join("\n<hr>\n", notes);
        }

        
        /// <summary>
        /// Read the notes from the file system and split them into segments based on the separator
        /// </summary>
        /// <param name="separator">Delimiter between segments in the notes</param>
        public List<string> ReadNotes()
        {
            // Output Data List
            var processedNotes = new List<string>();
            
            using (var noteFile = new StreamReader(NotePath))
            {

                var split = new List<string>();
                string line;

                var sanitizer = new Sanitizer();

                // Read from the file and separate the text into blocks by split
                while ((line = noteFile.ReadLine()) != null)
                {

                    line = line.Trim();
                    // Skip Commented lines
                    if (line.StartsWith(commentStart) && line.EndsWith(commentEnd)) continue;

                    // What to do if we've reached the end of a block
                    if (line == this.Separator)
                    {
                        // Special case if the separator is blank and the split is empty, don't add the split
                        // So 2 blank lines won't end up adding a blank split if newline is the separator
                        if ((this.Separator == "") && (split.Count() == 0)) continue;

                        // Reconnect with newlines, process text/html/markdown, append and clear
                        string splitString = String.Join("\n", split);
                        splitString = sanitizer.Prepare(splitString, this.fileExt);
                        processedNotes.Add(splitString);
                        split.Clear();    
                    } 
                    else
                    {
                        split.Add(line);
                    }
                }
                // On completion - add the final segment if it is not empty
                if (split.Count > 0)
                {
                    string splitString = String.Join("\n", split);
                    splitString = sanitizer.Prepare(splitString, this.fileExt);
                    processedNotes.Add(splitString);
                    split.Clear();
                }

            }

            return processedNotes;
        }
    }
}
