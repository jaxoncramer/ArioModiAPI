using System;
namespace Ario.API.Models.Authentication
{
    public class IntrospectionRequest
    {
        public IntrospectionRequest() {}

        public IntrospectionRequest(Token token) {
            this.token = token.Authorization;
            this.token_type_hint = "access_token";
        }

        public string token { get; set; }
        public string token_type_hint { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string client_assertion { get; set; }
        public string client_assertion_type { get; set; }
    }
}
