using System;

namespace Mapper.Models.Fias
{
    public abstract class Element
    {
        public Guid guid { get; set; }
        public string title { get; set; }
    }
}