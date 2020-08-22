using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Diagnostics;
using Ganss.XSS;

namespace SplitNotesCS.Parsing
{
    class NoteManager
    {
        private readonly List<string> Notes; // The notes themselves

        public string NotePath { get; private set; }  // Source file used for notes

        private readonly Properties.Settings Settings;
        private readonly string FileExt;  // File extension (used to decide how to format)

        // If a line starts and ends with these characters it will be skipped in parsing
        private string commentStart = "[";
        private string commentEnd = "]";

        public NoteManager(string notePath, Properties.Settings settings)
        {
            this.NotePath = notePath;
            this.FileExt = Path.GetExtension(notePath);

            this.Settings = settings;
            this.Notes = ReadNotes();
        }

        /// <summary>
        /// Obtain the raw notes, with segments separated by <hr>
        /// </summary>
        /// <param name="baseIndex">Index of the main note page</param>
        /// <returns></returns>
        public string GetRawNotes(int baseIndex)
        {
            if (baseIndex >= this.Notes.Count())
            {
                return "<h1>End of Notes Reached</h1>";
            }
            else
            {
                // Basic values for start and end index
                int startIndex = baseIndex - this.Settings.previousSplits;
                int endIndex = baseIndex + this.Settings.nextSplits + 1;

                // Restricting the ranges
                startIndex = Math.Min(Math.Max(startIndex, 0), this.Notes.Count());
                endIndex = Math.Max(Math.Min(endIndex, this.Notes.Count()), startIndex);

                List<string> notes = this.Notes.GetRange(startIndex, endIndex - startIndex);
                return String.Join("\n<hr>\n", notes);
            }
        }

        
        /// <summary>
        /// Read the notes from the file system and split them into segments based on the separator
        /// </summary>
        private List<string> ReadNotes()
        {
            // Output Data List
            var processedNotes = new List<string>();

            try
            {
                using (var noteFile = new StreamReader(this.NotePath))
                {

                    var split = new List<string>();
                    string line;

                    var sanitizer = new Sanitizer();

                    // Read from the file and separate the text into blocks by split
                    while ((line = noteFile.ReadLine()) != null)
                    {

                        // Skip Commented lines
                        if (line.StartsWith(commentStart) && line.EndsWith(commentEnd)) continue;

                        // What to do if we've reached the end of a block
                        if (line == this.Settings.splitSeparator)
                        {
                            // Special case if the separator is blank and the split is empty, don't add the split
                            // So 2 blank lines won't end up adding a blank split if newline is the separator
                            if ((this.Settings.splitSeparator == "") && (split.Count() == 0)) continue;

                            // Reconnect with newlines, process text/html/markdown, append and clear
                            string splitString = String.Join("\n", split);
                            splitString = sanitizer.Prepare(splitString, this.FileExt);
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
                        splitString = sanitizer.Prepare(splitString, this.FileExt);
                        processedNotes.Add(splitString);
                        split.Clear();
                    }

                }
            }
            catch (FileNotFoundException)
            {
                // I guess if you managed to get a file that didn't exist in the open file dialog you get here?
                processedNotes.Add($"Could not find notes file {this.NotePath}");
            }
            return processedNotes;
        }
    }
}
