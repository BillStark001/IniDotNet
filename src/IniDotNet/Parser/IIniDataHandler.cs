using IniDotNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Parser;

public interface IIniDataHandler
{
    public void Clear();
    public void SetScheme(IniScheme scheme);
}