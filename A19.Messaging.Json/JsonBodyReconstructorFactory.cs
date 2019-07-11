using A19.Messaging;

namespace Mrh.Messaging.Json
{
    public class JsonBodyReconstructorFactory : IBodyReconstructorFactory<string>
    {
        public IBodyReconstructor<string> Create(int total)
        {
            return new JsonBodyReconstructor(total);
        }
    }
}