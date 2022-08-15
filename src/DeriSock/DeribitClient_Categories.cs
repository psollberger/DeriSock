namespace DeriSock;

using DeriSock.Api;

public partial class DeribitClient : ICategoriesApi
{
  /// <inheritdoc cref="IPublicApi" />
  public IPublicApi Public => this;

  /// <inheritdoc cref="IPrivateApi" />
  public IPrivateApi Private => this;

  /// <inheritdoc cref="IAuthenticationApi" />
  public IAuthenticationApi Authentication => this;

  /// <inheritdoc cref="ISessionManagementApi" />
  public ISessionManagementApi SessionManagement => this;

  /// <inheritdoc cref="ISessionManagementApi" />
  public ISupportingApi Supporting => this;

  /// <inheritdoc cref="IAccountManagementApi" />
  public IAccountManagementApi AccountManagement => this;

  /// <inheritdoc cref="IBlockTradeApi" />
  public IBlockTradeApi BlockTrade => this;

  /// <inheritdoc cref="ITradingApi" />
  public ITradingApi Trading => this;

  /// <inheritdoc cref="IMarketDataApi" />
  public IMarketDataApi MarketData => this;

  /// <inheritdoc cref="IWalletApi" />
  public IWalletApi Wallet => this;
}
