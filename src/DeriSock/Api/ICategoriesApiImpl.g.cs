// --------------------------------------------------------------------------
// <auto-generated>
//      This code was generated by a tool.
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
// </auto-generated>
// --------------------------------------------------------------------------
#pragma warning disable CS1591
#nullable enable
namespace DeriSock
{
  using DeriSock.Api;
  
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class DeribitClient : ICategoriesApi
  {
    private IPublicApi? _public;
    private IPrivateApi? _private;
    private IAuthenticationApi? _authentication;
    private ISessionManagementApi? _sessionManagement;
    private ISupportingApi? _supporting;
    private IMarketDataApi? _marketData;
    private ITradingApi? _trading;
    private IComboBooksApi? _comboBooks;
    private IBlockTradeApi? _blockTrade;
    private IWalletApi? _wallet;
    private IAccountManagementApi? _accountManagement;
    private ISubscriptionsApi? _subscriptions;
    /// <inheritdoc cref="IPublicApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IPublicApi Public
    {
      get
      {
        _public ??= new PublicApiImpl(this);
        return _public;
      }
    }
    /// <inheritdoc cref="IPrivateApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IPrivateApi Private
    {
      get
      {
        _private ??= new PrivateApiImpl(this);
        return _private;
      }
    }
    /// <inheritdoc cref="IAuthenticationApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IAuthenticationApi Authentication
    {
      get
      {
        _authentication ??= new AuthenticationApiImpl(this);
        return _authentication;
      }
    }
    /// <inheritdoc cref="ISessionManagementApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public ISessionManagementApi SessionManagement
    {
      get
      {
        _sessionManagement ??= new SessionManagementApiImpl(this);
        return _sessionManagement;
      }
    }
    /// <inheritdoc cref="ISupportingApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public ISupportingApi Supporting
    {
      get
      {
        _supporting ??= new SupportingApiImpl(this);
        return _supporting;
      }
    }
    /// <inheritdoc cref="IMarketDataApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IMarketDataApi MarketData
    {
      get
      {
        _marketData ??= new MarketDataApiImpl(this);
        return _marketData;
      }
    }
    /// <inheritdoc cref="ITradingApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public ITradingApi Trading
    {
      get
      {
        _trading ??= new TradingApiImpl(this);
        return _trading;
      }
    }
    /// <inheritdoc cref="IComboBooksApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IComboBooksApi ComboBooks
    {
      get
      {
        _comboBooks ??= new ComboBooksApiImpl(this);
        return _comboBooks;
      }
    }
    /// <inheritdoc cref="IBlockTradeApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IBlockTradeApi BlockTrade
    {
      get
      {
        _blockTrade ??= new BlockTradeApiImpl(this);
        return _blockTrade;
      }
    }
    /// <inheritdoc cref="IWalletApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IWalletApi Wallet
    {
      get
      {
        _wallet ??= new WalletApiImpl(this);
        return _wallet;
      }
    }
    /// <inheritdoc cref="IAccountManagementApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public IAccountManagementApi AccountManagement
    {
      get
      {
        _accountManagement ??= new AccountManagementApiImpl(this);
        return _accountManagement;
      }
    }
    /// <inheritdoc cref="ISubscriptionsApi" />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    public ISubscriptionsApi Subscriptions
    {
      get
      {
        _subscriptions ??= new SubscriptionsApiImpl(this);
        return _subscriptions;
      }
    }
  }
}
