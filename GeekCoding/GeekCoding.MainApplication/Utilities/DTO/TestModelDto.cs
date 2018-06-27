using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Utilities.DTO
{
    [Serializable]
    public class TestModelDto
    {
        public string MemoryUsed { get; set; }
        public string ExecutionTime { get; set; }
        public string Message { get; set; }
        public int Point { get; set; }
    }
}
