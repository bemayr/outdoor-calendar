using Npoi.Mapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventlistToICal
{
    internal class Event
    {
        [Column(0)]
        public string Title { get; set; }

        [Column(1)]
        public string Link { get; set; }

        [Column(2)]
        public string Kind { get; set; }

        [Column(3)]
        public string Type { get; set; }

        [Column(4)]
        public string Country { get; set; }

        [Column(5)]
        public DateTime From { get; set; }

        [Column(6)]
        public DateTime To { get; set; }
    }
}
