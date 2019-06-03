namespace DeriSock.Utils
{
  using DeriSock.Converter;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Threading.Tasks;

  public class TypedTaskInfo<T> : TaskInfo
  {
    public IJsonConverter<T> Converter;
    public TaskCompletionSource<T> Tcs;

    public override object Convert(JToken value)
    {
      return this.Converter.Convert(value);
    }

    public override void Resolve(object value)
    {
      Tcs.SetResult((T)value);
    }

    public override void Reject(Exception e)
    {
      Tcs.SetException(e);
    }
  }
}
