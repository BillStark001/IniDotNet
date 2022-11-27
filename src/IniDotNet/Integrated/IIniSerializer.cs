using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated;


public interface IIniSerializer<T>
{

    public string Serialize(T? value);
    public T? Deserialize(string? value);
}