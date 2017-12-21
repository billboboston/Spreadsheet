///Author: William Boston
///SID: 11421575
///
/// HW 10
/// Last Modified:
///    3/3/17 
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpts321
{
    

    public class ExpTree
    {
        private BaseNode m_root;                    // Root Node
        private Dictionary<string, double> m_VarDict;    // Variable Dictionary

        //<><><><><> Expression Tree Nodes <><><><><><><>
        /// <summary>
        /// Central Base Class for the Expression Tree
        /// </summary>
        private abstract class BaseNode
        {
            /// <summary>
            /// Evaluation or Execution of the node data
            /// </summary>
            /// <returns></returns>
            public abstract double Eval();
        }

        /// <summary>
        /// Node representing a constant numerical value
        /// </summary>
        private class ConstValNode : BaseNode
        {
            private double m_ConstVal;

            public ConstValNode(double inVal)
            {
                m_ConstVal = inVal;
            }
            public override double Eval()
            {
                return m_ConstVal;
            }
        }

        /// <summary>
        /// Node representian a variable
        /// </summary>
        private class VariableNode : BaseNode
        {
            private string m_Variable;
            Dictionary<string, double> m_VarDict;

            // this function takes a reference to the Global Dictionary that houses all the variables
            public VariableNode(string varName, ref Dictionary<string, double> inDict)
            {
                if (!inDict.ContainsKey(varName))
                {
                    inDict.Add(varName, 0); // placing the Var in the dict with a default val of 0
                }
                m_VarDict = inDict;     //storing a reference of the dict locally
                m_Variable = varName;
            }

            
            public override double Eval()
            {   // getting the value from the dictionary reference
                return m_VarDict[m_Variable]; 
            }
        }

        /// <summary>
        /// Node reresenting a binary operator
        /// </summary>
        private class OperatorNode : BaseNode
        {
            private char Operator;
            private BaseNode LeftNode, RightNode;   // children nodes

            /// <summary>
            /// OperatorNode( Operator , Left Child, Right Child)
            /// </summary>
            /// <param name="op"></param>
            /// <param name="left"></param>
            /// <param name="right"></param>
            public OperatorNode(char op, BaseNode left, BaseNode right)
            {
                Operator = op;
                LeftNode = left;
                RightNode = right;
            }

            public override double Eval()
            {
                double left = LeftNode.Eval();      // Left SubTree evaluation
                double right = RightNode.Eval();    // Right SubTree evaluation

                switch (Operator)
                {

                    case '+':
                        return left + right;
                    case '-':
                        return left - right;
                    case '/':
                        return left / right;
                    case '*':
                        return left * right;
                }
                return double.NaN; // there was no correct output ERROR
            }
        }


        /// <summary>
        /// Constructor: Builds the ExpTree from the Expression string with proper operator precedence
        /// </summary>
        /// <param name="expression"></param>
        public ExpTree(string expression)
        {
            m_VarDict = new Dictionary<string, double>(); // replaces/deletes var mem when a new expression is entered.

            if (expression != "=")
            {
                this.m_root = BuildNodes(expression); // initial recursive call to build expression tree

            }
        }

        /// <summary>
        /// Adds a variable to the Dictionary for reference
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        public void SetVar(string varName, double varValue)
        {
            if (!m_VarDict.ContainsKey(varName)) // if key dosent exist
            {
                m_VarDict.Add(varName, varValue); //create new variable
            }
            else
            {
                m_VarDict[varName] = varValue; // overwiting with new data
            }
        }

        /// <summary>
        /// Returns: list of variable names as strings;
        /// </summary>
        /// <returns></returns>
        public List<string> getVarNames()
        {
            List<string> VarList = new List<string>();
            foreach (string s in m_VarDict.Keys)
            {
                VarList.Add(s);
            }
            return VarList;
        }

        

        /// <summary>
        /// Return: result of Exp tree as a double
        /// </summary>
        /// <returns></returns>
        public double Eval()
        {
            if (m_root != null)
            {
                return m_root.Eval();   // first call of Tree Eval process
            }
            else
                return double.NaN;  // there is no tree
        }

       

        /// <summary>
        /// ExpTree Helper Function:
        /// >>Recursively Builds the expression Tree -> returns the RootNode
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private BaseNode BuildNodes(string expression)
        {
            expression = expression.Replace(" ", "");
            
            if (expression[expression.Length - 1] == ')')       // parens Evaluation
            {
                int counter = 0;
                for (int i = expression.Length - 1; i >= 0; i--)
                {
                    if (expression[i] == ')')
                    {
                        counter++;
                    }
                    if (expression[i] == '(') // if iterator found an end parens
                    {
                        counter--;
                        if (counter == 0)
                        {
                            if (i == 0)// if ist the closing parens
                            {   // removes wrapping parens and recalls the build evaluator
                                return BuildNodes(expression.Substring(1, expression.Length - 2));
                            }
                            else // its found a sub expression
                            {
                                return new OperatorNode(expression[i - 1],
                                    BuildNodes(expression.Substring(0, i - 1)),
                                    BuildNodes(expression.Substring(i)));
                            }
                        }
                    }
                }
            }

            for (int l = expression.Length - 1; l >= 0; l--) // '+ -' evaluation
            {
                switch (expression[l]) // switch for operators in string
                {
                    case ')':
                        return new OperatorNode(
                                expression[l + 1],
                                BuildNodes(expression.Substring(1, l - 1)),
                                BuildNodes(expression.Substring(l + 2)));
                    case '+':
                    case '-':
                        return new OperatorNode(
                                expression[l],
                                BuildNodes(expression.Substring(0, l)),     // Left SubTree recursive call
                                BuildNodes(expression.Substring(l + 1)));   // Right SubTree recursive Call
                }
            }

            for (int l = expression.Length - 1; l >= 0; l--) // '* /' evaluation
            {
                switch (expression[l]) // switch for operators in string
                {
                    case ')':// if we found a sub expression 
                        return new OperatorNode(
                                expression[l + 1],
                                BuildNodes(expression.Substring(1, l - 1)),
                                BuildNodes(expression.Substring(l + 2)));
                    case '*':
                    case '/':
                        return new OperatorNode(expression[l],
                                BuildNodes(expression.Substring(0, l)),     // Left SubTree recursive call
                                BuildNodes(expression.Substring(l + 1)));   // Right SubTree recursive Call
                }
            }

            // if there are no remaining operators
            double num;
            if (double.TryParse(expression, out num))           // if Data is just a number
            {
                return new ConstValNode(num);
            }
            
            
             return new VariableNode(expression, ref m_VarDict);   // if data is a variable
           
        }
    }
}
