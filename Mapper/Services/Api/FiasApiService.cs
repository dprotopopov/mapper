using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper.Extensions;
using Mapper.Models.Fias;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Mapper.Services.Api
{
    public class FiasApiService
    {
        private readonly IConfiguration Configuration;
        private readonly string _childrenAddrobSql;
        private readonly string _childrenHouseSql;
        private readonly string _childrenRoomSql;
        private readonly string _childrenSteadSql;
        private readonly List<string> _listAddrob = new List<string>();
        private readonly List<string> _listHouse = new List<string>();
        private readonly List<string> _listRoom = new List<string>();
        private readonly List<string> _listStead = new List<string>();
        private readonly string _parentAddrobSql;
        private readonly string _parentHouseSql;
        private readonly string _parentRoomSql;
        private readonly string _parentSteadSql;
        private readonly string _rootAddrobSql;
        private readonly string _rootHouseSql;
        private readonly string _rootRoomSql;
        private readonly string _rootSteadSql;

        public FiasApiService(IConfiguration configuration)
        {
            Configuration = configuration;

            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                _listAddrob.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'addrob\d+'",
                    connection);
                _listHouse.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'house\d+'",
                    connection);
                _listRoom.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'room\d+'",
                    connection);
                _listStead.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'stead\d+'",
                    connection);

                _parentRoomSql = string.Join(" UNION ",
                    _listRoom.Select(x =>
                        $"SELECT houseguid,flatnumber,roomnumber FROM {x} WHERE roomguid=@p"));
                _parentHouseSql = string.Join(" UNION ",
                    _listHouse.Select(x =>
                        $"SELECT aoguid,housenum,buildnum,strucnum FROM {x} WHERE houseguid=@p"));
                _parentSteadSql = string.Join(" UNION ",
                    _listStead.Select(x =>
                        $"SELECT parentguid,number FROM {x} WHERE steadguid=@p"));
                _parentAddrobSql = string.Join(" UNION ",
                    _listAddrob.Select(x =>
                        $"SELECT parentguid,offname,formalname,shortname,socrbase.socrname FROM {x} JOIN socrbase ON {x}.shortname=socrbase.scname AND {x}.aolevel=socrbase.level WHERE aoguid=@p AND actstatus=1"));


                _childrenRoomSql = string.Join(" UNION ",
                    _listRoom.Select(x =>
                        $"SELECT roomguid,flatnumber,roomnumber FROM {x} WHERE houseguid=@p"));
                _childrenHouseSql = string.Join(" UNION ",
                    _listHouse.Select(x =>
                        $"SELECT houseguid,housenum,buildnum,strucnum FROM {x} WHERE aoguid=@p"));
                _childrenSteadSql = string.Join(" UNION ",
                    _listStead.Select(x =>
                        $"SELECT steadguid,number FROM {x} WHERE parentguid=@p"));
                _childrenAddrobSql = string.Join(" UNION ",
                    _listAddrob.Select(x =>
                        $"SELECT aoguid,offname,formalname,shortname,socrbase.socrname FROM {x} JOIN socrbase ON {x}.shortname=socrbase.scname AND {x}.aolevel=socrbase.level WHERE parentguid=@p AND actstatus=1"));

                _rootRoomSql = string.Join(" UNION ",
                    _listRoom.Select(x =>
                        $"SELECT roomguid,flatnumber,roomnumber FROM {x} WHERE houseguid IS NULL"));
                _rootHouseSql = string.Join(" UNION ",
                    _listHouse.Select(x =>
                        $"SELECT houseguid,housenum,buildnum,strucnum FROM {x} WHERE aoguid IS NULL"));
                _rootSteadSql = string.Join(" UNION ",
                    _listStead.Select(x =>
                        $"SELECT steadguid,number FROM {x} WHERE parentguid IS NULL"));
                _rootAddrobSql = string.Join(" UNION ",
                    _listAddrob.Select(x =>
                        $"SELECT aoguid,offname,formalname,shortname,socrbase.socrname FROM {x} JOIN socrbase ON {x}.shortname=socrbase.scname AND {x}.aolevel=socrbase.level WHERE parentguid IS NULL AND actstatus=1"));
            }

        }
        public async Task<List<Element>> GetDetails(string guid, bool formal = false, bool socr = false)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                var result = new List<Element>();

                using (var command = new NpgsqlCommand(_parentRoomSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var flatnumber = reader.GetString(1);
                            var roomnumber = reader.GetString(2);
                            if (!string.IsNullOrEmpty(roomnumber)) list.Add($"комн. {roomnumber}");
                            if (!string.IsNullOrEmpty(flatnumber)) list.Add($"кв. {flatnumber}");
                            list.Reverse();
                            result.Add(new Room
                            {
                                guid = Guid.Parse(guid),
                                flatnumber = flatnumber,
                                roomnumber = roomnumber,
                                title = string.Join(", ", list)
                            });
                            guid = reader.GetString(0);
                        }
                    }
                }


                using (var command = new NpgsqlCommand(_parentHouseSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var housenum = reader.GetString(1);
                            var buildnum = reader.GetString(2);
                            var strucnum = reader.GetString(3);
                            if (!string.IsNullOrEmpty(strucnum)) list.Add($"стр. {strucnum}");
                            if (!string.IsNullOrEmpty(buildnum)) list.Add($"корп. {buildnum}");
                            if (!string.IsNullOrEmpty(housenum)) list.Add($"д. {housenum}");
                            list.Reverse();
                            result.Add(new House
                            {
                                guid = Guid.Parse(guid),
                                housenum = housenum,
                                buildnum = buildnum,
                                strucnum = strucnum,
                                title = string.Join(", ", list)
                            });
                            guid = reader.GetString(0);
                        }
                    }
                }

                using (var command = new NpgsqlCommand(_parentSteadSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var number = reader.GetString(1);
                            if (!string.IsNullOrEmpty(number)) list.Add($"уч. {number}");
                            result.Add(new Stead
                            {
                                guid = Guid.Parse(guid),
                                number = number,
                                title = string.Join(", ", list)
                            });
                            guid = reader.GetString(0);
                        }
                    }
                }

                for (var run = true; run;)
                {
                    run = false;

                    using (var command = new NpgsqlCommand(_parentAddrobSql, connection))
                    {
                        command.Parameters.AddWithValue("p", guid);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                run = true;
                                var offname = reader.GetString(1);
                                var formalname = reader.GetString(2);
                                var shortname = reader.GetString(3);
                                var socrname = reader.GetString(4);
                                var title = $"{(socr ? socrname : shortname)} {(formal ? formalname : offname)}";
                                result.Add(new Address
                                {
                                    guid = Guid.Parse(guid),
                                    offname = offname,
                                    formalname = formalname,
                                    shortname = shortname,
                                    socrname = socrname,
                                    title = title
                                });
                                guid = reader.GetString(0);
                            }
                        }
                    }
                }

                result.Reverse();

                return result;
            }
        }
        public async Task<List<Element>> GetChildren(string guid, bool formal = false, bool socr = false)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                var result = new List<Element>();

                using (var command = new NpgsqlCommand(_childrenRoomSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var flatnumber = reader.GetString(1);
                            var roomnumber = reader.GetString(2);
                            if (!string.IsNullOrEmpty(roomnumber)) list.Add($"комн. {roomnumber}");
                            if (!string.IsNullOrEmpty(flatnumber)) list.Add($"кв. {flatnumber}");
                            list.Reverse();
                            result.Add(new Room
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                flatnumber = flatnumber,
                                roomnumber = roomnumber,
                                title = string.Join(", ", list)
                            });
                        }
                    }
                }


                using (var command = new NpgsqlCommand(_childrenHouseSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var housenum = reader.GetString(1);
                            var buildnum = reader.GetString(2);
                            var strucnum = reader.GetString(3);
                            if (!string.IsNullOrEmpty(strucnum)) list.Add($"стр. {strucnum}");
                            if (!string.IsNullOrEmpty(buildnum)) list.Add($"корп. {buildnum}");
                            if (!string.IsNullOrEmpty(housenum)) list.Add($"д. {housenum}");
                            list.Reverse();
                            result.Add(new House
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                housenum = housenum,
                                buildnum = buildnum,
                                strucnum = strucnum,
                                title = string.Join(", ", list)
                            });
                        }
                    }
                }

                using (var command = new NpgsqlCommand(_childrenSteadSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var number = reader.GetString(1);
                            if (!string.IsNullOrEmpty(number)) list.Add($"уч. {number}");
                            result.Add(new Stead
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                number = number,
                                title = string.Join(", ", list)
                            });
                        }
                    }
                }


                using (var command = new NpgsqlCommand(_childrenAddrobSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var offname = reader.GetString(1);
                            var formalname = reader.GetString(2);
                            var shortname = reader.GetString(3);
                            var socrname = reader.GetString(4);
                            var title = $"{(socr ? socrname : shortname)} {(formal ? formalname : offname)}";
                            result.Add(new Address
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                offname = offname,
                                formalname = formalname,
                                shortname = shortname,
                                socrname = socrname,
                                title = title
                            });
                        }
                    }
                }

                return result;
            }
        }
        public async Task<List<Element>> GetRoots(bool formal = false, bool socr = false)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                var result = new List<Element>();

                using (var command = new NpgsqlCommand(_rootRoomSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var flatnumber = reader.GetString(1);
                            var roomnumber = reader.GetString(2);
                            if (!string.IsNullOrEmpty(roomnumber)) list.Add($"комн. {roomnumber}");
                            if (!string.IsNullOrEmpty(flatnumber)) list.Add($"кв. {flatnumber}");
                            list.Reverse();
                            result.Add(new Room
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                flatnumber = flatnumber,
                                roomnumber = roomnumber,
                                title = string.Join(", ", list)
                            });
                        }
                    }
                }


                using (var command = new NpgsqlCommand(_rootHouseSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var housenum = reader.GetString(1);
                            var buildnum = reader.GetString(2);
                            var strucnum = reader.GetString(3);
                            if (!string.IsNullOrEmpty(strucnum)) list.Add($"стр. {strucnum}");
                            if (!string.IsNullOrEmpty(buildnum)) list.Add($"корп. {buildnum}");
                            if (!string.IsNullOrEmpty(housenum)) list.Add($"д. {housenum}");
                            list.Reverse();
                            result.Add(new House
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                housenum = housenum,
                                buildnum = buildnum,
                                strucnum = strucnum,
                                title = string.Join(", ", list)
                            });
                        }
                    }
                }

                using (var command = new NpgsqlCommand(_rootSteadSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var list = new List<string>();
                            var number = reader.GetString(1);
                            if (!string.IsNullOrEmpty(number)) list.Add($"уч. {number}");
                            result.Add(new Stead
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                number = number,
                                title = string.Join(", ", list)
                            });
                        }
                    }
                }


                using (var command = new NpgsqlCommand(_rootAddrobSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var offname = reader.GetString(1);
                            var formalname = reader.GetString(2);
                            var shortname = reader.GetString(3);
                            var socrname = reader.GetString(4);
                            var title = $"{(socr ? socrname : shortname)} {(formal ? formalname : offname)}";
                            result.Add(new Address
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                offname = offname,
                                formalname = formalname,
                                shortname = shortname,
                                socrname = socrname,
                                title = title
                            });
                        }
                    }
                }

                return result;
            }
        }

        private string GetConnectionString()
        {
            return Configuration.GetConnectionString("FiasConnection");
        }
    }
}