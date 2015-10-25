using System.Collections.Generic;

namespace Gitter.DataObjects.Concrete
{
    public class Version
    {
        public string Name { get; set; }
        public IEnumerable<string> Features { get; set; }
    }
}
