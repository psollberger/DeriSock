namespace DeriSock.DevTools;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

internal interface IDevToolsConsole
{
  Task Run(IEnumerable<string> args, CancellationToken cancellationToken = default);
}
