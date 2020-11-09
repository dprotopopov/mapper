using System;
using System.Collections.Generic;

namespace Mapper.Models
{
    public class FiasResult
    {
        public List<Address> address { get; set; } = new List<Address>();
        public List<House> house { get; set; } = new List<House>();
        public List<Room> room { get; set; } = new List<Room>();
        public List<Stead> stead { get; set; } = new List<Stead>();
    }

    public abstract class Element
    {
        public Guid guid { get; set; }
    }

    public class Address : Element
    {
        public string offname { get; set; }
        public string formalname { get; set; }
        public string shortname { get; set; }
        public string socrname { get; set; }
    }

    public class House : Element
    {
        public string housenum { get; set; }
        public string buildnum { get; set; }
        public string strucnum { get; set; }
    }

    public class Room : Element
    {
        public string flatnumber { get; set; }
        public string roomnumber { get; set; }
    }

    public class Stead : Element
    {
        public string number { get; set; }
    }
}