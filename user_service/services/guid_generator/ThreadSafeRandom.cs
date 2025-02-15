using System.Runtime.CompilerServices;

namespace user_service.services.guid_generator;

public static class ThreadSafeRandom
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Random ObtainThreadStaticRandom() => ObtainRandom();

    private static Random ObtainRandom() => Random.Shared;
}
