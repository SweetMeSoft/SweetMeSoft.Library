using System.Collections;

namespace HughesNet.Base.Files;

public class ExcelSheet
{
    public ExcelSheet(string name, IEnumerable list)
    {
        Name = name;
        List = list;
    }

    public string Name { get; set; }

    public IEnumerable List { get; set; }
}