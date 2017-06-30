namespace Owpini.EntityFramework
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
