using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace Mapper.Services.Upload
{
    public interface IUploadrService
    {
        Task Install(Stream uploadStream, NpgsqlConnection connection, string session);
        Task Update(Stream uploadStream, NpgsqlConnection connection, string session);
    }
}