using Newtonsoft.Json;
using ProcessesTestRunner.Shared.Managers;
using ProcessesTestRunner.Shared.Models;

namespace ProcessesTestRunner.Shared.Mock;

public static class GetMockData
{
    private static string mockPath => DirectoryManager.GetFilePath(DirectoryManager.BaseDataMockPath, "TransitionDataModel.data");

    public static IEnumerable<TransitionDataModel> GetTransitionDataModelGenerated()
    {
        var desired = 1000;

        if (File.Exists(mockPath) && JsonConvert.DeserializeObject<IEnumerable<TransitionDataModel>>(File.ReadAllText(mockPath)) is IEnumerable<TransitionDataModel> mock && mock.Count() == desired)
        {
            return mock;
        }
        else
        {
            var result = GenerateMockData.GenerateTimes<TransitionDataModel>(desired);

            File.WriteAllText(mockPath, JsonConvert.SerializeObject(result));

            return result;
        }
    }
}