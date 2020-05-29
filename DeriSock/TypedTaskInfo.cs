namespace DeriSock.Utils
{
  using System;
  using System.Threading.Tasks;
  using DeriSock.Converter;
  using Newtonsoft.Json.Linq;

  public class TypedTaskInfo<T> : TaskInfo
  {
    public IJsonConverter<T> Converter { get; set; }
    public TaskCompletionSource<T> CompletionSource { get; set; }

    public override object Convert(JToken value)
    {
      return Converter.Convert(value);
    }

    public override void Resolve(object value)
    {
      CompletionSource.SetResult((T)value);
    }

    public override void Reject(Exception e)
    {
      CompletionSource.SetException(e);
    }
  }
}
