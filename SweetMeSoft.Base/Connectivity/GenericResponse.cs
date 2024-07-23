namespace SweetMeSoft.Base.Connectivity;

public class GenericResponse<T>
{
    public HttpResponseMessage HttpResponse { get; set; }

    public string Cookies { get; set; } = "";

    public T Object { get; set; }

    public ErrorDetails Error { get; set; }
}