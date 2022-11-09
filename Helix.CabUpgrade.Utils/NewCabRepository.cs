using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.CabUpgrade.Utils
{
    public class NewCabRepository
    {
        Settings _settings { get; set; }

        public NewCabRepository(Settings options)
        {
            _settings = options;
        }

        public IEnumerable<CabInfo> GetNewCabs()
        {
            return _settings.CabMapping.Select(a => a.Value);
        }
    }
}
