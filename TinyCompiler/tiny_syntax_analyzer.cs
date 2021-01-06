using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCompiler
{
    public class Node
    {
        public string NodeName;
        public List<Node> childrenNodes;
        public List<string> nodeErrors;
        public Node(string name)
        {
            NodeName = name;
            childrenNodes = new List<Node>();
            nodeErrors = new List<string>();
        }
    }
    class tiny_syntax_analyzer
    {
        static int tokensPointer;
        static List<KeyValuePair<string, TinyToken>> myTokens; // key is the token string   and value is the token type
        static Node root;
        static List<string> parserErrors;
        public static void Initialize(List<KeyValuePair<string, TinyToken>> inputTokens)
        {
            tokensPointer = 0;
            myTokens = inputTokens;
            root = Program();
            parserErrors = new List<string>();
            int sz = 0;
            if (root.childrenNodes.Count > 0) sz = root.childrenNodes.Count;
            if (sz < 1 || root.childrenNodes[sz - 1].NodeName != "MainFunction")
            {
                parserErrors.Add("Main Function isn't implemented properly or not implemented at all !!!");
            }
        }
        public static Node Program()
        {
            Node curNode = new Node("Program");
            int tmpPntr = tokensPointer;
            Node node1;
            while (true)
            {
                node1 = MainFunction();
                if (node1 != null)
                {
                    curNode.childrenNodes.Add(node1);
                    return curNode;
                }
                tokensPointer = tmpPntr;
                node1 = Function();
                if (node1 != null)
                {
                    curNode.childrenNodes.Add(node1);
                    continue;
                }
                return null;
            }
        }
        public static Node MainFunction()
        {
            Node curNode = new Node("MainFunction");
            Node node1 = DataType();
            string error = "";
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier && myTokens[tokensPointer].Key == "main")
            {
                curNode.childrenNodes.Add(new Node("main"));
                tokensPointer++;// increase for the first token 
            }
            else
            {
                error = "Error in MainFunction ... Reserved Keyword   'main'   Not Found  :(  !!!";
                parserErrors.Add(error);
                tokensPointer++;// increase for the first token 
            }
            tokensPointer++;// increase for the first token 


            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                curNode.childrenNodes.Add(new Node("("));
                tokensPointer++;
            }
            else
            {
                error = "Error in MainFunction ... Left Bracket Not Found  :(  !!!";
                parserErrors.Add(error);
                tokensPointer++;
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_rBracket)
            {
                curNode.childrenNodes.Add(new Node(")"));
                tokensPointer++;
            }
            else
            {
                error = "Error in MainFunction ... Right Bracket Not Found  :(  !!!";
                parserErrors.Add(error);
                tokensPointer++;
            }
            node1 = FunctionBody();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (curNode.childrenNodes.Count == 5) return curNode;
            return null;
        }
        public static Node Function()
        {
            Node curNode = new Node("Function");
            Node node1 = FunctionDecl();// 1st function declaration 
            if (node1 != null)
                curNode.childrenNodes.Add(node1);
            node1 = FunctionBody();// 2nd function body
            if (node1 != null)
                curNode.childrenNodes.Add(node1);
            if (curNode.childrenNodes.Count == 2) return curNode;
            return null;
        }
        public static Node FunctionDecl()
        {
            Node curNode = new Node("FunctionDecl");
            string error = "";
            Node node1 = Decl1Var();// 1st declare 1 variable for function datatype and function name 
            if (node1 != null)
                curNode.childrenNodes.Add(node1);

            if (tokensPointer < myTokens.Count &&
                myTokens[tokensPointer].Value != TinyToken.t_lBracket)// 2nd : the left bracket '('
            {
                error = "Error in FunctionDecl ... Expected (   found ";
                error += myTokens[tokensPointer].ToString();
                parserErrors.Add(error);
                tokensPointer++;// increase for the first token
            }
            else
            {
                curNode.childrenNodes.Add(new Node("("));
                tokensPointer++;// increase for the first token
            }


            node1 = ParametersDecl();// 3rd function parameteres declaration 
            if (node1 != null)
                curNode.childrenNodes.Add(node1);

            if (tokensPointer < myTokens.Count &&
                myTokens[tokensPointer].Value != TinyToken.t_rBracket)// 4th : the right bracket ')'
            {
                error = "Error in FunctionDecl ... Expected )  found ";
                error += myTokens[tokensPointer].ToString();
                parserErrors.Add(error);
                tokensPointer++;// increase for the second token in the function
            }
            else
            {
                curNode.childrenNodes.Add(new Node(")"));
                tokensPointer++;// increase for the second token in the function
            }

            if (curNode.childrenNodes.Count == 4) return curNode;
            return null;
        }

        public static Node Decl1Var()
        {
            Node curNode = new Node("Decl1Var");
            Node node1 = DataType();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in Decl1Var ... Expected identifier   found ";
                error += myTokens[tokensPointer].ToString();
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }


            if (curNode.childrenNodes.Count == 2)
            {
                return curNode;
            }
            return null;
        }
        public static Node ParametersDecl()
        {
            Node curNode = new Node("ParametersDecl");
            int tmpPntr = tokensPointer;
            Node node1 = Parameters();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node FunctionBody()
        {
            Node curNode = new Node("FunctionBody");

            if (myTokens[tokensPointer].Value == TinyToken.t_lCurlyBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in FunctionBody ... Expected '{'  found ";
                error += myTokens[tokensPointer].ToString();
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }


            Node node1 = ManyStatements();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            node1 = ReturnStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_rCurlyBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the second token in the function
            }
            else
            {
                string error = "Error in FunctionBody ... Expected '}'  found ";
                error += myTokens[tokensPointer].ToString();
                parserErrors.Add(error);
                tokensPointer++;// increase it for the second token in the function
            }

            if (curNode.childrenNodes.Count == 4) return curNode;// the whole function is correct

            return null;
        }
        public static Node DataType()
        {
            Node curNode = new Node("DataType");
            if (myTokens[tokensPointer].Value == TinyToken.t_int)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_float)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_string)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Not a Valid DataType :( !!!!";
                parserErrors.Add(error);
            }
            tokensPointer++;// increase it for the first token in the function
            if (curNode.childrenNodes.Count > 0) return curNode;
            return null;

        }
        public static Node Parameters()
        {
            Node curNode = new Node("Parameters");
            /// To be implmeneted 
            return null;
        }
        public static Node ParametersDash()
        {
            Node curNode = new Node("ParametersDash");
            /// To be implmeneted 
            return null;
        }
        public static Node ManyStatements()
        {
            Node curNode = new Node("ManyStatements");
            /// To be implmeneted 
            return null;
        }
        public static Node Statement()
        {
            Node curNode = new Node("Statement");

            int tmpPntr = tokensPointer;
            Node node1 = IfStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;


            node1 = WriteStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            node1 = ReadStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            node1 = ReturnStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            node1 = AssignmentStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            node1 = FunctionCall();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            node1 = RepeatStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            node1 = DeclarationStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            if (curNode.childrenNodes.Count == 1) return curNode;
            return null;
        }
        public static Node IfStatement()
        {
            Node curNode = new Node("IfStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_if)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in IfStatement ... couldn't find reserved keyword 'if' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            Node node1 = ConditionStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            if (myTokens[tokensPointer].Value == TinyToken.t_then)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in IfStatement ... couldn't find reserved keyword 'then' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            node1 = StatmentsForIf();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            node1 = Ifff();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            if (curNode.childrenNodes.Count == 5) return curNode;
            return null;
        }
        public static Node ConditionStatement()
        {
            Node curNode = new Node("ConditionStatement");
            /// To be implmeneted 
            return null;
        }
        public static Node ConditionStatementDash()
        {
            Node curNode = new Node("ConditionStatementDash");
            /// To be implmeneted 
            return null;
        }
        public static Node Condition()
        {
            Node curNode = new Node("Condition");

            if (myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in Condition ... Couldn't find the Identifier ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            Node node1 = ConditionOperator();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            node1 = Term();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            if (curNode.childrenNodes.Count == 3)
                return curNode;

            return null;
        }
        public static Node ConditionOperator()
        {
            Node curNode = new Node("ConditionOperator");
            if (myTokens[tokensPointer].Value == TinyToken.t_lessThan)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_biggerThan)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_notEqual)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_isEqual)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Couldn't find an Operator  :( !!!!";
                parserErrors.Add(error);

            }
            tokensPointer++;// increase it for the first token in the function
            if (curNode.childrenNodes.Count == 1) return curNode;
            return null;
        }
        public static Node Term()
        {
            Node curNode = new Node("Term");
            int tmpPntr = tokensPointer;
            Node node1 = FunctionCall();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else tokensPointer = tmpPntr;

            if (myTokens[tokensPointer].Value == TinyToken.t_number)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }

            if (curNode.childrenNodes.Count == 1) return curNode;
            return null;
        }
        public static Node FunctionCall()
        {
            Node curNode = new Node("FunctionCall");

            if (myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find the function name";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            if (myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find left brackt '(' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            Node node1 = ManyIdentifiers();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            if (myTokens[tokensPointer].Value == TinyToken.t_rBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find the Right Bracket ')'";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            if (myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find the SemiColon ';' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            if (curNode.childrenNodes.Count == 5) return curNode;
            return null;
        }
        public static Node ManyIdentifiers()
        {
            Node curNode = new Node("ManyIdentifiers");
            int tmpPntr = tokensPointer;
            Node node1 = Identifiers();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node Identifiers()
        {
            Node curNode = new Node("Identifiers");
            /// To be implmeneted 
            return null;
        }
        public static Node IdentifiersDash()
        {
            Node curNode = new Node("IdentifiersDash");
            /// To be implmeneted 
            return null;
        }
        public static Node BooleanOperator()
        {
            Node curNode = new Node("BooleanOperator");
            if (myTokens[tokensPointer].Value == TinyToken.t_and)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_or)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            if (curNode.childrenNodes.Count == 1) return curNode;
            return null;
        }
        public static Node StatmentsForIf()
        {
            Node curNode = new Node("StatmentsForIf");
            /// To be implmeneted 
            return null;
        }
        public static Node StatmentsForIfDash()
        {
            Node curNode = new Node("StatmentsForIfDash");
            /// To be implmeneted 
            return null;
        }
        public static Node Ifff()
        {
            Node curNode = new Node("Ifff");
            int tmpPntr = tokensPointer;
            Node node1 = ElseIfStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                tokensPointer = tmpPntr;
                node1 = ElseStatement();
                if (node1 != null)
                {
                    curNode.childrenNodes.Add(node1);
                }
                else
                {
                    tokensPointer = tmpPntr;
                    if (myTokens[tokensPointer].Value == TinyToken.t_end)
                    {
                        curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                    }
                    else
                    {
                        string error = "Error in ifff ... couldn't find reserved keyword 'end' ";
                        parserErrors.Add(error);
                        tokensPointer++;// increase it for the first token in the function
                    }
                }
            }
            if (curNode.childrenNodes.Count == 1)
                return curNode;
            return null;
        }
        public static Node ElseIfStatement()
        {
            Node curNode = new Node("ElseIfStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_elseif)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ElseIfStatement ... couldn't find reserved keyword 'elseif' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            Node node1 = ConditionStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            if (myTokens[tokensPointer].Value == TinyToken.t_then)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ElseIfStatement ... couldn't find reserved keyword 'then' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            node1 = StatmentsForIf();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            node1 = Ifff();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }

            if (curNode.childrenNodes.Count == 5) return curNode;
            return null;
        }
        public static Node ElseStatement()
        {
            Node curNode = new Node("ElseStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_else)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ElseStatement ... couldn't find reserved keyword 'else' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            Node node1 = StatmentsForIf();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_end)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ElseStatement ... couldn't find reserved keyword 'end' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (curNode.childrenNodes.Count == 3) return curNode;
            return null;
        }
        public static Node WriteStatement()
        {
            Node curNode = new Node("WriteStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_write)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in WriteStatement ... couldn't find reserved keyword 'write' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            Node node1 = Something();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_endl)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in WriteStatement ... couldn't find reserved keyword 'endl' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_comma)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in WriteStatement ... couldn't find reserved keyword ';' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (curNode.childrenNodes.Count == 3) return curNode;
            return null;
        }
        public static Node Something()
        {
            Node curNode = new Node("Something");
            int tmpPntr = tokensPointer;
            Node node1 = Expression();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                tokensPointer = tmpPntr;
                if (myTokens[tokensPointer].Value == TinyToken.t_endl)
                {
                    curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                }
                else
                {
                    string error = "Error in ifff ... couldn't find reserved keyword 'endl' ";
                    parserErrors.Add(error);
                    tokensPointer++;// increase it for the first token in the function
                }
            }
            if (curNode.childrenNodes.Count == 1)
                return curNode;
            return null;
        }
        public static Node Expression()
        {
            Node curNode = new Node("Expression");
            int tmpPntr = tokensPointer;
            Node node1 = Equation();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                tokensPointer = tmpPntr;
                node1 = Term();
                if (node1 != null)
                {
                    curNode.childrenNodes.Add(node1);
                }
                else
                {
                    tokensPointer = tmpPntr;
                    if (myTokens[tokensPointer].Value == TinyToken.t_constantString)
                    {
                        curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                    }
                    else
                    {
                        string error = "Error in Expression ... couldn't find Constant String ";
                        parserErrors.Add(error);
                        tokensPointer++;// increase it for the first token in the function
                    }
                }
            }
            if (curNode.childrenNodes.Count == 1)
                return curNode;
            return null;
        }
        public static Node Equation()// Equation CFG needs to be modified 
        {
            Node curNode = new Node("Equation");
            /// To be implmeneted 
            return null;
        }
        public static Node HelperTerm()
        {
            Node curNode = new Node("HelperTerm");
            int tmpPntr = tokensPointer;
            Node node1 = Term();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                tokensPointer = tmpPntr;
                node1 = HelperEquation();
                if (node1 != null)
                {
                    curNode.childrenNodes.Add(node1);
                }
            }
            if (curNode.childrenNodes.Count == 1)
                return curNode;
            return null;
        }
        public static Node HelperEquation()
        {
            Node curNode = new Node("HelperEquation");
            if (myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in HelperEquation ... couldn't find reserved keyword 'left bracket' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            Node node1 = ManyTerms();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_rBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in HelperEquation ... couldn't find reserved keyword 'Right bracket' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (curNode.childrenNodes.Count == 3) return curNode;
            return null;
        }
        public static Node ManyTerms()
        {
            Node curNode = new Node("ManyTerms");
            /// To be implmeneted 
            return null;
        }
        public static Node ManyTermsDash()
        {
            Node curNode = new Node("ManyTermsDash");
            /// To be implmeneted 
            return null;
        }
        public static Node ArthimiticOperator()
        {
            Node curNode = new Node("ArthimiticOperator");
            if (myTokens[tokensPointer].Value == TinyToken.t_plus)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_minus)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_multiply)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (myTokens[tokensPointer].Value == TinyToken.t_divide)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Couldn't find an ArthimiticOperator Operator  :( !!!!";
                parserErrors.Add(error);

            }
            tokensPointer++;// increase it for the first token in the function
            if (curNode.childrenNodes.Count == 1) return curNode;
            return null;
        }
        public static Node ReadStatement()
        {
            Node curNode = new Node("ReadStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_read)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ReadStatement ... couldn't find reserved keyword 'read' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ReadStatement ... couldn't find an identifier ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ReadStatement ... couldn't find ';' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (curNode.childrenNodes.Count == 3) return curNode;
            return null;
        }
        public static Node ReturnStatement()
        {
            Node curNode = new Node("ReturnStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_return)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ReturnStatement ... couldn't find reserved keyword 'return' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            Node node1 = Expression();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in ReturnStatement ... couldn't find reserved keyword ';' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (curNode.childrenNodes.Count == 3) return curNode;
            return null;
        }
        public static Node AssignmentStatement()
        {
            Node curNode = new Node("AssignmentStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in AssignmentStatement ... couldn't find an identifier ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_assign)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in AssignmentStatement ... couldn't find ':=' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            Node node1 = Expression();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in AssignmentStatement ... couldn't find  ';' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            if (curNode.childrenNodes.Count == 4) return curNode;
            return null;
        }
        public static Node RepeatStatement()
        {
            Node curNode = new Node("RepeatStatement");
            if (myTokens[tokensPointer].Value == TinyToken.t_repeat)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in RepeatStatement ... couldn't find reserved keyword 'repeat' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            Node node1 = StatmentsForIf();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_until)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in RepeatStatement ... couldn't find reserved keyword 'until' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            node1 = ConditionStatement();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (curNode.childrenNodes.Count == 4) return curNode;
            return null;
        }
        public static Node DeclarationStatement()
        {
            Node curNode = new Node("DeclarationStatement");
            Node node1 = DataType();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            node1 = ManyIdentifiersDecl();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            if (myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in DeclarationStatement ... couldn't find ';' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }

            if (curNode.childrenNodes.Count == 4) return curNode;
            return null;
        }
        public static Node ManyIdentifiersDecl()
        {
            Node curNode = new Node("ManyIdentifiersDecl");
            /// To be implmeneted 
            return null;
        }
        public static Node ManyIdentifiersDeclDash()
        {
            Node curNode = new Node("ManyIdentifiersDeclDash");
            /// To be implmeneted 
            return null;
        }
        public static Node AssignmentInDecl()
        {
            Node curNode = new Node("AssignmentInDecl");
            int tmpPntr = tokensPointer;
            if (myTokens[tokensPointer].Value == TinyToken.t_assign)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;// increase it for the first token in the function
            }
            else
            {
                string error = "Error in AssignmentInDecl ... couldn't find ':=' ";
                parserErrors.Add(error);
                tokensPointer++;// increase it for the first token in the function
            }
            Node node1 = Expression();
            if (node1 != null)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }

    }
}