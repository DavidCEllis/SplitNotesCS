using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core;
using Stubble.Core.Builders;

namespace SplitNotesCS.Parsing
{
    /// <summary>
    /// This class handles putting the notes provided into an HTML template
    /// </summary>
    class Templater
    {
        private readonly Properties.Settings Settings;
        private readonly string CSS;
        private readonly string BaseTemplate;

        private readonly StubbleVisitorRenderer Renderer = new StubbleBuilder().Build();

        public Templater(Properties.Settings settings)
        {
            this.Settings = settings;

            // Read the templates into memory as strings
            this.CSS = this.GetCSS();
            this.BaseTemplate = this.GetBaseTemplate();
        }

        private string GetCSS ()
        {
            string cssData;
            string cssPath = Path.Combine(this.Settings.cssTemplateFolder, this.Settings.cssTemplateFile);
            using (var cssFile = new StreamReader(cssPath))
            {
                cssData = cssFile.ReadToEnd();
            }
            return cssData;
        }

        private string GetBaseTemplate ()
        {
            string htmlTemplate;
            string htmlPath = Path.Combine(this.Settings.htmlTemplateFolder, this.Settings.htmlTemplateFile);
            using (var htmlFile = new StreamReader(htmlPath))
            {
                htmlTemplate = htmlFile.ReadToEnd();
            }
            return htmlTemplate;
        }

        /// <summary>
        /// Render the input text with everything into an HTML String
        /// </summary>
        /// <param name="splitText">The split data</param>
        /// <returns></returns>
        public string RenderTemplate(string splitText)
        {
            
            var renderSource = new Dictionary<string, string>()
            {
                { "font_size", this.Settings.fontSize.ToString() },
                { "text_color", this.Settings.textColor },
                { "bg_color", this.Settings.backgroundColor },
                { "css", this.CSS },
                { "splits", splitText }
            };

            return this.Renderer.Render(this.BaseTemplate, renderSource);

        }
    }
}
