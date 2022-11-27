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
    public void Start();
    public void End();

    public void HandleComment(string comment, uint line);

    public void HandleProperty(string key, string value, uint line);

    public void HandleMultilineProperty(string? key, string value, uint line);

    public void EnterSection(string section, uint line);

    public bool IsSectionEntered(string section, uint line);

}

public interface IIniDataHandler<T> : IIniDataHandler
{
    public T Get();
}