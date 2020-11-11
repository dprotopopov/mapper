using NpgsqlTypes;

namespace Mapper.Services.Api
{
    public enum OsmType
    {
        [PgName("node")]
        Node,
        [PgName("way")]
        Way,
        [PgName("relation")]
        Relation,
    }
}