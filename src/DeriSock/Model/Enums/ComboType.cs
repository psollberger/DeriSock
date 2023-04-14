namespace DeriSock.Model;

using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum ComboType
{
  Undefined,
  FutureSpread,
  CallSpread,
  PutSpread,
  Straddle,
  Strangle,
  StrangleItm,
  RiskReversal,
  RiskReversalItm,
  CallCalendarSpread,
  PutCalendarSpread,
  CallDiagonalCalendar,
  PutDiagonalCalendar,
  StraddleCalendar,
  StraddleCalendarDiagonal,
  ReversalConversion,
  CallButterfly,
  PutButterfly,
  IronButterfly,
  SkinnyCallButterfly,
  SkinnyPutButterfly,
  CallLadder,
  PutLadder,
  CallCondor,
  PutCondor,
  IronCondor,
  Box,
  JellyRoll,
  PutRatioSpread1x3,
  CallRatioSpread1x2,
  CallRatioSpread1x3,
  CallRatioSpread2x3,
  PutRatioSpread1x2,
  PutRatioSpread2x3,
  Call,
  Put,
  ShortCall,
  ShortPut,
  LongCall,
  LongPut
}
#pragma warning restore CS1591
