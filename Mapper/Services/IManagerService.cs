using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace Mapper.Services
{
    public interface IManagerService
    {
        Task Install(Stream uploadStream, NpgsqlConnection connection, string session);
        Task Update(Stream uploadStream, NpgsqlConnection connection, string session);
    }
}