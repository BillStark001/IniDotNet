using IniDotNet.Integrated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Example;

public record ConfigModel
{
    [IniModel]
    public record General
    {
        [IniProperty("setUpdate")]
        public int Update { get; set; }

        [IniProperty("setMaxErrors")]
        public int MaxErrors { get; set; }

    }


    public General GeneralConfiguration { get; set; } = new();

    public Dictionary<string, string> Users { get; set; } = new();
}
