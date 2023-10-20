using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Base;

public interface IIniHandler
{
    public void Clear();
    public void Start();
    public void End();

    public void HandleComment(string comment, uint line);

    public void HandleProperty(string key, string value, uint line);


    public void EnterSection(string section, uint line);

    public void LeaveSection(uint line);

    public void EnterSubsection(string section, uint line);

    public void LeaveSubsection(uint line);

    public bool IsSectionEntered(string section, uint line);

}

public interface IIniHandler<T> : IIniHandler
{
    public T Get();
}