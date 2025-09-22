using ScriptiumBackend.Entities;

namespace ScriptiumBackend.Interface;

using System.Threading.Tasks;


public interface ISearchService
{
    Task<SearchResultDto> SearchAsync(string queryText);
}
