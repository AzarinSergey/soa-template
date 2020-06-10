using System;
using System.Collections.Generic;
using System.Threading;

namespace Svc.Contract.Service.Models
{
    public class ComplexModel
    {
        public List<byte[]> Files { get; set; }

        public string Encoding { get; set; }

        public IReadOnlyDictionary<DateTimeKind, DateTime> Dates { get; set; }

        public EventResetMode? NullableEnumNull { get; set; }

        public EventResetMode NullableEnumValue { get; set; }
    }
}