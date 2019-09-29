
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;

//namespace Gateway.Middleware
//{
//    public class GatewayAuthNAttribute : ActionFilterAttribute
//    {
//        [ActionFilterDependency]
//        public IApiAuthorizeService AuthService { get; set; }

//        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
//        {
//            var (Success, Error, ApiKey, ApiPassphrase, ApiSignature, ApiNonce) = ReadAuthHeaders(actionContext);
//            if (!Success)
//            {
//                // TODO: Beautify
//                var result = new Result<string>
//                {
//                    Items = new ResultItem<string>[]
//                    {
//                        new ResultItem<string>
//                        {
//                            Status = new BTCS.LibStd.Status(1, Error)
//                        }
//                    }
//                };
//                CreateHttpResponse(actionContext, HttpStatusCode.Unauthorized, result);
//                return;
//            }

//            var authRequest = new Request<AuthApiAccessRequestItem>
//            {
//                Payload = new AuthApiAccessRequestItem
//                {
//                    ApiKey = ApiKey,
//                    ApiPassphrase = ApiPassphrase,
//                    ApiSignature = ApiSignature,
//                    ApiNonce = ApiNonce,
//                    ApiMethod = actionContext.Request.Method.Method,
//                    ApiPath = actionContext.Request.RequestUri.LocalPath
//                }
//            };
//            authRequest.Properties = new EnvelopeProperties(actionContext.ActionArguments);

//            var authResult = await AuthService.AuthApiAccessAsync(authRequest);
//            var authItem = authResult.Items.First();
//            if (authItem.Status.Code != 0)
//            {
//                CreateHttpResponse(actionContext, HttpStatusCode.Unauthorized, authResult);
//                return;
//            }

//            // Inject customer id into downstream request
//            Attribution.InjectGatewaySettings(authItem.Data.CustomerId, actionContext);

//            await base.OnActionExecutingAsync(actionContext, cancellationToken);
//        }

//        public (bool Success, string Error, string ApiKey, string ApiPassphrase, string ApiSignature, long ApiNonce) ReadAuthHeaders(HttpActionContext actionContext)
//        {
//            HttpHeaders headers = actionContext.Request.Headers;
//            if (headers.TryGetHeaderValue(AuthConstants.ApiKey, out string apiKey, out string error)
//                && headers.TryGetHeaderValue(AuthConstants.ApiPassphrase, out string apiPassphrase, out error)
//                && headers.TryGetHeaderValue(AuthConstants.ApiSignature, out string apiSignature, out error)
//                && headers.TryGetHeaderValue(AuthConstants.ApiNonce, out string apiNonceStr, out error))
//            {
//                if (long.TryParse(apiNonceStr, out long apiNonce))
//                {
//                    return (true, null, apiKey, apiPassphrase, apiSignature, apiNonce);
//                }
//                else
//                {
//                    error = $"Header {AuthConstants.ApiNonce} value must be int64";
//                }
//            }
//            return (false, error, null, null, null, -1);
//        }

//        private void CreateHttpResponse(ActionExecutingContext actionContext, HttpStatusCode statusCode, object responseBody = null)
//        {
//            if (responseBody == null)
//            {
//                actionContext.Response = actionContext.Request.CreateResponse(statusCode);
//            }
//            else
//            {
//                actionContext.Response = actionContext.Request.CreateResponse(
//                    statusCode,
//                    responseBody,
//                    actionContext.ControllerContext.Configuration.Formatters.JsonFormatter
//                    );
//            }
//        }
//    }
//}
