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

    public void HandleComment(string comment, int line);

    public void HandleProperty(string key, string value, int line);

    public void HandleMultilineProperty(string value, int line);

    public void EnterSection(string nspace, int line);

}