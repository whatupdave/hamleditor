using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace HamlEditor
{
//    public class HamlToken
//    {
////        public static List<HamlToken> Tokens
////        {
////            get
////            {
////                if (_tokens == null)
////                {
////                    _tokens = new List<HamlToken>
////                              {
////                                  new HamlToken("Haml Code", COLORINDEX.CI_AQUAMARINE, COLORINDEX.CI_USERTEXT_BK),
////                                  new HamlToken("Haml Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK),
////                                  new HamlToken("Haml Partial", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK),
////                                  new HamlToken("Haml Tag", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK),
////                                  new HamlToken("Haml Tag Class", COLORINDEX.CI_YELLOW, COLORINDEX.CI_USERTEXT_BK),
////                                  new HamlToken("Haml Tag Id", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK),
////                                  new HamlToken("Haml Text", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK),
////                                  new HamlToken("Haml Whitespace", COLORINDEX.CI_USERTEXT_FG, COLORINDEX.CI_USERTEXT_BK),
////                              };
////                }
////
////                for (int i = 0; i < _tokens.Count; i++)
////                {
////                    _tokens[i]._tokenIndex = i;
////                }
////
////                return _tokens;
////            }
////        }
//
//        public string DisplayName
//        {
//            get { return _displayName; }
//        }
//
//        public static HamlToken Code()      { return new HamlToken(""); }
//        public static HamlToken Comment()   { return Tokens[1]; }
//        public static HamlToken Partial()   { return Tokens[2]; }
//        public static HamlToken Tag()       { return Tokens[3]; }
//        public static HamlToken TagClass()  { return Tokens[4]; }
//        public static HamlToken TagId()     { return Tokens[5]; }
//        public static HamlToken Text()      { return Tokens[6]; }
//        public static HamlToken Whitespace(){ return Tokens[7]; }
//
//        private readonly COLORINDEX _background;
//        private readonly string _displayName;
//        private readonly COLORINDEX _foreground;
//        private static List<HamlToken> _tokens;
//        private int _tokenIndex;
//
//        public HamlToken(string displayName, COLORINDEX foreground, COLORINDEX background)
//        {
//            _background = background;
//            _displayName = displayName;
//            _foreground = foreground;
//        }
//
//        public void Set(TokenInfo info)
//        {
//            info.Color = (TokenColor)_tokenIndex;
//        }
//
//        public TokenColor Color
//        {
//            get { return (TokenColor) _tokenIndex; }
//        }
//    }
}