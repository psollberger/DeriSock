// ReSharper disable StringLiteralTypo

namespace DeriSock.Constants
{
  public sealed class ErrorCode
  {
    public static ErrorCode ApiNotEnabled = new ErrorCode(9999, "api_not_enabled", "User didn't enable API for the Account.");
    public static ErrorCode AuthorizationRequired = new ErrorCode(10000, "authorization_required", "Authorization issue, invalid or absent signature etc.");
    public static ErrorCode GeneralError = new ErrorCode(10001, "error", "Some general failure, no public information available.");
    public static ErrorCode QuantityTooLow = new ErrorCode(10002, "qty_too_low", "Order quantity is too low.");
    public static ErrorCode OrderOverlap = new ErrorCode(10003, "order_overlap", "Rejection, order overlap is found and self-trading is not enabled.");
    public static ErrorCode OrderNotFound = new ErrorCode(10004, "order_not_found", "Attempt to operate with order that can't be found by specified id.");

    public static ErrorCode PriceTooLowLimit =
      new ErrorCode(10005, "price_too_low<Limit>", "Price is too low, <Limit> defines current limit for the operation.");

    public static ErrorCode PriceTooLowForIndex = new ErrorCode(10006, "price_too_low4idx <Limit>",
      "Price is too low for current index, <Limit> defines current bottom limit for the operation.");

    public static ErrorCode PriceTooHighLimit =
      new ErrorCode(10007, "price_too_high <Limit>", "Price is too high, <Limit> defines current up limit for the operation.");

    public static ErrorCode PriceTooHigh4Index = new ErrorCode(10008, "price_too_high4idx <Limit>",
      "Price is too high for current index, <Limit> defines current up limit for the operation.");

    public static ErrorCode NotEnoughFunds = new ErrorCode(10009, "not_enough_funds", "Account has not enough funds for the operation.");
    public static ErrorCode AlreadyClosed = new ErrorCode(10010, "already_closed", "Attempt of doing something with closed order.");
    public static ErrorCode PriceNotAllowed = new ErrorCode(10011, "price_not_allowed", "This price is not allowed for some reason.");
    public static ErrorCode BookClosed = new ErrorCode(10012, "book_closed", "Operation for instrument which order book had been closed.");

    public static ErrorCode PmeMaxTotalOpenOrdersLimit = new ErrorCode(10013, "pme_max_total_open_orders <Limit>",
      "Total limit of open orders has been exceeded, it is applicable for PME users.");

    public static ErrorCode PmeMaxFutureOpenOrdersLimit = new ErrorCode(10014, "pme_max_future_open_orders <Limit>",
      "Limit of count of futures' open orders has been exceeded, it is applicable for PME users.");

    public static ErrorCode PmeMaxOptionOpenOrdersLimit = new ErrorCode(10015, "pme_max_option_open_orders <Limit>",
      "Limit of count of options' open orders has been exceeded, it is applicable for PME users.");

    public static ErrorCode PmeMaxFutureOpenOrdersSizeLimit = new ErrorCode(10016, "pme_max_future_open_orders_size <Limit>",
      "Limit of size for futures has been exceeded, it is applicable for PME users.");

    public static ErrorCode PmeMaxOptionOpenOrdersSizeLimit = new ErrorCode(10017, "pme_max_option_open_orders_size <Limit>",
      "Limit of size for options has been exceeded, it is applicable for PME users.");

    public static ErrorCode NonPmeMaxFuturePositionSizeLimit = new ErrorCode(10018, "non_pme_max_future_position_size <Limit>",
      "Limit of size for futures has been exceeded, it is applicable for non-PME users.");

    public static ErrorCode LockedByAdmin = new ErrorCode(10019, "locked_by_admin", "Trading is temporary locked by admin.");
    public static ErrorCode InvalidOrUnsupportedInstrument = new ErrorCode(10020, "invalid_or_unsupported_instrument", "Instrument name is not valid.");
    public static ErrorCode InvalidAmount = new ErrorCode(10021, "invalid_amount", "Amount is not valid.");
    public static ErrorCode InvalidQuantity = new ErrorCode(10022, "invalid_quantity", "quantity was not recognized as a valid number (for API v1).");
    public static ErrorCode InvalidPrice = new ErrorCode(10023, "invalid_price", "price was not recognized as a valid number.");
    public static ErrorCode InvalidMaxShow = new ErrorCode(10024, "invalid_max_show", "max_show parameter was not recognized as a valid number.");
    public static ErrorCode InvalidOrderId = new ErrorCode(10025, "invalid_order_id", "Order id is missing or its format was not recognized as valid.");
    public static ErrorCode PricePrecisionExceeded = new ErrorCode(10026, "price_precision_exceeded", "Extra precision of the price is not supported.");

