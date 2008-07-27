using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace HamlEditor
{

    public enum HamlTokenColor
    {
        CsComment = 1,
        CsString,
        CsNumber,
        HamlText,
        HamlCode,
        HamlComment,
        HamlPartial,
        HamlTag,
        HamlTagClass,
        HamlTagId,
        HamlWhitespace,
    }

    // This attribute indicates that this managed type is visible to COM
    [ComVisible(true)]
    [Guid(Constants.LanguageServiceGuid)]
    public class HamlLanguageService : LanguageService
    {
        private HamlScanner _scanner;
        private LanguagePreferences _preferences;
        private readonly ColorableItem[] _colorableItems;


        public HamlLanguageService()
        {
            _colorableItems = new[]
                                  {
              ColorableItem("Comment", COLORINDEX.CI_GREEN), 
              ColorableItem("String", COLORINDEX.CI_MAROON), 
              ColorableItem("Number", COLORINDEX.CI_PURPLE), 
              ColorableItem("Haml Text", COLORINDEX.CI_SYSPLAINTEXT_FG), 
              ColorableItem("Haml Code", COLORINDEX.CI_AQUAMARINE), 
              ColorableItem("Haml Comment", COLORINDEX.CI_LIGHTGRAY), 
              ColorableItem("Haml Partial", COLORINDEX.CI_PURPLE), 
              ColorableItem("Haml Tag", COLORINDEX.CI_BLUE), 
              ColorableItem("Haml Tag Class", COLORINDEX.CI_YELLOW), 
              ColorableItem("Haml Tag Id", COLORINDEX.CI_RED), 
              ColorableItem("Haml Whitespace", COLORINDEX.CI_PURPLE), 
                                  };

        }

        private static ColorableItem ColorableItem(string name, COLORINDEX fg)
        {
            return new ColorableItem(name, name, fg, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT);
        }

        /* For coloring */
        public override int GetItemCount(out int count)
        {
            count = _colorableItems.Length;
            return VSConstants.S_OK;
        }

        public override int GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index < 1 || index > _colorableItems.Length)
            {
                item = null;
                return VSConstants.S_FALSE;
            }
            item = _colorableItems[index - 1];
            return VSConstants.S_OK;
        }        /*  */

        /// <summary>
        /// This method parses the source code based on the specified ParseRequest object.
        /// We don't need implement any logic here.
        /// </summary>
        /// <param name="req">The <see cref="ParseRequest"/> describes how to parse the source file.</param>
        /// <returns>If successful, returns an <see cref="AuthoringScope"/> object; otherwise, returns a null value.</returns>
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return null;
        }

        /// <summary>
        /// Language name property.
        /// </summary>        
        public override string Name
        {
            get { return "Haml Language Service"; }
        }

        /// <summary>
        /// Returns a string with the list of the supported file extensions for this language service.
        /// </summary>
        /// <returns>Returns a LanguagePreferences object</returns>
        public override string GetFormatFilterList()
        {
            return "Haml File (*.haml)\t*.haml";
        }

        /// <summary>
        /// Create and return instantiation of a parser represented by RegularExpressionScanner object.
        /// </summary>
        /// <param name="buffer">An <see cref="IVsTextLines"/> represents lines of source to parse.</param>
        /// <returns>Returns a RegularExpressionScanner object</returns>
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (_scanner == null)
            {
                _scanner = new HamlScanner();
            }
            return _scanner;
        }

        /// <summary>
        /// Returns a <see cref="LanguagePreferences"/> object for this language service.
        /// </summary>
        /// <returns>Returns a LanguagePreferences object</returns>
        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(
                    Site, 
                    typeof(HamlLanguageService).GUID, 
                    "Haml Language Service");
            }

            return _preferences;
        }


    }
}
