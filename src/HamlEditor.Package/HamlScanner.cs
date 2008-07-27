using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Package;

namespace HamlEditor
{
    public enum HamlState
    {
        BeforeTag,
        AfterNonCodeTag,
        AfterPartial,
        AfterCodeTag,
        InTag
    }

    /// <summary>
    /// This class implements IScanner interface and performs
    /// text parsing on the base of rules' table. 
    /// </summary>
    public class HamlScanner : IScanner
    {
        private string _currentLine;
        private int _currentPos;

        private HamlState _state;

        /// <summary>
        /// This method is used to parse next language token from the current line and return information about it.
        /// </summary>
        /// <param name="tokenInfo"> The TokenInfo structure to be filled in.</param>
        /// <param name="state"> The scanner's current state value.</param>
        /// <returns>Returns true if a token was parsed from the current line and information returned;
        /// otherwise, returns false indicating no more tokens are on the current line.</returns>
        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            if (_currentLine == "" || RestOfLine.Length == 0)
                return false;

            var charsMatched = 0;

            switch (_state)
            {
                case HamlState.BeforeTag:
                    switch (RestOfLine[0])
                    {
                        case ' ':
                            charsMatched = InterpretWhiteSpace(tokenInfo);
                            break;
                        case '%':
                        case '#':
                        case '.':
                            charsMatched = InterpretTag(tokenInfo);
                            break;
                        case '=':
                        case '-':
                            charsMatched = InterpretAfterCodeTag(tokenInfo);
                            break;

                        case '_':
                            charsMatched = InterpretPartial(tokenInfo);
                            break;
                        case '!':
                            SetColor(tokenInfo, HamlTokenColor.HamlTag);
                            charsMatched = RestOfLine.Length;
                            break;

                        default:
                            return false;
                    }
                    break;

                case HamlState.AfterCodeTag:
                    charsMatched = InterpretAfterCodeTag(tokenInfo);
                    break;
                case HamlState.AfterNonCodeTag:
                    charsMatched = InterpretAfterNonCodeTag(tokenInfo);
                    break;

                case HamlState.AfterPartial:
                    charsMatched = InterpretPartial(tokenInfo);
                    break;
                case HamlState.InTag:
                    charsMatched = InterpretTagIdOrClass(tokenInfo);
                    break;

            }
            
            tokenInfo.StartIndex = _currentPos;
            tokenInfo.EndIndex = _currentPos + charsMatched - 1;

            _currentPos = tokenInfo.EndIndex + 1;

            return true;
        }

        private int InterpretTag(TokenInfo info)
        {
            SetColor(info, HamlTokenColor.HamlTag);

            var match = Regex.Match(RestOfLine, @"\s*[\s|=|\-|\{|#]");

            if (!match.Success)
                return RestOfLine.Length;

            int matchCount = match.Index;
            switch (match.Value.Trim())
            {
                case "=":
                case "-":
                case "{":
                    _state = HamlState.AfterCodeTag;
                    break;
                case "#":
                    _state = HamlState.InTag;
                    break;
                default:
                    _state = HamlState.AfterNonCodeTag;
                    break;
            }
            return matchCount;
        }

        private int InterpretTagIdOrClass(TokenInfo info)
        {
            switch(RestOfLine[0])
            {
                case '#':
                    SetColor(info, HamlTokenColor.HamlTagId);
                    break;
                case '.':
                    SetColor(info, HamlTokenColor.HamlTagClass);
                    break;
            }

            var match = Regex.Match(RestOfLine.Substring(1), @"[\s|\.|=|\{]");

            if (!match.Success)
                return RestOfLine.Length;

            int matchCount = match.Index + 1;

            if (match.Value == ".")
                _state = HamlState.InTag;
            else
            {
                _state = "-={".Contains(match.Value) ? 
                    HamlState.AfterCodeTag : HamlState.AfterNonCodeTag;
            }

            return matchCount;
        }

        private int InterpretAfterNonCodeTag(TokenInfo info)
        {
            SetColor(info, HamlTokenColor.HamlText);

            return RestOfLine.Length;
        }

        private int InterpretAfterCodeTag(TokenInfo info)
        {
            _state = HamlState.AfterCodeTag;
            SetColor(info, HamlTokenColor.HamlCode);

            var match = Regex.Match(RestOfLine, @"//|""");

            if (!match.Success)
                return RestOfLine.Length;

            if (match.Index == 0)
            {
                switch (match.Value)
                {
                    case "//":
                        SetColor(info, HamlTokenColor.CsComment);
                        return RestOfLine.Length;
                    case "\"":
                        SetColor(info, HamlTokenColor.CsString);
                        var endOfString = Regex.Match(RestOfLine.Substring(1), "\"");
                        if (endOfString.Success)
                            return endOfString.Index + 2;

                        return RestOfLine.Length;
                }
            }

            return match.Index;
        }


        private int InterpretPartial(TokenInfo info)
        {
            SetColor(info, HamlTokenColor.HamlPartial);

            return RestOfLine.Length;
        }

        private int InterpretWhiteSpace(TokenInfo info)
        {
            SetColor(info, HamlTokenColor.HamlWhitespace);
            
            var match = Regex.Match(RestOfLine, @"\s+");
            return match.Value.Length;
        }

        private static void SetColor(TokenInfo info, TokenColor color)
        {
            info.Color = color;
        }

        private static void SetColor(TokenInfo info, HamlTokenColor color)
        {
            SetColor(info, (TokenColor)color);
        }

        private string RestOfLine
        {
            get { return _currentLine.Substring(_currentPos); }
        }

        /// <summary>
        /// This method is used to set the line to be parsed.
        /// </summary>
        /// <param name="source">The line to parse.</param>
        /// <param name="offset">The character offset in the line to start parsing from. 
        /// You have to pay attention to this value.</param>
        public void SetSource(string source, int offset)
        {
            _currentLine = source;
            _currentPos = offset;
            _state = GetCurrentLineState();
        }

        private HamlState GetCurrentLineState()
        {
            int tagTypeIndex = Regex.Match(_currentLine, @"[^\s]").Index;

            if (_currentPos <= tagTypeIndex)
                return HamlState.BeforeTag;

//            int endOfTagIndex = Regex.Match(_currentLine.Substring(tagTypeIndex), @"[\s|=|\{]").Index;
//
//            if (_currentPos <= endOfTagIndex)
//                return HamlState.AtTag;

            return HamlState.AfterNonCodeTag;
        }
    }
}