    public static ErrorCode NonIntegerContractAmount =
      new ErrorCode(10027, "non_integer_contract_amount", "Futures contract amount was not recognized as integer.");

    public static ErrorCode TooManyRequests = new ErrorCode(10028, "too_many_requests", "Allowed request rate has been exceeded.");
    public static ErrorCode NotOwnerOfOrder = new ErrorCode(10029, "not_owner_of_order", "Attempt to operate with not own order.");
    public static ErrorCode MustBeWebsocketRequest = new ErrorCode(10030, "must_be_websocket_request", "REST request where Websocket is expected.");
    public static ErrorCode InvalidArgsForInstrument = new ErrorCode(10031, "invalid_args_for_instrument", "Some of arguments are not recognized as valid.");
    public static ErrorCode WholeCostTooLow = new ErrorCode(10032, "whole_cost_too_low", "Total cost is too low.");
    public static ErrorCode NotImplemented = new ErrorCode(10033, "not_implemented", "Method is not implemented yet.");
    public static ErrorCode StopPriceTooHigh = new ErrorCode(10034, "stop_price_too_high", "Stop price is too high.");
    public static ErrorCode StopPriceTooLow = new ErrorCode(10035, "stop_price_too_low", "Stop price is too low.");
    public static ErrorCode InvalidMaxShowAmount = new ErrorCode(10036, "invalid_max_show_amount", "Max Show Amount is not valid.");
    public static ErrorCode Retry = new ErrorCode(10040, "retry", "Request can't be processed right now and should be retried.");

    public static ErrorCode SettlementInProgress = new ErrorCode(10041, "settlement_in_progress",
      "Settlement is in progress. Every day at settlement time for several seconds, the system calculates user profits and updates balances. That time trading is paused for several seconds till the calculation is completed.");

    public static ErrorCode PriceWrongTick = new ErrorCode(10043, "price_wrong_tick", "Price has to be rounded to a certain tick size.");
    public static ErrorCode StopPriceWrongTick = new ErrorCode(10044, "stop_price_wrong_tick", "Stop Price has to be rounded to a certain tick size.");
    public static ErrorCode CannotCancelLiquidationOrder = new ErrorCode(10045, "can_not_cancel_liquidation_order", "Liquidation order can't be canceled.");
    public static ErrorCode CannotEditLiquidationOrder = new ErrorCode(10046, "can_not_edit_liquidation_order", "Liquidation order can't be edited.");

    public static ErrorCode MatchingEngineQueueFull =
      new ErrorCode(10047, "matching_engine_queue_full", "Reached limit of pending Matching Engine requests for user.");

    public static ErrorCode NotOnThisServer = new ErrorCode(10048, "not_on_this_server", "The requested operation is not available on this server.");

    public static ErrorCode CancelOnDisconnectFailed =
      new ErrorCode(10049, "cancel_on_disconnect_failed", "Enabling Cancel On Disconnect for the connection failed.");

    public static ErrorCode AlreadyFilled = new ErrorCode(11008, "already_filled", "This request is not allowed in regards to the filled order.");
    public static ErrorCode InvalidArguments = new ErrorCode(11029, "invalid_arguments", "Some invalid input has been detected.");

    public static ErrorCode OtherReject = new ErrorCode(11030, "other_reject <Reason>",
      "Some rejects which are not considered as very often, more info may be specified in <Reason>.");

    public static ErrorCode OtherError = new ErrorCode(11031, "other_error <Error>",
      "Some errors which are not considered as very often, more info may be specified in <Error>.");

    public static ErrorCode NoMoreStops = new ErrorCode(11035, "no_more_stops <Limit>", "Allowed amount of stop orders has been exceeded.");

    public static ErrorCode InvalidStopPxForIndexOrLast =
      new ErrorCode(11036, "invalid_stoppx_for_index_or_last", "Invalid StopPx (too high or too low) as to current index or market.");

    public static ErrorCode OutdatedInstrumentForIvOrder =
      new ErrorCode(11037, "outdated_instrument_for_IV_order", "Instrument already not available for trading.");

