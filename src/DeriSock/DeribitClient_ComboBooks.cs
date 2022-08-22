namespace DeriSock;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<Combo>> InternalPublicGetComboDetails(PublicGetComboDetailsRequest args, CancellationToken cancellationToken)
    => await Send("public/get_combo_details", args, new ObjectJsonConverter<Combo>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string[]>> InternalPublicGetComboIds(PublicGetComboIdsRequest args, CancellationToken cancellationToken)
    => await Send("public/get_combo_ids", args, new ObjectJsonConverter<string[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Combo[]>> InternalPublicGetCombos(PublicGetCombosRequest args, CancellationToken cancellationToken)
    => await Send("public/get_combos", args, new ObjectJsonConverter<Combo[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Combo>> InternalPrivateCreateCombo(PrivateCreateComboRequest args, CancellationToken cancellationToken)
    => await Send("private/create_combo", args, new ObjectJsonConverter<Combo>(), cancellationToken).ConfigureAwait(false);
}
