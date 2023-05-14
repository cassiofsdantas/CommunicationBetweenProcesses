using System.Net;

namespace ProcessesTestRunner.Shared.Identifiers;

public static class MemoryMappedIdentifiers
{
    public static string SharedFileMemorySpaceIdentifier => nameof(SharedFileMemorySpaceIdentifier);
    public static string SharedMemorySpaceIdentifier => nameof(SharedMemorySpaceIdentifier);
    public static string MMAPSpaceIdentifier => nameof(MMAPSpaceIdentifier);
    public static string MutextSpaceIdentifier => nameof(MutextSpaceIdentifier);

    public const int SocketPort = 8092;
    public const int SocketPortSecondary = 8093;
    public static IPAddress SocketIp => IPAddress.Parse("127.0.0.1");
}