    public static ErrorCode NoAdvancedForFutures = new ErrorCode(11038, "no_adv_for_futures", "Advanced orders are not available for futures.");
    public static ErrorCode NoAdvancedPostOnly = new ErrorCode(11039, "no_adv_postonly", "Advanced post-only orders are not supported yet.");
    public static ErrorCode NotAdvancedOrder = new ErrorCode(11041, "not_adv_order", "Advanced order properties can't be set if the order is not advanced.");
    public static ErrorCode PermissionDenied = new ErrorCode(11042, "permission_denied", "Permission for the operation has been denied.");
    public static ErrorCode BadArgument = new ErrorCode(11043, "bad_argument", "Bad argument has been passed.");
    public static ErrorCode NotOpenOrder = new ErrorCode(11044, "not_open_order", "Attempt to do open order operations with the not open order.");
    public static ErrorCode InvalidEvent = new ErrorCode(11045, "invalid_event", "Event name has not been recognized.");

    public static ErrorCode OutdatedInstrument = new ErrorCode(11046, "outdated_instrument",
      "At several minutes to instrument expiration, corresponding advanced implied volatility orders are not allowed.");

    public static ErrorCode UnsupportedArgumentCombination =
      new ErrorCode(11047, "unsupported_arg_combination", "The specified combination of arguments is not supported.");

    public static ErrorCode WrongMaxShowForOption = new ErrorCode(11048, "wrong_max_show_for_option", "Wrong Max Show for options.");
    public static ErrorCode BadArguments = new ErrorCode(11049, "bad_arguments", "Several bad arguments have been passed.");
    public static ErrorCode BadRequest = new ErrorCode(11050, "bad_request", "Request has not been parsed properly.");
    public static ErrorCode SystemMaintenance = new ErrorCode(11051, "system_maintenance", "System is under maintenance.");

    public static ErrorCode SubscribeErrorUnsubscribed = new ErrorCode(11052, "subscribe_error_unsubscribed",
      "Subscription error. However, subscription may fail without this error, please check list of subscribed channels returned, as some channels can be not subscribed due to wrong input or lack of permissions.");

    public static ErrorCode TransferNotFound = new ErrorCode(11053, "transfer_not_found", "Specified transfer is not found.");
    public static ErrorCode InvalidAddress = new ErrorCode(11090, "invalid_addr", "Invalid address.");
    public static ErrorCode InvalidTransferAddress = new ErrorCode(11091, "invalid_transfer_address", "Invalid addres for the transfer.");
    public static ErrorCode AddressAlreadyExist = new ErrorCode(11092, "address_already_exist", "The address already exists.");
    public static ErrorCode MaxAddressCountExceeded = new ErrorCode(11093, "max_addr_count_exceeded", "Limit of allowed addresses has been reached.");

    public static ErrorCode InternalServerError = new ErrorCode(11094, "internal_server_error",
      "Some unhandled error on server. Please report to admin. The details of the request will help to locate the problem.");

    public static ErrorCode DisabledDepositAddressCreation =
      new ErrorCode(11095, "disabled_deposit_address_creation", "Deposit address creation has been disabled by admin.");

    public static ErrorCode AddressBelongsToUser = new ErrorCode(11096, "address_belongs_to_user", "Withdrawal instead of transfer.");
    public static ErrorCode BadTfaCode = new ErrorCode(12000, "bad_tfa", "Wrong TFA code");
    public static ErrorCode TooManySubaccounts = new ErrorCode(12001, "too_many_subaccounts", "Limit of subbacounts is reached.");
    public static ErrorCode WrongSubaccountName = new ErrorCode(12002, "wrong_subaccount_name", "The input is not allowed as name of subaccount.");
    public static ErrorCode TfaOverLimit = new ErrorCode(12998, "tfa_over_limit", "The number of failed TFA attempts is limited.");
    public static ErrorCode LoginOverLimit = new ErrorCode(12003, "login_over_limit", "The number of failed login attempts is limited.");
    public static ErrorCode RegistrationOverLimit = new ErrorCode(12004, "registration_over_limit", "The number of registration requests is limited.");
    public static ErrorCode CountryIsBanned = new ErrorCode(12005, "country_is_banned", "The country is banned (possibly via IP check).");

    public static ErrorCode TransferNotAllowed =
      new ErrorCode(12100, "transfer_not_allowed", "Transfer is not allowed. Possible wrong direction or other mistake.");

