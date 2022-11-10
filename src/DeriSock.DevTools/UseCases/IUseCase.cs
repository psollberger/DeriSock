namespace DeriSock.DevTools;

using System.Threading;
using System.Threading.Tasks;

internal interface IUseCase
{
  Task Run(CancellationToken cancellationToken);
}
