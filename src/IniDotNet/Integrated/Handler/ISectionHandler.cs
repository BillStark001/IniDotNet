using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Integrated.Handler;

public interface ISectionHandler
{

    public void Start();

    public void Put(string key, string value);

    public void PutObject(string key, object? value);

    public object? End();

    public (ISectionHandler?, Type) GetSectionHandler(string section);
}

public class InvalidTypeHandler : ISectionHandler
{
    public string Message { get; set; } = "";

    public object? End()
    {
        throw new InvalidOperationException(Message);
    }

    public void Put(string key, string value)
    {
        throw new InvalidOperationException(Message);
    }

    public void PutObject(string key, object? value)
    {
        throw new InvalidOperationException(Message);
    }

    public void Start()
    {
        // do nothing
    }

    public (ISectionHandler?, Type) GetSectionHandler(string section)
    {
        throw new InvalidOperationException(Message);
    }
}
