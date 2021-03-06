﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stuff.StuffMath.Expressions.Operators
{
    public class Subtract : Expression
    {
        private Expression left;

        private Expression right;

        public override double Priority
        {
            get
            {
                return 3;
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        public Subtract(Expression leftHand, Expression rightHand)
        {
            this.left = leftHand;
            this.right = rightHand;
        }

        public override double Evaluate(Dictionary<string, double> values)
        {
            return left.Evaluate(values) - right.Evaluate(values);
        }

        public override Expression Differentiate(string variable)
        {
            return left.Differentiate(variable) - right.Differentiate(variable);
        }

        public override Expression Reduce(Dictionary<string, double> values = null)
        {
            Expression leftReduced = left.Reduce(values);
            Expression rightReduced = right.Reduce(values);
            if (leftReduced is ValueExpression && rightReduced is ValueExpression)
                return new ValueExpression(leftReduced.Evaluate() - rightReduced.Evaluate());
            else if (leftReduced is ValueExpression && leftReduced.Evaluate() == 0)
                return rightReduced;
            else if (rightReduced is ValueExpression && rightReduced.Evaluate() == 0)
                return leftReduced;
            else
                return leftReduced - rightReduced;
        }

        public override bool IsEqual(Expression exp)
        {
            if (exp is Subtract)
            {
                var subtract = (Subtract)exp;
                return subtract.left.IsEqual(left) && subtract.right.IsEqual(right);
            }
            return false;
        }

        public override bool ContainsVariable(string variable)
        {
            return left.ContainsVariable(variable) || right.ContainsVariable(variable);
        }

        public override HashSet<string> ContainedVariables(HashSet<string> vars)
        {
            return left.ContainedVariables(right.ContainedVariables(vars));
        }

        public override string ToString()
        {
            return "(" + left.ToString() + " - " + right.ToString() + ")";
        }

        public override string ToLatex()
        {
            return left.ToLatex() + "-" + right.ToLatex();
        }
    }
}
