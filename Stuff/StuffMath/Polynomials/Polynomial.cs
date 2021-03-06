﻿using Stuff.StuffMath.Expressions;
using Stuff.StuffMath.Expressions.Operators;
using Stuff.StuffMath.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stuff.StuffMath
{
    public class Polynomial : IPolynomial
    {
        private readonly IReadOnlyDictionary<int, double> coefficients;

        public IPolynomial ZERO => new Polynomial();

        public Real ONE => 1;

        public int Degree => coefficients.Keys.Where(k => coefficients[k] != 0).Max();

        /// <summary>
        /// Creates a new Polynomial with all coefficients set to 0.
        /// </summary>
        public Polynomial()
        {
            coefficients = new Dictionary<int, double>();
        }

        public Polynomial(params (int exponent, double coef)[] coefs)
        {
            var tempCoefs = new Dictionary<int, double>();
            foreach (var (exp, coef) in coefs)
            {
                if (exp < 0)
                    throw new Exception("Polynomials cannot contain exponents below 0.");
                if (coef != 0)
                    tempCoefs[exp] = coef;
            }
            coefficients = tempCoefs;
        }

        public Polynomial(params double[] coefs)
        {
            var tempCoefs = new Dictionary<int, double>();
            for (int i = 0; i < coefs.Length; ++i)
            {
                if (coefs[i] != 0)
                    tempCoefs[i] = coefs[i];
            }
            coefficients = tempCoefs;
        }

        public Polynomial(Dictionary<int, double> coefs)
        {
            var tempCoefs = new Dictionary<int, double>();
            foreach (var (exp, coef) in coefs)
            {
                if (exp < 0)
                    throw new Exception("Polynomials cannot contain exponents below 0.");
                if (coef != 0)
                    tempCoefs[exp] = coef;
            }
            coefficients = tempCoefs;
        }

        public Polynomial(Vector<FDouble> v)
        {
            var tempCoefs = new Dictionary<int, double>();
            for (int i = 0; i < v.Size; ++i)
            {
                if (v[i] != 0)
                    tempCoefs[i] = v[i];
            }
            coefficients = tempCoefs;
        }

        public static Polynomial operator+(Polynomial pol1, Polynomial pol2)
        {
            var result = new Dictionary<int, double>();

            foreach (var coef in pol1.coefficients)
                result[coef.Key] = coef.Value;

            foreach (var coef in pol2.coefficients)
            {
                if (result.ContainsKey(coef.Key))
                    result[coef.Key] += coef.Value;
                else
                    result[coef.Key] = coef.Value;
            }

            return new Polynomial(result);
        }

        public static Polynomial operator-(Polynomial pol1, Polynomial pol2)
        {
            var result = new Dictionary<int, double>();

            foreach (var coef in pol1.coefficients)
                result[coef.Key] = coef.Value;

            foreach (var coef in pol2.coefficients)
            {
                if (result.ContainsKey(coef.Key))
                    result[coef.Key] -= coef.Value;
                else
                    result[coef.Key] = -coef.Value;
            }

            return new Polynomial(result);
        }

        public static Polynomial operator -(Polynomial pol)
        {
            return new Polynomial(pol.coefficients.Select(kv => (kv.Key, -kv.Value)).ToArray());
        }

        public static Polynomial operator *(Polynomial pol, double d)
        {
            return new Polynomial(pol.coefficients.Select(kv => (kv.Key, kv.Value * d)).ToArray());
        }

        public static Polynomial operator *(Polynomial p1, Polynomial p2)
        {
            var resultDegree = p1.Degree + p2.Degree;
            var result = new double[resultDegree + 1];
            for (int i = 0; i < resultDegree + 1; ++i)
            {
                var coef = 0d;
                for (int j = 0; j < p1.Degree + 1; ++j)
                    coef += p1[j] * p2[i - j];
                result[i] = coef;
            }
            return new Polynomial(result);
        }

        public static bool operator ==(Polynomial pol1, IPolynomial pol2)
        {
            return pol1.Equals(pol2);
        }

        public static bool operator !=(Polynomial pol1, IPolynomial pol2)
        {
            return !pol1.Equals(pol2);
        }

        public double Y(double x)
        {
            return coefficients.Sum(coef => Math.Pow(x, coef.Key) * coef.Value);
        }

        public (Polynomial quo, Polynomial mod) Divide(Polynomial p)
        {
            var quo = new Polynomial();
            var pol = this;
            while (pol.Degree >= p.Degree)
            {
                var factor = new Polynomial((pol.Degree - p.Degree, pol.GreatestCoef() / p.GreatestCoef()));
                pol -= p * factor;
                quo += factor;
            }
            return (quo, pol);
        }

        public IPolynomial Differentiate()
        {
            var result = new Dictionary<int, double>();
            foreach (var coef in coefficients)
            {
                if (coef.Key != 0)
                    result[coef.Key - 1] = coef.Value == double.NaN ? double.NaN : coef.Value * coef.Key;
            }
            return new Polynomial(result);
        }

        public static LEMatrix DifferentiationMatrix(int degree)
        {
            var result = new LEMatrix.MatrixRow[degree];
            for (int j = 0; j < result.Length; ++j)
            {
                var row = new double[degree + 1];
                for (int i = 0; i < row.Length; ++i)
                {
                    if (i - j == 1)
                        row[i] = i;
                    else
                        row[i] = 0;
                }
                result[j] = new LEMatrix.MatrixRow(row);
            }
            return new LEMatrix(result);
        }

        public IPolynomial Integrate()
        {
            var result = new Dictionary<int, double>();
            foreach (var coef in coefficients)
            {
                result[coef.Key + 1] = coef.Value == double.NaN ? double.NaN : coef.Value / (coef.Key + 1);
            }
            result[0] = double.NaN;
            return new Polynomial(result);
        }

        public double Integrate(double a, double b)
        {
            return coefficients.Sum(coef => Math.Pow(b, coef.Key + 1) * (coef.Value / (coef.Key + 1)) - Math.Pow(b, coef.Key + 1) * (coef.Value / (coef.Key + 1)));
        }

        public double Coefficient(int exponent)
        {
            return coefficients.ContainsKey(exponent) ? coefficients[exponent] : 0;
        }

        public double this[int exponent] => Coefficient(exponent);

        public double GreatestCoef()
        {
            return Coefficient(Degree);
        }

        public Polynomial AsPolynomial() => this;

        public Vector<FDouble> ToVector()
        {
            var result = new FDouble[Degree + 1];
            for (int i = 0; i <= Degree; ++i)
            {
                if (coefficients.ContainsKey(i))
                    result[i] = coefficients[i];
                else
                    result[i] = 0;
            }
            return new Vector<FDouble>(result);
        }

        /// <summary>
        /// The given expression may only contain one variable.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="degree"></param>
        /// <param name="variableName"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IPolynomial Taylor(Expression exp, int degree, string variableName = "x", double point = 0)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            Dictionary<string, double> vars = new Dictionary<string, double>
            {
                [variableName] = point
            };

            result[0] = exp.Evaluate(vars);
            long factorial = 1;
            for (int i = 1; i <= degree; i++)
                result[i] = (exp = exp.Differentiate(variableName).Reduce()).Evaluate(vars) / (factorial *= i);
            return new Polynomial(result);
        }

        public IPolynomial MoveVertical(double k)
        {
            var result = coefficients.Copy();

            if (coefficients.ContainsKey(0))
                result[0] += k;
            else
                result[0] = k;
            return new Polynomial(result);
        }

        public IPolynomial MoveHoriz(double k)
        {
            var result = new Polynomial();

            foreach (var coef in coefficients)
            {
                var coefPol = new Dictionary<int, double>();
                for (int i = coef.Key; i >= 0; i--)
                    coefPol[i] = Basic.Factorial(coef.Key)/(Basic.Factorial(i)*Basic.Factorial(coef.Key-i))*coef.Value * Math.Pow(-k, coef.Key - i);
                result += new Polynomial(coefPol);
            }
            return result;
        }

        public Expression ToExpression(string variableName)
        {
            return coefficients.Select(c => c.Value * new Power(new Variable(variableName), c.Key)).Aggregate((coef, result) => result + coef);
        }

        public override string ToString()
        {
            string result = "";
            foreach (var (exp, coef) in coefficients.OrderBy(coef => coef.Key))
            {
                if (coef != 0)
                {
                    bool neg = coef < 0;
                    var newCoef = neg ? -coef : coef;
                    if (exp == 0)
                        result += (neg ? " - " : " + ") + newCoef;
                    else if (exp == 1)
                        result += (neg ? " - " : " + ") + (coef == 1 ? "" : "" + newCoef) + "x";
                    else
                        result += (neg ? " - " : " + ") + (coef == 1 ? "" : "" + newCoef) + "x^" + exp;
                }
            }

            return (result.Length > 0 ? (result.StartsWith(" - ") ? result.Substring(1) : result.Substring(3)) : " 0");
        }

        public override bool Equals(object obj)
        {
            if (obj is IPolynomial p)
            {
                for (int i = 0; i < Math.Max(Degree, p.Degree); ++i)
                {
                    if (this[i] != p[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Misc.HashCode(17, 23, coefficients.Select(kv => kv.Key * 3 + kv.Value * 5));
        }

        public IPolynomial Add(IPolynomial t)
        {
            return this + t.AsPolynomial();
        }

        public IPolynomial AdditiveInverse()
        {
            return -this;
        }

        public IPolynomial Multiply(Real s)
        {
            return this * (double)s;
        }

        public bool EqualTo(IPolynomial t)
        {
            return Equals(t);
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < coefficients.Keys.Max(); ++i)
                yield return coefficients.ContainsKey(i) ? coefficients[i] : 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}