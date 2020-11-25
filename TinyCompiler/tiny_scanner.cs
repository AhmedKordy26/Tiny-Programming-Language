using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TinyToken
{
    t_dot, t_semicolon, t_comma, t_lBracket, t_rBracket,
    t_biggerThan, t_lessThan, t_plus, t_minus,t_multiply,
    t_divide, t_assign, t_isEqual, t_and, t_or,t_notEqual,
    t_backSlash, t_lCurlyBracket, t_rCurlyBracket,t_Quotation,

    t_int,t_float,t_string,t_read,t_write,t_repeat,t_until,t_if,
    t_elseif,t_else,t_then,t_return,t_endl,

    t_number, t_identifier
}
namespace TinyCompiler
{

    class tiny_scanner
    {
        public static List<KeyValuePair<string,TinyToken>> tinyTokensList=new List<KeyValuePair<string, TinyToken>>();
        public static List<string> splittedStringsList = new List<string>();
        public static List<string> errorsList = new List<string>();
        public static List<string> newSplitter(string toSplit) ////// question :::  should i add numbers in tokens and lexxems
        {
            //Console.WriteLine(toSplit);
            List<string> tmpList = new List<string>();
            string tmp = "",operatorTmp="";
            bool typeString = false;
            for (int i = 0; i < toSplit.Length; ++i)
            {
                if (toSplit[i] == ' ' || toSplit[i] == '\n')
                {
                    if (tmp.Length > 0) splittedStringsList.Add(tmp);
                    tmp = "";
                    continue;
                }
                if(typeString==false && toSplit[i]== '"')
                {
                    tmp = "";
                    tmp += '"';
                    typeString = true;
                    continue;
                }
                if(typeString==true )
                {
                    if(toSplit[i]=='"')
                    {
                        tmp += '"';
                        if (i < toSplit.Length - 1 && toSplit[i + 1] != ' ')
                        {
                            splittedStringsList.Add(tmp);
                            tmp = "";
                        }
                        typeString = false;
                    }
                    else
                    {
                        tmp += toSplit[i];
                    }
                    continue;
                }
                operatorTmp = "";
                operatorTmp += toSplit[i];
                if (isOperator(operatorTmp).Key == true)
                {
                    if(tmp.Length>0)
                    {
                        splittedStringsList.Add(tmp);
                        tmp = "";
                    }
                    splittedStringsList.Add(operatorTmp);
                    operatorTmp = "";
                    continue;
                }
                if(i<toSplit.Length-1)
                {
                    operatorTmp += toSplit[i+1];
                    if (isOperator(operatorTmp).Key == true)
                    {
                        if (tmp.Length > 0)
                        {
                            splittedStringsList.Add(tmp);
                            tmp = "";
                        }
                        splittedStringsList.Add(operatorTmp);
                        operatorTmp = "";
                        ++i;
                        continue;
                    }
                }
                tmp += toSplit[i];
            }
            if (tmp.Length > 0)
                splittedStringsList.Add(tmp);
            foreach(string st in splittedStringsList)
            {
               // Console.WriteLine(st);
            }
            return splittedStringsList;
        }
        public static void findTokensAndErrors()
        {
            KeyValuePair<bool, TinyToken> tmp = new KeyValuePair<bool, TinyToken>();
            foreach(string st in splittedStringsList)
            {
                bool ok = false;
                tmp = isOperator(st);
                if(tmp.Key==true)
                {
                    KeyValuePair<string, TinyToken>newtmp = new KeyValuePair<string, TinyToken>(st, tmp.Value);
                    tinyTokensList.Add(newtmp);
                    ok = true;
                }
                tmp = isReservedWord(st);
                if(tmp.Key==true)
                {
                    tinyTokensList.Add(new KeyValuePair<string, TinyToken>(st, tmp.Value));
                    ok = true;
                }
                else if(isNumber(st).Key==true)
                {
                    tinyTokensList.Add(new KeyValuePair<string, TinyToken>(st, TinyToken.t_number));
                    ok = true;
                }
                else if(isIdentifier(st) == true)
                {
                    tinyTokensList.Add(new KeyValuePair<string, TinyToken>(st, TinyToken.t_identifier));
                    ok = true;
                }
                else if(isString(st))
                {
                    tinyTokensList.Add(new KeyValuePair<string, TinyToken>(st, TinyToken.t_string));
                    ok = true;
                }
                if(ok==false)
                {
                    errorsList.Add(st);
                }
            }
        }
        public static KeyValuePair<bool, TinyToken> isOperator(string lexm)// search in operators
        {
            KeyValuePair<bool, TinyToken> ans = new KeyValuePair<bool, TinyToken>(false, TinyToken.t_dot); // dumy ans ,, return if not found in operators
            if (lexm == ".")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_dot);
            }
            else if (lexm == ";")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_semicolon);
            }
            else if (lexm == ",")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_comma);
            }
            else if (lexm == "(")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_lBracket);
            }
            else if (lexm == ")")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_rBracket);
            }
            else if (lexm == "<")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_lessThan);
            }
            else if (lexm == ">")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_biggerThan);
            }
            else if (lexm == "+")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_plus);
            }
            else if (lexm == "-")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_minus);
            }
            else if (lexm == "*")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_multiply);
            }
            else if (lexm == ":=")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_assign);
            }
            else if (lexm == "=")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_isEqual);
            }
            else if (lexm == "&&")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_and);
            }
            else if (lexm == "||")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_or);
            }
            else if (lexm == "{")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_lCurlyBracket);
            }
            else if (lexm == "}")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_rCurlyBracket);
            }
            else if (lexm == "\"")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_Quotation);
            }
            else if (lexm == "\\")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_backSlash);
            }
            else if (lexm == "/")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_divide);
            }
            else if (lexm == "<>")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_notEqual);
            }
            return ans;
        }
        public static KeyValuePair<bool, TinyToken> isReservedWord(string lexm)// search in reserved words
        {
            KeyValuePair<bool, TinyToken> ans = new KeyValuePair<bool, TinyToken>(false, TinyToken.t_int); // dumy ans ,, return if not found in reserved words
            if (lexm == "int")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_int);
            }
            else if (lexm == "float")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_float);
            }
            else if (lexm == "string")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_string);
            }
            else if (lexm == "read")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_read);
            }
            else if (lexm == "write")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_write);
            }
            else if (lexm == "repeat")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_repeat);
            }
            else if (lexm == "until")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_until);
            }
            else if (lexm == "if")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_if);
            }
            else if (lexm == "elseif")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_elseif);
            }
            else if (lexm == "else")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_else);
            }
            else if (lexm == "then")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_then);
            }
            else if (lexm == "return")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_return);
            }
            else if (lexm == "endl")
            {
                ans = new KeyValuePair<bool, TinyToken>(true, TinyToken.t_endl);
            }
            return ans;
        }
        public static KeyValuePair<bool, double> isNumber(string num)// check if it's a number
        {
            KeyValuePair<bool, double> ans = new KeyValuePair<bool, double>(false, 0.0);// dummy
            if (double.TryParse(num, out double x) == true)
            {
                ans = new KeyValuePair<bool, double>(true, x);
            }
            return ans;
        }
        public static bool isIdentifier(string idntf)
        {
            return (idntf[0] >= 'a' && idntf[0] <= 'z');
        }
        public static bool isString(string tmString)
        {
            return (tmString[0] == '"' && tmString[tmString.Length - 1] == '"');
        }
    }
}
