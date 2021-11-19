using AuthError.ServiceModel;
using ServiceStack;

namespace AuthError.ServiceInterface
{
    [RequiredPermission("CustomPermission")]
    [Authenticate]
    public class MyServices : Service
    {

        public object Any(Hello request)
        {
            return new HelloResponse { Result = $"Hello, {request.Name}!" };
        }
    }
}
