/***********************************************************************************************
 * Easy Alphabet Arabic v3.0
 * Fayyad Sufyan (c) 2017
 * 
 * Description:
 * public code of the tool that handles the correction of sentences and line wrapping.
 * Core arabic characters correction is implemented in the dll Correct() function.
 * 
 ***********************************************************************************************/


using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using NetExtension;
namespace EasyAlphabetArabic
{
    public static class EasyArabicCore
    {

        #region enums

        /// <summary>
        ///  numbers format enum
        /// </summary>
        private enum numCase
        {
            LATIN   = 0,
            ARABIC  = 1,
            PERSIAN = 2
        }

        #endregion



        #region static variables

        static numCase NumCase = numCase.LATIN;

        // a dictionary for (startTag, endTag).
        // it is important to be case-insensitive to accept tags with mixed lower and capital chars
        // that maybe cased by a user typo e.g <coloR>abc</color>
        static Dictionary<string, string> startEndTags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "<i>", "</i>" },
            { "<b>", "</b>" },
            { "<size=", "</size>" },
            { "<color=", "</color>" },
        };

        #endregion



        #region static public functions

        /// <summary>
        /// Returns corrected arabic string
        /// </summary>
        /// <param name="text">text to be corrected</param>
        /// <param name="numsFormat">
        /// 0 = Latin ,
        /// 1 = Arabic (default),
        /// 2 = Persian
        /// </param>
        /// <returns></returns>
        public static string CorrectString(string text, int numsFormat = 1)
        {
            if (! IsRTL(text))
            {
                return text;
            }
            // set desired numbers format
            NumCase = (numCase)numsFormat;
            if ((int)NumCase < 0 || (int)NumCase > 2) NumCase = numCase.LATIN; // wrong input, fallback to latin
            
            return CorrectText(ExtractStrings(text));
        }


        /// <summary>
        /// Returns the text with correct line wrapping (used for Unity.UI.Text component only)
        /// </summary>
        /// <param name="text">UI text string</param>
        /// <param name="textComponent">Unity UI Text component</param>
        /// <param name="numsFormat">
        /// 0 = Latin ,
        /// 1 = Arabic (default),
        /// 2 = Persian
        /// </param>
        /// <returns></returns>
        public static string CorrectWithLineWrapping( Text textComponent)
        {
            Debug.Log("CorrectWithLineWrapping----->");
            int numsFormat = 1;
            string text = textComponent.text;
            // arbitrary max line length. needs to be long enough for a start
            // the correct max will be calculated later
            int lineMax = 1000;
            List<string> linesStr = new List<string>();

            // force updating the text graphic (performance cost and Garbage) because the text has not been rendered yet
            // this is needed when your are assigning new text and correcting it in the same frame
            textComponent.Rebuild(UnityEngine.UI.CanvasUpdate.PreRender);


            // set desired numbers format
            NumCase = (numCase)numsFormat;
            if ((int)NumCase < 0 || (int)NumCase > 2) NumCase = numCase.LATIN; // wrong input, fallback to latin

            // parse each line, correct it and add it to the final strings list
            for (int i = 0; i < textComponent.cachedTextGenerator.lines.Count; i++)
            {
                int startIndex = textComponent.cachedTextGenerator.lines[i].startCharIdx;
                int endIndex = -1;

                if (i == textComponent.cachedTextGenerator.lines.Count - 1) endIndex = textComponent.text.Length; // one line 
                else endIndex = textComponent.cachedTextGenerator.lines[i + 1].startCharIdx; // line ends where next one starts

                // line length
                int length = endIndex - startIndex;
                // extract the line string
                string line = textComponent.text.Substring(startIndex, length);
                //Debug.Log(line);

                // correct the extracted line and add it to the list
                linesStr.Add(CorrectText(ExtractStrings(line)));
            }

            for (int j = 0; j < linesStr.Count; j++)
            {
                int currLen = linesStr[j].Length; //current line length
                //Debug.Log(currLen);

                if (linesStr.Count > 1)
                {
                    if (j + 1 < linesStr.Count)
                    {
                        List<string> nextLineStrings = new List<string>(linesStr[j + 1].Split()); // splitted strings in next line
                        nextLineStrings.Reverse();

                        foreach (var s in nextLineStrings)
                        {
                            int finalLen = s.Length + currLen;

                           if (finalLen <= lineMax)
                            {
                                linesStr[j] = linesStr[j].Insert(0, s); // insert into previous line
                                linesStr[j + 1] = linesStr[j + 1].Remove(linesStr[j + 1].IndexOf(s), s.Length).Trim(); // remove from next line

                                lineMax = linesStr[j].Length; // now we are able to calculate the correct capacity of the line

                                if (linesStr[j + 1].Length == 0) // all next line strings has been added to previous one, remove the empty line
                                {
                                    // insert a space after last word. Note: remember the string is reversed
                                    linesStr[j] = linesStr[j].Insert(linesStr[j].LastIndexOf(s) + s.Length, " ");
                                    linesStr.Remove(linesStr[j + 1]);
                                }

                                currLen = finalLen; break;
                            }
                           // else break;
                        } // foreach end

                        nextLineStrings.Clear();
                    }
                } // if end
            } // for end

            // join the strings into one final string
            string myStr = "";
            for (int i = 0; i < linesStr.Count; i++)
            {
                myStr += linesStr[i].Trim();                // concatenate each line into the final string
                if (i != linesStr.Count - 1) myStr += Environment.NewLine; // make sure it is not one line, then wrap it into new line

            }
            linesStr.Clear();

            return myStr;
        }

        public static void CorrectWithLineWrapping(Text text,string content)
        {
            HttpMgr.Instance.StartRequestTask(FixText(text, content));
        }

     static   IEnumerator FixText(Text myText, string EnterText)
        {
    
            //used in cutting the lines 
            int startIndex;
            int endIndex;
            int length;
            int j;
            string TextHolder=string.Empty;

            var Holder = EnterText.Split('\n');
            string[] FixedText = new string[30];
            for (int i = 0; i < Holder.Length; i++)
            {
                myText.text = CorrectString(Holder[i]);//  ArabicFixer.Fix(Holder[i], false, false);
                Canvas.ForceUpdateCanvases();
                for (int k = 0; k < FixedText.Length; k++)
                {
                    FixedText[k] = "";
                }

                for (int k = 0; k < myText.cachedTextGenerator.lines.Count; k++)
                {
                    startIndex = myText.cachedTextGenerator.lines[k].startCharIdx;
                    endIndex = (k == myText.cachedTextGenerator.lines.Count - 1) ? myText.text.Length
                       : myText.cachedTextGenerator.lines[k + 1].startCharIdx;
                    length = endIndex - startIndex;
                   // Debug.Log(myText.text.Substring(startIndex, length));
                    FixedText[k] = myText.text.Substring(startIndex, length);
                }
                myText.text = "";
                for (int k = FixedText.Length - 1; k >= 0; k--)
                {
                    if (FixedText[k] != "" && FixedText[k] != "\n" && FixedText[k] != null)
                    {

                        TextHolder += FixedText[k] + "\n";
                    }
                }
            }
            myText.text = TextHolder;

            yield return new WaitForEndOfFrame();
            HttpMgr.Instance.EndRequest();

        }



        /// <summary>
        /// Corrects TextMesh Pro text(3D and UI)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="numsFormat">
        /// 0 = Latin ,
        /// 1 = Arabic (default),
        /// 2 = Persian
        /// </param>
        /// <returns></returns>
        public static string CorrectTextMeshPro(string text, int numsFormat = 1)
        {
            // set desired numbers format
            NumCase = (numCase)numsFormat;
            if ((int)NumCase < 0 || (int)NumCase > 2) NumCase = numCase.LATIN; // wrong input, fallback to latin

            return CorrectText(ExtractStrings(text), true);
        }


        /// <summary>
        /// Is text a RTL(right-to-left) or not.
        /// </summary>
        /// <param name="txt">text to investigate</param>
        /// <returns></returns>
        public static bool IsRTL(string txt)
        {
            foreach (var c in txt) if ((c >= 65136 && c <= 65279) || (c >= 1536 && c <= 1791)) return true;
            return false;
        }


        #endregion



        #region static private functions

        /// <summary>
        /// Extract text as blocks (normal or richtext) and add to a list,
        /// then correct each text in the list individually
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        static List<string> ExtractStrings(string inputStr)
        {
            List<string> extractedStrings = new List<string>();

            // core loop that runs on every character
            for (int i = 0; i < inputStr.Length; i++)
            {
                // if the char is a richtext, run a loop to extract the whole richtext sentence "<>abc def </>"
                if (inputStr[i].Equals('<')) // rich text early indicator
                {
                    string remainingString = inputStr.Substring(i, inputStr.Length - i); // return remaining text (richtext or not)

                    // make sure the text starts with richtext tag
                    if (StartsWithTag(remainingString))
                    {
                        // retrieve the start tag
                        string dictStartTag = remainingString.Substring(0, remainingString.IndexOf('>') + 1);
                        
                        // if tag is <color=#ffff> or <size=20> (has =) strip it (<color=) to match the dictionary key,
                        // otherwise return end tag as is ("</i>", "</b>")
                        string endTag = (dictStartTag.Contains("=") ? startEndTags[dictStartTag.Substring(0, dictStartTag.IndexOf("=") + 1)] : startEndTags[dictStartTag]);
                        int endIdx = remainingString.IndexOf(endTag) + endTag.Length - 1;
                        remainingString = remainingString.Substring(0, endIdx + 1); // we finally able to accurately isolate the richtext sentence from the rest
                        
                        // we have to make sure each start tag has a corresponding end tag! before we move on to extract the richtext
                        if ( remainingString.Contains(endTag))
                        {
                            List<char> richtextChars = new List<char>(remainingString.Length);

                            // extract the entire richtext (including the tags)
                            // e.g single tag <>abc</>  or nested tags <><>abc</></>
                            for (int k = 0; k < remainingString.Length; k++) richtextChars.Add(remainingString[k]);

                            // advance core loop counter 'i' to next text
                            i += remainingString.Length - 1;

                            // add richtext string to the list
                            extractedStrings.Add(new string(richtextChars.ToArray()));
                        }
                        else throw new Exception("End tag not found! this maybe a user typo or line wrapping that is breaking the tags.");
                    }
                    // just a '<' character and not a real richtext tag <b>
                    else extractedStrings.Add("<");

                } // end if

                // otherwise it is normal text, run a loop and extract it
                else
                {
                    // the loop by default ends on last char of the text (assuming no richtext),
                    // but if there is one, it will break once we reach the char before the start of richtext
                    List<char> textChars = new List<char>(inputStr.Length); // setup with large size to avoid GC caused by list.GrowIfNeeded()
                    char curr, next = ' ';

                    int j = i;
                    for (; j < inputStr.Length; j++)
                    {
                        curr = inputStr[j];
                        if (j + 1 < inputStr.Length) next = inputStr[j + 1]; // assign the next character after array bound check

                        // next char is richtext, add current char and exit
                        if (next == '<') { textChars.Add(curr); break; }
                        // next char is not richtext, add the char
                        else textChars.Add(curr);
                    }
                    // advance core loop counter
                    i = j;

                    // add the string to the list
                    extractedStrings.Add(new string(textChars.ToArray()));
                }
            }

            return extractedStrings;
        }


        /// <summary>
        /// Correct each string in the list whether the text is normal or between tags (richtext)
        /// </summary>
        /// <param name="inputStrings"></param>
        /// <param name="TMPro">is TextMesh Pro text?</param>
        /// <returns></returns>
        static string CorrectText(List<string> inputStrings, bool TMPro = false)
        {
            string finalString = "";

            if(!TMPro) inputStrings.Reverse();

            for (int i = 0; i < inputStrings.Count; i++)
            {
                // richtext string. extract the text between tags, correct it and replace the old one with it
                if (StartsWithTag(inputStrings[i]) && EndsWithTag(inputStrings[i]))
                {
                    if (!TMPro) inputStrings[i] = CorrectBetweenTags(inputStrings[i], false);
                    else inputStrings[i] = CorrectBetweenTags(inputStrings[i], true);
                }
                else
                {
                    inputStrings[i] = EasyArabicInternals.Correct(inputStrings[i].ToCharArray(), (int)NumCase); // correct the string

                    if (!TMPro) inputStrings[i] = ReverseArabic(inputStrings[i]); // don't reverse textmesh pro text
                }

                // concatenate the corrected strings
                finalString += inputStrings[i];
            }

            return finalString;
        }


        /// <summary>
        /// Correct the text between tags, including nested richtext tags (if any)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TMPro">is TextMesh Pro text?</param>
        /// <returns></returns>
        static string CorrectBetweenTags(string text, bool TMPro = false)
        {
            string finalString = "";

            int startIdx = text.IndexOf('>');
            int endIdx = text.LastIndexOf("</");
            string startTag = text.Substring(0, startIdx + 1);
            string endTag = text.Substring(endIdx, text.Length - endIdx);

            // text between tags (may or may not include nested richtext) <>abc</> or <><>abc</></>
            string betweenStr = text.Substring(startTag.Length, text.Length - startTag.Length - endTag.Length);

            // recursively extract nested richtext
            if (StartsWithTag(betweenStr) && EndsWithTag(betweenStr))
            {
                betweenStr = CorrectBetweenTags(betweenStr);
            }
            else // no richtext, correct
            {
                betweenStr = EasyArabicInternals.Correct(betweenStr.ToCharArray(), (int)NumCase);

                if (!TMPro) betweenStr = ReverseArabic(betweenStr);
            }

            // append the strings
            finalString += betweenStr;
            // insert the start tag at the start, and end tag at the end.  <>abc  abc</>  ==>  <>abc</>
            finalString = finalString.Insert(0, startTag);
            finalString = finalString.Insert(finalString.Length, endTag);

            return finalString;
        }


        /// <summary>
        /// Inform if a string starts with a richtext tag
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static bool StartsWithTag(string s)
        {
            return (s.StartsWith("<i>", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("<b>", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("<color=", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("<size=", StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Inform if a string ends with a richtext tag
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static bool EndsWithTag(string s)
        {
            return (s.EndsWith("</i>", StringComparison.OrdinalIgnoreCase) ||
                    s.EndsWith("</b>", StringComparison.OrdinalIgnoreCase) ||
                    s.EndsWith("</color>", StringComparison.OrdinalIgnoreCase) ||
                    s.EndsWith("</size>", StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Reverse arabic words (only) order
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string ReverseArabic(string str)
        {
            if (! IsRTL(str))
            {
                return str;
            }
            StringBuilder result = new StringBuilder();

            int Length = 0;
            for (int i = str.Length; i > 0;)
            {
                --i;
                // found arabic char
                if ((str[i] >= 65136 && str[i] <= 65279) || str[i] == 32 ||str[i]==46||str[i]==40||str[i]==41) //添加翻转字符 46:".";40:"(";41:"）" 
                {
                    if (Length > 0)
                    {
                        result.Append(str.Substring(i + 1, Length));
                    }
                    Length = 0;
                    result.Append(str[i]);
                }
                else
                    Length++;
            }
            if (Length > 0)
            {
                result.Append(str.Substring(0, Length));
            }
            return result.ToString();
        }

        #endregion

    }
}