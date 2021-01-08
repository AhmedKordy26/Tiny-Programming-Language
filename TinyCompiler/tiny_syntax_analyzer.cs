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
        public static Node root;
        public static void Initialize(List<KeyValuePair<string, TinyToken>> inputTokens)
        {
            tokensPointer = 0;
            myTokens = inputTokens;
            root = Program();
            int sz = 0;
            if (root.childrenNodes.Count > 0) sz = root.childrenNodes.Count;

            if (tokensPointer < myTokens.Count)
            {
                root.nodeErrors.Add("MainFunction must be the last function in the program");
            }
            else if (sz < 1 || root.childrenNodes[sz - 1].NodeName != "MainFunction")
            {
                root.nodeErrors.Add("Main Function isn't implemented properly or not implemented at all !!!");
            }
        }
        public static Node Program()
        {
            Node curNode = new Node("Program");
            int tmpPntr = tokensPointer, idx;
            Node node1 = DataType();
            idx = tokensPointer;
            if ((node1.nodeErrors.Count == 0 && idx < myTokens.Count && myTokens[idx].Value == TinyToken.t_identifier && myTokens[idx].Key != "main"))
            {
                tokensPointer = tmpPntr;
                node1 = Function();// 2nd recursion for function
                if (node1.nodeErrors.Count == 0)
                {
                    curNode.childrenNodes.Add(node1);
                }
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                node1 = Program();
                if (node1.nodeErrors.Count == 0)
                {
                    curNode.childrenNodes.Add(node1);
                }
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                return curNode;
            }
            else
            {
                tokensPointer = tmpPntr;
                node1 = MainFunction();// 1st main functoin
                if (node1.nodeErrors.Count == 0)
                {
                    curNode.childrenNodes.Add(node1);
                }
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                return curNode;
            }
        }
        public static Node MainFunction()
        {
            Node curNode = new Node("MainFunction");
            Node node1 = DataType();
            string error = "";
            if (node1.nodeErrors.Count == 0)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier && myTokens[tokensPointer].Key == "main")
            {
                curNode.childrenNodes.Add(new Node("main"));
            }
            else
            {
                error = "Error in MainFunction ... Reserved Keyword   'main'   Not Found  :(  !!!";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;// increase for the first token 


            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                curNode.childrenNodes.Add(new Node("("));
            }
            else
            {
                error = "Error in MainFunction ... Left Bracket Not Found  :(  !!!";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_rBracket)
            {
                curNode.childrenNodes.Add(new Node(")"));
            }
            else
            {
                error = "Error in MainFunction ... Right Bracket Not Found  :(  !!!";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            node1 = FunctionBody();
            if (node1.nodeErrors.Count == 0)
            {
                curNode.childrenNodes.Add(node1);
            }
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node Function()
        {
            Node curNode = new Node("Function");
            Node node1 = FunctionDecl();// 1st function declaration 
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = FunctionBody();// 2nd function body
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node FunctionDecl()
        {
            Node curNode = new Node("FunctionDecl");
            string error = "";
            Node node1 = Decl1Var();// 1st declare 1 variable for function datatype and function name 
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count &&
                myTokens[tokensPointer].Value != TinyToken.t_lBracket)// 2nd : the left bracket '('
            {
                error = "Error in Function Declaration ... Expected '('  ";
                if (tokensPointer < myTokens.Count)
                {
                    error += "found: ";
                    error += myTokens[tokensPointer].ToString();
                }
                curNode.nodeErrors.Add(error);
            }
            else
            {
                curNode.childrenNodes.Add(new Node("("));
            }
            tokensPointer++;// increase for the first token

            node1 = ParametersDecl();// 3rd function parameteres declaration 
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }

            if (tokensPointer < myTokens.Count &&
                myTokens[tokensPointer].Value != TinyToken.t_rBracket)
            {
                error = "Error in FunctionDecl ... Expected )  found ";
                error += myTokens[tokensPointer].ToString();
                curNode.nodeErrors.Add(error);
            }
            else
            {
                curNode.childrenNodes.Add(new Node(")"));
            }
            tokensPointer++;
            return curNode;
        }

        public static Node Decl1Var()
        {
            Node curNode = new Node("Decl1Var");
            Node node1 = DataType();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in Variable Declaration ... Expected identifier ";
                if (tokensPointer < myTokens.Count)
                {
                    error += "found: ";
                    error += myTokens[tokensPointer].ToString();
                }
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node ParametersDecl()
        {
            Node curNode = new Node("ParametersDecl");
            int tmpPntr = tokensPointer;
            Node node1 = DataType();
            if (node1.nodeErrors.Count == 0)
            {
                tokensPointer = tmpPntr;
                node1 = Parameters();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }

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

            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_lCurlyBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in FunctionBody ... Expected '{'  found ";
                error += myTokens[tokensPointer].ToString();
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;// increase it for the first token in the function

            Node node1 = ManyStatements();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = ReturnStatement();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_rCurlyBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {

                string error = "Error in FunctionBody ... Expected '}' ";
                if(tokensPointer<myTokens.Count)
                {
                    error += "found: ";
                    error += myTokens[tokensPointer].ToString();
                }
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node DataType()
        {
            Node curNode = new Node("DataType");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_int)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_float)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_string)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Not a Valid DataType :( !!!!";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;// increase it for the first token in the function
            return curNode;
        }
        public static Node Parameters()
        {
            Node curNode = new Node("Parameters");
            Node node1 = Decl1Var();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = ParametersDash();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node ParametersDash()
        {
            Node curNode = new Node("ParametersDash");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_comma)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;
                Node node1 = Decl1Var();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                node1 = ParametersDash();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }

            }
            else
            {
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node ManyStatements()
        {
            Node curNode = new Node("ManyStatements");
            int tmpPntr = tokensPointer;
            if (StatementLookAhead())
            {
                Node node1 = Statement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                node1 = ManyStatements();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node Statement()
        {
            Node curNode = new Node("Statement");

            int tmpPntr = tokensPointer;
            Node dtpye = DataType();
            if (dtpye.nodeErrors.Count == 0)
            {
                Node node1 = DeclarationStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_if)
            {
                tokensPointer = tmpPntr;
                Node node1 = IfStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_write)
            {
                Node node1 = WriteStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_read)
            {
                Node node1 = ReadStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_return)
            {
                Node node1 = ReturnStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_repeat)
            {
                Node node1 = RepeatStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier && tokensPointer + 1 < myTokens.Count && myTokens[tokensPointer + 1].Value == TinyToken.t_lBracket)
            {
                Node node1 = FunctionCall();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                Node node1 = AssignmentStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            return curNode;
        }
        public static Node IfStatement()
        {
            Node curNode = new Node("IfStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_if)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in IfStatement ... couldn't find reserved keyword 'if' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;

            Node node1 = ConditionStatement();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }

            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_then)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in IfStatement ... couldn't find reserved keyword 'then' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            node1 = StatmentsForIf();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = Ifff();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node ConditionStatement()
        {
            Node curNode = new Node("ConditionStatement");
            Node node1 = Condition();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = ConditionStatementDash();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node ConditionStatementDash()
        {
            Node curNode = new Node("ConditionStatementDash");
            int tmpPntr = tokensPointer;
            Node node1=BooleanOperator();
            if(node1.nodeErrors.Count==0)
            {
                curNode.childrenNodes.Add(node1);
                node1 = Condition();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                node1 = ConditionStatementDash();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node Condition()
        {
            Node curNode = new Node("Condition");

            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in Condition ... Couldn't find the Identifier ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = ConditionOperator();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = Term();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node ConditionOperator()
        {
            Node curNode = new Node("ConditionOperator");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_lessThan)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_biggerThan)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_notEqual)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_isEqual)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Couldn't find a Condition Operator  :( !!!!";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;// increase it for the first token in the function
            return curNode;
        }
        public static Node Term()
        {
            Node curNode = new Node("Term");
            int tmpPntr = tokensPointer;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier && tokensPointer + 1 < myTokens.Count && myTokens[tokensPointer + 1].Value == TinyToken.t_lBracket)
            {
                Node node1 = FunctionCall();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_number)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
                {
                    curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                }
                else
                {
                    string error = "Couldn't find an identifier  :( !!!!";
                    curNode.nodeErrors.Add(error);
                }
            }

            return curNode;
        }
        public static Node FunctionCall()
        {
            Node curNode = new Node("FunctionCall");

            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find the function name";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find left brackt '(' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = ManyIdentifiers();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }

            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_rBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find the Right Bracket ')'";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in FunctionCall ... couldn't find the SemiColon ';' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node ManyIdentifiers()
        {
            Node curNode = new Node("ManyIdentifiers");
            int tmpPntr = tokensPointer;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                Node node1 = Identifiers();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node Identifiers()
        {
            Node curNode = new Node("Identifiers");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in Identifiers ... couldn't find an identifier ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = IdentifiersDash();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node IdentifiersDash()
        {
            Node curNode = new Node("IdentifiersDash");
            Node node1;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_comma)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;
                if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
                {
                    curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                }
                else
                {
                    string error = "Error in Function Call ... couldn't find the identifier ";
                    curNode.nodeErrors.Add(error);
                }
                tokensPointer++;
                node1 = IdentifiersDash();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }

            }
            else
            {
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node BooleanOperator()
        {
            Node curNode = new Node("BooleanOperator");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_and)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_or)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Couldn't find a Boolean Operator  :( !!!!";
                curNode.nodeErrors.Add(error);
            }
            return curNode;
        }
        public static Node StatmentsForIf()
        {
            Node curNode = new Node("StatmentsForIf");
            Node node1 = Statement();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = StatmentsForIfDash();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static bool StatementLookAhead()
        {
            // used else if instead of or ,, to make the code  more readable ,, tmam ??
            if(tokensPointer<myTokens.Count &&myTokens[tokensPointer].Value==TinyToken.t_if)
            {
                return true;
            }
            else if(tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_write)
            {
                return true;
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_read)
            {
                return true;
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_return)
            {
                return true;
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                return true;
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_repeat)
            {
                return true;
            }
            else
            {
                Node node1 = DataType();
                if (node1.nodeErrors.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static Node StatmentsForIfDash()
        {
            Node curNode = new Node("StatmentsForIfDash");
            int tmpPntr = tokensPointer;
            if(StatementLookAhead())
            {
                Node node1 = StatmentsForIf();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node Ifff()
        {
            Node curNode = new Node("Ifff");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_elseif)
            {
                Node node1 = ElseIfStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }

            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_else)
            {
                Node node1 = ElseStatement();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }

            }
            else
            {
                if (myTokens[tokensPointer].Value == TinyToken.t_end)
                {
                    curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                }
                else
                {
                    string error = "couldn't find reserved keyword 'end' ";
                    curNode.nodeErrors.Add(error);
                }
                tokensPointer++;
            }

            return curNode;
        }
        public static Node ElseIfStatement()
        {
            Node curNode = new Node("ElseIfStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_elseif)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ElseIfStatement ... couldn't find reserved keyword 'elseif' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;

            Node node1 = ConditionStatement();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }

            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_then)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ElseIfStatement ... couldn't find reserved keyword 'then' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            node1 = StatmentsForIf();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = Ifff();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node ElseStatement()
        {
            Node curNode = new Node("ElseStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_else)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ElseStatement ... couldn't find reserved keyword 'else' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = StatmentsForIf();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_end)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ElseStatement ... couldn't find reserved keyword 'end' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return null;
        }
        public static Node WriteStatement()
        {
            Node curNode = new Node("WriteStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_write)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in WriteStatement ... couldn't find reserved keyword 'write' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = Something();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_comma)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in WriteStatement ... couldn't find reserved keyword ';' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return null;
        }
        public static Node Something()
        {
            Node curNode = new Node("Something");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_endl)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;
            }
            else
            {
                Node node1 = Expression();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            return curNode;
        }
        public static Node Expression()
        {
            Node curNode = new Node("Expression");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_constantString)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;
            }
            else
            {
                Node node1 = Equation();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            return curNode;
        }
        public static Node Equation()// Equation CFG needs to be modified 
        {
            Node curNode = new Node("Equation");
            Node node1 = HelperTerm();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = EquationDash();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node EquationDash()// Equation CFG needs to be modified 
        {
            Node curNode = new Node("EquationDash");
            int tmpPntr = tokensPointer;
            Node node1 = ArthimiticOperator();
            if (node1.nodeErrors.Count == 0)
            {
                curNode.childrenNodes.Add(node1);
                node1 = HelperTerm();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                node1 = EquationDash();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node HelperTerm()
        {
            Node curNode = new Node("HelperTerm");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                Node node1 = HelperEquation();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                Node node1 = Term();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            return curNode;
        }
        public static Node HelperEquation()
        {
            Node curNode = new Node("HelperEquation");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_lBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in HelperEquation ... couldn't find  '(' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = ManyTerms();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_rBracket)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in HelperEquation ... couldn't find ')' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node ManyTerms()
        {
            Node curNode = new Node("ManyTerms");
            Node node1 = Term();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = ManyTermsDash();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node ManyTermsDash()
        {
            Node curNode = new Node("ManyTermsDash");
            int tmpPntr = tokensPointer;
            Node node1 = ArthimiticOperator();
            if (node1.nodeErrors.Count == 0)
            {
                curNode.childrenNodes.Add(node1);
                node1 = Term();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                node1 = ManyTermsDash();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                tokensPointer = tmpPntr;
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node ArthimiticOperator()
        {
            Node curNode = new Node("ArthimiticOperator");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_plus)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_minus)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_multiply)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_divide)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Couldn't find an ArthimiticOperator Operator  :( !!!!";
                curNode.nodeErrors.Add(error);

            }
            tokensPointer++;// increase it for the first token in the function
            return curNode;
        }
        public static Node ReadStatement()
        {
            Node curNode = new Node("ReadStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_read)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ReadStatement ... couldn't find reserved keyword 'read' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ReadStatement ... couldn't find an identifier ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ReadStatement ... couldn't find ';' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node ReturnStatement()
        {
            Node curNode = new Node("ReturnStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_return)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ReturnStatement ... couldn't find reserved keyword 'return' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = Expression();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ReturnStatement ... couldn't find ';' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node AssignmentStatement()
        {
            Node curNode = new Node("AssignmentStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in AssignmentStatement ... couldn't find an identifier ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_assign)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in AssignmentStatement ... couldn't find ':=' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = Expression();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in AssignmentStatement ... couldn't find  ';' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node RepeatStatement()
        {
            Node curNode = new Node("RepeatStatement");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_repeat)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in RepeatStatement ... couldn't find reserved keyword 'repeat' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = StatmentsForIf();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_until)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in RepeatStatement ... couldn't find reserved keyword 'until' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            node1 = ConditionStatement();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node DeclarationStatement()
        {
            Node curNode = new Node("DeclarationStatement");
            Node node1 = DataType();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = ManyIdentifiersDecl();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_semicolon)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in DeclarationStatement ... couldn't find ';' ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            return curNode;
        }
        public static Node ManyIdentifiersDecl()
        {
            Node curNode = new Node("ManyIdentifiersDecl");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
            }
            else
            {
                string error = "Error in ManyIdentifiersDecl ... couldn't find an identifier ";
                curNode.nodeErrors.Add(error);
            }
            tokensPointer++;
            Node node1 = AssignmentInDecl();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            node1 = ManyIdentifiersDeclDash();
            if (node1.nodeErrors.Count == 0)
                curNode.childrenNodes.Add(node1);
            else
            {
                for (int i = 0; i < node1.nodeErrors.Count; i++)
                {
                    curNode.nodeErrors.Add(node1.nodeErrors[i]);
                }
            }
            return curNode;
        }
        public static Node ManyIdentifiersDeclDash()
        {
            Node curNode = new Node("ManyIdentifiersDeclDash");
            Node node1;
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_comma)
            {
                curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                tokensPointer++;
                if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_identifier)
                {
                    curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                }
                else
                {
                    string error = "Error in Declaration ... couldn't find the identifier ";
                    curNode.nodeErrors.Add(error);
                }
                tokensPointer++;
                node1 = AssignmentInDecl();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
                node1 = ManyIdentifiersDeclDash();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }

            }
            else
            {
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }
        public static Node AssignmentInDecl()
        {
            Node curNode = new Node("AssignmentInDecl");
            if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_assign)
            {
                if (tokensPointer < myTokens.Count && myTokens[tokensPointer].Value == TinyToken.t_assign)
                {
                    curNode.childrenNodes.Add(new Node(myTokens[tokensPointer].Key));
                }
                else
                {
                    string error = "Error in AssignmentInDecl ... couldn't find ':=' ";
                    curNode.nodeErrors.Add(error);
                }
                tokensPointer++;
                Node node1 = Expression();
                if (node1.nodeErrors.Count == 0)
                    curNode.childrenNodes.Add(node1);
                else
                {
                    for (int i = 0; i < node1.nodeErrors.Count; i++)
                    {
                        curNode.nodeErrors.Add(node1.nodeErrors[i]);
                    }
                }
            }
            else
            {
                curNode.childrenNodes.Add(new Node("Epsilon"));
            }
            return curNode;
        }

    }
}
