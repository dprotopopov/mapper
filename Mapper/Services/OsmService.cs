using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mapper.Extensions;
using Mapper.Hubs;
using Newtonsoft.Json;
using Npgsql;
using OsmSharp;
using OsmSharp.Streams;

namespace Mapper.Services
{
    public class OsmService : IManagerService
    {
        private readonly ProgressHub _progressHub;

        public OsmService(ProgressHub progressHub)
        {
            _progressHub = progressHub;
        }

        public async Task Install(Stream uploadStream, NpgsqlConnection connection, string session)
        {
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Mapper.Sql.Osm.CreateTables.sql"))
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            using (var command = new NpgsqlCommand(await sr.ReadToEndAsync(), connection))
            {
                command.ExecuteNonQuery();
            }

            long count = 0;
            using (var source = new PBFOsmStreamSource(uploadStream))
            {
                foreach (var element in source)
                {
                    switch (element)
                    {
                        case Node node:
                            var nodeDictionary = new Dictionary<string, string>
                            {
                                {"Id", node.Id.ToString()},
                                {"Version", node.Version.ToString()},
                                {"Latitude", node.Latitude.ValueAsText()},
                                {"Longitude", node.Latitude.ValueAsText()},
                                {"ChangeSetId", node.ChangeSetId.ToString()},
                                {"TimeStamp", node.TimeStamp.ValueAsText()},
                                {"UserId", node.UserId.ToString()},
                                {"UserName", node.UserName.ValueAsText()},
                                {"Visible", node.Visible.ToString()},
                                {
                                    "Tags",
                                    $"'{string.Join(", ", node.Tags.Select(t => $"\"{t.Key.TextEscape(true)}\" => \"{t.Value.TextEscape(true)}\""))}'"
                                }
                            };

                            using (var command = new NpgsqlCommand(
                                $"INSERT INTO Node ({string.Join(", ", nodeDictionary.Keys)}) VALUES ({string.Join(", ", nodeDictionary.Values)})",
                                connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            break;
                        case Way way:
                            var wayDictionary = new Dictionary<string, string>
                            {
                                {"Id", way.Id.ToString()},
                                {"Version", way.Version.ToString()},
                                {"ChangeSetId", way.ChangeSetId.ToString()},
                                {"TimeStamp", way.TimeStamp.ValueAsText()},
                                {"UserId", way.UserId.ToString()},
                                {"UserName", way.UserName.ValueAsText()},
                                {"Visible", way.Visible.ToString()},
                                {
                                    "Tags",
                                    $"'{string.Join(", ", way.Tags.Select(t => $"\"{t.Key.TextEscape(true)}\" => \"{t.Value.TextEscape(true)}\""))}'"
                                },
                                {
                                    "Nodes",
                                    $"{{{string.Join(",", way.Nodes.Select(t => $"{t.ToString()}"))}}}"
                                }
                            };

                            using (var command = new NpgsqlCommand(
                                $"INSERT INTO Way ({string.Join(", ", wayDictionary.Keys)}) VALUES ({string.Join(", ", wayDictionary.Values)})",
                                connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            break;
                        case Relation relation:
                            var relationDictionary = new Dictionary<string, string>
                            {
                                {"Id", relation.Id.ToString()},
                                {"Version", relation.Version.ToString()},
                                {"ChangeSetId", relation.ChangeSetId.ToString()},
                                {"TimeStamp", relation.TimeStamp.ValueAsText()},
                                {"UserId", relation.UserId.ToString()},
                                {"UserName", relation.UserName.ValueAsText()},
                                {"Visible", relation.Visible.ToString()},
                                {
                                    "Tags",
                                    $"'{string.Join(", ", relation.Tags.Select(t => $"\"{t.Key.TextEscape(true)}\" => \"{t.Value.TextEscape(true)}\""))}'"
                                },
                                {
                                    "Members",
                                    $"{{{string.Join(",", relation.Members.Select(t => $"({((int) t.Type).ToString()},{t.Id.ToString()},\"{t.Role.TextEscape(true)}\")"))}}}"
                                }
                            };

                            using (var command = new NpgsqlCommand(
                                $"INSERT INTO Relation ({string.Join(", ", relationDictionary.Keys)}) VALUES ({string.Join(", ", relationDictionary.Values)})",
                                connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (count++ % 1000 == 0)
                        await _progressHub.Progress(100f * uploadStream.Position / uploadStream.Length, session);
                }
            }

            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Mapper.Sql.Osm.CreateIndeces.sql"))
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            using (var command = new NpgsqlCommand(await sr.ReadToEndAsync(), connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public async Task Update(Stream uploadStream, NpgsqlConnection connection, string session)
        {
            var source = new PBFOsmStreamSource(uploadStream);
            foreach (var element in source)
                switch (element)
                {
                    case Node node:
                        Console.WriteLine(JsonConvert.SerializeObject(node));
                        break;
                    case Way way:
                        Console.WriteLine(JsonConvert.SerializeObject(way));
                        break;
                    case Relation relation:
                        Console.WriteLine(JsonConvert.SerializeObject(relation));
                        break;
                    default:
                        throw new NotImplementedException();
                }
        }
    }
}