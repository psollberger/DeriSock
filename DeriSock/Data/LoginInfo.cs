namespace DeriSock.Data
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock;

  public class LoginInfo
    {
        private readonly DeribitApiV2 _api;

        private readonly Timer _refreshTimer;

        public LoginInfo(DeribitApiV2 api)
        {
            _api = api;
            _refreshTimer = new Timer(
                                      state =>
                                      {
                                          Task.WaitAll(Refresh());
                                      }, null, Timeout.Infinite, Timeout.Infinite);
            Task.WaitAll(Refresh());
        }

        public async Task Refresh()
        {
            var loginInfo = await _api.LoginAsync();
            _api.Socket.AccessToken = loginInfo.access_token;
            _refreshTimer.Change(TimeSpan.FromSeconds(loginInfo.expires_in - 30d), Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _refreshTimer?.Dispose();
        }
    }
}
