using Newtonsoft.Json;
using System.Text;

namespace ProcessesTestRunner.Shared.Models;

public enum SendProcessWay
{
    Server,
    Client
}

public record TransitionDataModel
{
    public SendProcessWay SendProcessWay { get; set; }
    public string? RandomText { get; set; }
    public char[]? RandomCharArray { get; set; }
    public int? RandomInteger { get; set; }
    public double? RandomDouble { get; set; }

    public static implicit operator byte[](TransitionDataModel model) => ObjectToByteArray(model);
    public static implicit operator TransitionDataModel(byte[] model) => ByteArrayToTransitionDataModel(model);

    public bool Compare(TransitionDataModel transitionDataModel)
     => RandomText == transitionDataModel.RandomText
        && RandomCharArray.SequenceEqual(transitionDataModel.RandomCharArray)
        && RandomInteger == transitionDataModel.RandomInteger
        && RandomDouble == transitionDataModel.RandomDouble;

    private static byte[] ObjectToByteArray(TransitionDataModel obj) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
    private static TransitionDataModel? ByteArrayToTransitionDataModel(byte[] obj) =>
        JsonConvert.DeserializeObject<TransitionDataModel>(Encoding.UTF8.GetString(obj));

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
