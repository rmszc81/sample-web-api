using System.Runtime.Serialization;

namespace SampleModel
{
    [DataContract]
    public class ValueItem
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string Name { get; set; }
        [DataMember(Order = 3)]
        public bool IsComplete { get; set; }
    }
}
