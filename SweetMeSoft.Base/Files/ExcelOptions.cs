namespace SweetMeSoft.Base.Files;

public class ExcelOptions
{
    public ExcelOptions(Stream stream, int headerRow)
    {
        Stream = stream;
        HeaderRow = headerRow;
    }

    public Stream Stream { get; set; }

    public Action<int, Exception> ErrorCallback { get; set; }

    public int HeaderRow { get; set; } = 1;
}