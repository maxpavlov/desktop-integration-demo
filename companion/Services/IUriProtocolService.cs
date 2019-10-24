using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace companion.Services
{
    interface IUriProtocolService
    {
        bool RegisterUriScheme();

        string GetUriParams(string[] args);

        string GetCurrentRegistryCommand();
    }
}
