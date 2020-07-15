﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
            // Restrict index range - should be at least 0 and less than the number of elements in notes.
            int startIndex = Math.Max(baseIndex - this.Settings.previousSplits, 0);
            int endIndex = Math.Min(baseIndex + this.Settings.nextSplits, this.Notes.Count());

            List<string> notes = this.Notes.GetRange(startIndex, endIndex - startIndex);
            return String.Join("\n<hr>\n", notes);
        }

        
        /// <summary>
        /// Read the notes from the file system and split them into segments based on the separator
        /// </summary>
        private List<string> ReadNotes()
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

            return processedNotes;
        }
    }
}