    public static ErrorCode TfaUsed = new ErrorCode(12999, "tfa_used", "TFA code is correct but it is already used. Please, use next code.");
    public static ErrorCode InvalidLogin = new ErrorCode(13000, "invalid_login", "Login name is invalid (not allowed or it contains wrong characters).");
    public static ErrorCode AccountNotActivated = new ErrorCode(13001, "account_not_activated", "Account must be activated.");
    public static ErrorCode AccountBlocked = new ErrorCode(13002, "account_blocked", "Account is blocked by admin.");
    public static ErrorCode TfaRequired = new ErrorCode(13003, "tfa_required", "This action requires TFA authentication.");
    public static ErrorCode InvalidCredentials = new ErrorCode(13004, "invalid_credentials", "Invalid credentials has been used.");
    public static ErrorCode PasswordMatchError = new ErrorCode(13005, "pwd_match_error", "Password confirmation error.");
    public static ErrorCode SecurityError = new ErrorCode(13006, "security_error", "Invalid Security Code.");
    public static ErrorCode UserNotFound = new ErrorCode(13007, "user_not_found", "User's security code has been changed or wrong.");
    public static ErrorCode RequestFailed = new ErrorCode(13008, "request_failed", "Request failed because of invalid input or internal failure.");

    public static ErrorCode Unauthorized = new ErrorCode(13009, "unauthorized",
      "Wrong or expired authorization token or bad signature. For example, please check scope of the token, \"connection\" scope can't be reused for other connections.");

    public static ErrorCode ValueRequired = new ErrorCode(13010, "value_required", "Invalid input, missing value.");
    public static ErrorCode ValueTooShort = new ErrorCode(13011, "value_too_short", "Input is too short.");
    public static ErrorCode UnavailableInSubaccount = new ErrorCode(13012, "unavailable_in_subaccount", "Subaccount restrictions.");
    public static ErrorCode InvalidPhoneNumber = new ErrorCode(13013, "invalid_phone_number", "Unsupported or invalid phone number.");
    public static ErrorCode CannotSendSms = new ErrorCode(13014, "cannot_send_sms", "SMS sending failed -- phone number is wrong.");
    public static ErrorCode InvalidSmsCode = new ErrorCode(13015, "invalid_sms_code", "Invalid SMS code.");
    public static ErrorCode InvalidInput = new ErrorCode(13016, "invalid_input", "Invalid input.");
    public static ErrorCode SubscriptionFailed = new ErrorCode(13017, "subscription_failed", "Subscription hailed, invalid subscription parameters.");
    public static ErrorCode InvalidContentType = new ErrorCode(13018, "invalid_content_type", "Invalid content type of the request.");
    public static ErrorCode OrderbookClosed = new ErrorCode(13019, "orderbook_closed", "Closed, expired order book.");
    public static ErrorCode NotFound = new ErrorCode(13020, "not_found", "Instrument is not found, invalid instrument name.");
    public static ErrorCode Forbidden = new ErrorCode(13021, "forbidden", "Not enough permissions to execute the request, forbidden.");

    public static ErrorCode MethodSwitchedOffByAdmin =
      new ErrorCode(13025, "method_switched_off_by_admin", "API method temporarily switched off by administrator.");

    public static ErrorCode InvalidParams = new ErrorCode(-32602, "Invalid params", "see JSON-RPC spec.");
    public static ErrorCode MethodNotFound = new ErrorCode(-32601, "Method not found", "see JSON-RPC spec.");
    public static ErrorCode ParseError = new ErrorCode(-32700, "Parse error", "see JSON-RPC spec.");
    public static ErrorCode MissingParams = new ErrorCode(-32000, "Missing params", "see JSON-RPC spec.");

    public int Code { get; }
    public string ShortMessage { get; }
    public string Description { get; }

    private ErrorCode(int code, string shortMessage, string description)
    {
      Code = code;
      ShortMessage = shortMessage;
      Description = description;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      switch (obj)
      {
        case ErrorCode codeObj:
          return Code == codeObj.Code;
        case int codeInt:
          return Code == codeInt;
        default:
          return false;
      }
    }

    public override int GetHashCode()
    {
      return Code.GetHashCode();
    }

    public static bool operator ==(ErrorCode a, int b)
    {
      // ReSharper disable once SuspiciousTypeConversion.Global
      return !ReferenceEquals(null, a) && a.Equals(b);
    }

    public static bool operator !=(ErrorCode a, int b)
    {
      // ReSharper disable once SuspiciousTypeConversion.Global
      return ReferenceEquals(null, a) || !a.Equals(b);
    }
  }
}
