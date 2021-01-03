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
        public Node(string name)
        {
            NodeName = name;
            childrenNodes = new List<Node>();
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
            int sz = root.childrenNodes.Count;
            if (sz<1 || root.childrenNodes[sz-1].NodeName!= "MainFunction")
            {
                parserErrors.Add("Main Function isn't implemented properly or not implemented at all !!!");
            }
        }
        public static Node Program()
        {
            Node curNode = new Node("Program");
            int tmpPntr = tokensPointer;
            Node node1 = MainFunction(); // 1st main functoin
            if(node1!=null)
            {
                curNode.childrenNodes.Add(node1);
                return curNode;
            }
            else
            {
                tokensPointer = tmpPntr;/*
                                         * we found it's not a main function 
                                         * so get back the tokens pointer
                                         * to where it was before trying to match it with main function
                                         * */
                tmpPntr = tokensPointer;
                node1 = Function();// 2nd recursion for functoin
                if (node1 != null)
                {
                    curNode.childrenNodes.Add(node1);
                    Program();
                    return curNode;
                }
                else tokensPointer = tmpPntr;
            }
            return curNode;
        }
        public static Node MainFunction()
        {
            Node curNode = new Node("MainFunction");
            Node node1 = DataType();
            if(node1!=null)
            {
                curNode.childrenNodes.Add(node1);
                if((myTokens[tokensPointer].Value==TinyToken.t_identifier && myTokens[tokensPointer++].Key=="main")
                    && myTokens[tokensPointer++].Value==TinyToken.t_lBracket
                    && myTokens[tokensPointer++].Value == TinyToken.t_rBracket)
                {
                    node1 = FunctionBody();
                    if(node1!=null)
                    {
                        curNode.childrenNodes.Add(node1);
                        return curNode;
                    }
                }
            }
            return null;
        }
        public static Node Function()
        {
            Node curNode = new Node("Function");
            Node node1 = FunctionDecl();// 1st function declaration 
            if(node1!=null)
                curNode.childrenNodes.Add(node1);
            node1 = FunctionBody();// 2nd function body
            if(node1!=null)
                curNode.childrenNodes.Add(node1);
            if (curNode.childrenNodes.Count == 2) return curNode;
            return null;
        }
        public static Node FunctionDecl()
        {
            Node curNode = new Node("FunctionDecl");
            string error = "";
            Node node1 = Decl1Var();// 1st declare 1 variable for function datatype and function name 
            if(node1!=null)
                curNode.childrenNodes.Add(node1);

            if (tokensPointer<myTokens.Count &&
                myTokens[tokensPointer].Value != TinyToken.t_lBracket)// 2nd : the left bracket '('
            {
                error = "Error in FunctionDecl ... Expected (   found ";
                error += myTokens[tokensPointer].ToString();
                parserErrors.Add(error);
                error = "";
                tokensPointer++;
            }
            else curNode.childrenNodes.Add(new Node("("));

            node1 = ParametersDecl();// 3rd function parameteres declaration 
            if (node1 != null)
                curNode.childrenNodes.Add(node1);

            if (tokensPointer < myTokens.Count && 
                myTokens[tokensPointer].Value != TinyToken.t_rBracket)// 4th : the right bracket ')'
            {
                error = "Error in FunctionDecl ... Expected )   found ";
                error += myTokens[tokensPointer].ToString();
                parserErrors.Add(error);
                error = "";
                tokensPointer++;
            }
            else curNode.childrenNodes.Add(new Node(")"));

            return null;
        }
        
        public static Node Decl1Var()
        {
            Node curNode = new Node("Decl1Var");
            Node node1 = DataType();
            if(node1!=null)
            {
            }
            return null;
        }
        public static Node ParametersDecl()
        {
            Node curNode = new Node("ParametersDecl");
            /// To be implmeneted 
            return null;
        }
        public static Node FunctionBody()
        {
            Node curNode = new Node("FunctionBody");
            /// To be implmeneted 
            return null;
        }
        public static Node DataType()
        {
            Node curNode = new Node("DataType");
            /// To be implmeneted 
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
            /// To be implmeneted 
            return null;
        }
        public static Node IfStatement()
        {
            Node curNode = new Node("IfStatement");
            /// To be implmeneted 
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
            /// To be implmeneted 
            return null;
        }
        public static Node ConditionOperator()
        {
            Node curNode = new Node("ConditionOperator");
            /// To be implmeneted 
            return null;
        }
        public static Node Term()
        {
            Node curNode = new Node("Term");
            /// To be implmeneted 
            return null;
        }
        public static Node FunctionCall()
        {
            Node curNode = new Node("FunctionCall");
            /// To be implmeneted 
            return null;
        }
        public static Node ManyIdentifiers()
        {
            Node curNode = new Node("ManyIdentifiers");
            /// To be implmeneted 
            return null;
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
            /// To be implmeneted 
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
            /// To be implmeneted 
            return null;
        }
        public static Node ElseIfStatement()
        {
            Node curNode = new Node("ElseIfStatement");
            /// To be implmeneted 
            return null;
        }
        public static Node WriteStatement()
        {
            Node curNode = new Node("WriteStatement");
            /// To be implmeneted 
            return null;
        }
        public static Node Expression()
        {
            Node curNode = new Node("Expression");
            /// To be implmeneted 
            return null;
        }
        public static Node Equation()// Equation CFG needs to be modified 
        {
            Node curNode = new Node("Equation");
            /// To be implmeneted 
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
            /// To be implmeneted 
            return null;
        }
        public static Node ReadStatement()
        {
            Node curNode = new Node("ReadStatement");
            /// To be implmeneted 
            return null;
        }
        public static Node ReturnStatement()
        {
            Node curNode = new Node("ReturnStatement");
            /// To be implmeneted 
            return null;
        }
        public static Node AssignmentStatement()
        {
            Node curNode = new Node("AssignmentStatement");
            /// To be implmeneted 
            return null;
        }
        public static Node RepeatStatement()
        {
            Node curNode = new Node("RepeatStatement");
            /// To be implmeneted 
            return null;
        }
        public static Node DeclarationStatement()
        {
            Node curNode = new Node("DeclarationStatement");
            /// To be implmeneted 
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
            /// To be implmeneted 
            return null;
        }

    }
}
