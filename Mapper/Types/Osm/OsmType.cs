using NpgsqlTypes;

namespace Mapper.Types.Osm
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