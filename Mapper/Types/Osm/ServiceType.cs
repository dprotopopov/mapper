using NpgsqlTypes;

namespace Mapper.Types.Osm
{
    public enum ServiceType
    {
        [PgName("node")]
        Node,
        [PgName("way")]
        Way,
        [PgName("relation")]
        Relation,
        [PgName("place")]
        Place,
    }
}