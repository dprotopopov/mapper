using NpgsqlTypes;

namespace Mapper.Services.Api
{
    public class RelationMember
    {
        [PgName("id")]
        public long Id { get; set; }
        [PgName("role")]
        public string Role { get; set; }
        [PgName("type")]
        public int Type { get; set; }
    }
}