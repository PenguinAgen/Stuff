﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Stuff
{
    public interface ISaveable
    {
        XElement ToXElement(string name);
    }
}
