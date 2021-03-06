﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stuff.StuffMath.Structures
{
    public interface IHilbertField<T> : IField<T>, ILatexable where T : IHilbertField<T>, new()
    {
        T AbsSqrt();

        T Conjugate();
    }
}
