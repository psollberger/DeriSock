namespace DeriSock.DevTools.UseCases;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class CreateEnumOverrides : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var map = await ApiDocUtils.ReadEnumMapAsync(Consts.EnumMapPath, cancellationToken).ConfigureAwait(false);

    if (map is not null)
      await ApiDocUtils.WriteEnumOverridesFromMapAsync(map, Consts.EnumOverridesPath, cancellationToken).ConfigureAwait(false);
  }
}
