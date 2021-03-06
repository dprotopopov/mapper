﻿using System.Threading.Tasks;
using Mapper.Extensions;
using Mapper.Types.Osm;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OsmSharp;

namespace Mapper.Services.Api
{
    public class OsmApiService
    {
        private readonly IConfiguration _configuration;

        private readonly string _selectNodeById = @"SELECT 
        id,
        version,
        latitude,
        longitude,
        change_set_id,
        time_stamp,
        user_id,
        user_name,
        visible,
        hstore_to_json(tags)
        FROM node
        WHERE id=@p";

        private readonly string _selectRelationById = @"SELECT 
        id,
        version,
        change_set_id,
        time_stamp,
        user_id,
        user_name,
        visible,
        hstore_to_json(tags),
        members
        FROM relation
        WHERE id=@p";

        private readonly string _selectWayById = @"SELECT 
        id,
        version,
        change_set_id,
        time_stamp,
        user_id,
        user_name,
        visible,
        hstore_to_json(tags),
        nodes
        FROM way
        WHERE id=@p";

        public OsmApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<OsmGeo> GetOsmById(long osm_id, OsmType type)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                connection.TypeMapper.MapComposite<RelationMember>("relation_member");
                connection.TypeMapper.MapEnum<OsmType>("osm_type");
                connection.TypeMapper.MapEnum<ServiceType>("service_type");

                switch (type)
                {
                    case OsmType.Node:
                        using (var command = new NpgsqlCommand(_selectNodeById, connection))
                        {
                            command.Parameters.AddWithValue("p", osm_id);
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var result = new Node();
                                    result.Fill(reader);
                                    return result;
                                }
                            }
                        }

                        break;
                    case OsmType.Way:

                        using (var command = new NpgsqlCommand(_selectWayById, connection))
                        {
                            command.Parameters.AddWithValue("p", osm_id);
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var result = new Way();
                                    result.Fill(reader);
                                    return result;
                                }
                            }
                        }

                        break;
                    case OsmType.Relation:
                        using (var command = new NpgsqlCommand(_selectRelationById, connection))
                        {
                            command.Parameters.AddWithValue("p", osm_id);
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var result = new Relation();
                                    result.Fill(reader);
                                    return result;
                                }
                            }
                        }

                        break;
                }
            }

            return null;
        }


        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("OsmConnection");
        }
    }
}