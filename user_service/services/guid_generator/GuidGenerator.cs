namespace user_service.services.guid_generator;

public class GuidGenerator
{
    public static unsafe Guid GenerateByBytes()
    {
        var bytes = stackalloc byte[16];
        var dst = bytes;

        var random = ThreadSafeRandom.ObtainThreadStaticRandom();
        
        random.NextBytes(new Span<byte>(bytes, 16));

        return *(Guid*)bytes;
    }
}
