using System;

namespace PerfWcf.Console
{
    [Serializable]
    public class FakeRmqMessage
    {
        public Guid Guid { get; set; }
    }
}
