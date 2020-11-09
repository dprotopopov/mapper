using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper.Extensions;
using Mapper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Mapper.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class FiasController : ControllerBase
    {
        private readonly List<string> _addrobList = new List<string>();
        private readonly string _rootAddrobSql;
        private readonly string _rootHouseSql;
        private readonly string _rootRoomSql;
        private readonly string _rootSteadSql;

        private readonly string _childrenAddrobSql;
        private readonly string _childrenHouseSql;
        private readonly string _childrenRoomSql;
        private readonly string _childrenSteadSql;
        private readonly List<string> _houseList = new List<string>();
        private readonly string _parentAddrobSql;
        private readonly string _parentHouseSql;
        private readonly string _parentRoomSql;
        private readonly string _parentSteadSql;
        private readonly List<string> _roomList = new List<string>();
        private readonly List<string> _steadList = new List<string>();

        protected readonly IConfiguration Configuration;

        public FiasController(IConfiguration configuration)
        {
            Configuration = configuration;

            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                _addrobList.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'addrob\d+'",
                    connection);
                _houseList.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'house\d+'",
                    connection);
                _roomList.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'room\d+'",
                    connection);
                _steadList.Fill(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_name similar to 'stead\d+'",
                    connection);

                _parentRoomSql = string.Join(" UNION ",
                    _roomList.Select(x =>
                        $"SELECT houseguid,flatnumber,roomnumber FROM {x} WHERE roomguid=@p"));
                _parentHouseSql = string.Join(" UNION ",
                    _houseList.Select(x =>
                        $"SELECT aoguid,housenum,buildnum,strucnum FROM {x} WHERE houseguid=@p"));
                _parentSteadSql = string.Join(" UNION ",
                    _steadList.Select(x =>
                        $"SELECT parentguid,number FROM {x} WHERE steadguid=@p"));
                _parentAddrobSql = string.Join(" UNION ",
                    _addrobList.Select(x =>
                        $"SELECT parentguid,offname,formalname,shortname,socrbase.socrname FROM {x} JOIN socrbase ON {x}.shortname=socrbase.scname AND cast({x}.aolevel as text)=socrbase.level WHERE aoguid=@p AND actstatus=1"));


                _childrenRoomSql = string.Join(" UNION ",
                    _roomList.Select(x =>
                        $"SELECT roomguid,flatnumber,roomnumber FROM {x} WHERE houseguid=@p"));
                _childrenHouseSql = string.Join(" UNION ",
                    _houseList.Select(x =>
                        $"SELECT houseguid,housenum,buildnum,strucnum FROM {x} WHERE aoguid=@p"));
                _childrenSteadSql = string.Join(" UNION ",
                    _steadList.Select(x =>
                        $"SELECT steadguid,number FROM {x} WHERE parentguid=@p"));
                _childrenAddrobSql = string.Join(" UNION ",
                    _addrobList.Select(x =>
                        $"SELECT aoguid,offname,formalname,shortname,socrbase.socrname FROM {x} JOIN socrbase ON {x}.shortname=socrbase.scname AND cast({x}.aolevel as text)=socrbase.level WHERE parentguid=@p AND actstatus=1"));

                _rootRoomSql = string.Join(" UNION ",
                    _roomList.Select(x =>
                        $"SELECT roomguid,flatnumber,roomnumber FROM {x} WHERE houseguid IS NULL"));
                _rootHouseSql = string.Join(" UNION ",
                    _houseList.Select(x =>
                        $"SELECT houseguid,housenum,buildnum,strucnum FROM {x} WHERE aoguid IS NULL"));
                _rootSteadSql = string.Join(" UNION ",
                    _steadList.Select(x =>
                        $"SELECT steadguid,number FROM {x} WHERE parentguid IS NULL"));
                _rootAddrobSql = string.Join(" UNION ",
                    _addrobList.Select(x =>
                        $"SELECT aoguid,offname,formalname,shortname,socrbase.socrname FROM {x} JOIN socrbase ON {x}.shortname=socrbase.scname AND cast({x}.aolevel as text)=socrbase.level WHERE parentguid IS NULL AND actstatus=1"));
            }
        }

        [HttpGet("parent/{guid}")]
        [ProducesResponseType(200, Type = typeof(FiasResult))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetParent(string guid)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                var result = new FiasResult();

                using (var command = new NpgsqlCommand(_parentRoomSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.room.Add(new Room
                            {
                                guid = Guid.Parse(guid),
                                flatnumber = reader.GetString(1),
                                roomnumber = reader.GetString(2)
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
                            result.house.Add(new House
                            {
                                guid = Guid.Parse(guid),
                                housenum = reader.GetString(1),
                                buildnum = reader.GetString(2),
                                strucnum = reader.GetString(3)
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
                            result.stead.Add(new Stead
                            {
                                guid = Guid.Parse(guid),
                                number = reader.GetString(1)
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
                                result.address.Add(new Address
                                {
                                    guid = Guid.Parse(guid),
                                    offname = reader.GetString(1),
                                    formalname = reader.GetString(2),
                                    shortname = reader.GetString(3),
                                    socrname = reader.GetString(4)
                                });
                                guid = reader.GetString(0);
                            }
                        }
                    }
                }

                result.address.Reverse();

                return Ok(result);
            }
        }

        [HttpGet("children/{guid}")]
        [ProducesResponseType(200, Type = typeof(FiasResult))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetChildren(string guid)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                var result = new FiasResult();

                using (var command = new NpgsqlCommand(_childrenRoomSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.room.Add(new Room
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                flatnumber = reader.GetString(1),
                                roomnumber = reader.GetString(2)
                            });
                    }
                }


                using (var command = new NpgsqlCommand(_childrenHouseSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.house.Add(new House
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                housenum = reader.GetString(1),
                                buildnum = reader.GetString(2),
                                strucnum = reader.GetString(3)
                            });
                    }
                }

                using (var command = new NpgsqlCommand(_childrenSteadSql, connection))
                {
                    command.Parameters.AddWithValue("p", guid);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.stead.Add(new Stead
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                number = reader.GetString(1)
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
                            result.address.Add(new Address
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                offname = reader.GetString(1),
                                formalname = reader.GetString(2),
                                shortname = reader.GetString(3),
                                socrname = reader.GetString(4)
                            });
                    }
                }


                return Ok(result);
            }
        }


        [HttpGet("roots")]
        [ProducesResponseType(200, Type = typeof(FiasResult))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoots()
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();

                var result = new FiasResult();

                using (var command = new NpgsqlCommand(_rootRoomSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.room.Add(new Room
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                flatnumber = reader.GetString(1),
                                roomnumber = reader.GetString(2)
                            });
                    }
                }


                using (var command = new NpgsqlCommand(_rootHouseSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.house.Add(new House
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                housenum = reader.GetString(1),
                                buildnum = reader.GetString(2),
                                strucnum = reader.GetString(3)
                            });
                    }
                }

                using (var command = new NpgsqlCommand(_rootSteadSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.stead.Add(new Stead
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                number = reader.GetString(1)
                            });
                        }
                    }
                }

                using (var command = new NpgsqlCommand(_rootAddrobSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.address.Add(new Address
                            {
                                guid = Guid.Parse(reader.GetString(0)),
                                offname = reader.GetString(1),
                                formalname = reader.GetString(2),
                                shortname = reader.GetString(3),
                                socrname = reader.GetString(4)
                            });
                    }
                }


                return Ok(result);
            }
        }


        protected string GetConnectionString()
        {
            return Configuration.GetConnectionString("FiasConnection");
        }
    }
}