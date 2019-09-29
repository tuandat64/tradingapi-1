//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using BTCS.Channel.Pro.Rest.Core.Utils;
//using BTCS.LibStd;
//using Gateway.Host.Web.Middleware.Authentication.Abstractions;
//using Gateway.Host.Web.Middleware.Services;
//using Newtonsoft.Json;
//using Trading.Foundation.Protocol;

//namespace Gateway.ApiAccess
//{
//    public static class ResultExtensions
//    {
//        public static TResult AddStatusItem<TResult, TItem>(this TResult result, int code, string message)
//            where TResult : Result<TItem>
//        {
//            AddStatusItem<TResult, TItem>(result, new Status(code, message));
//            return result;
//        }

//        public static TResult AddStatusItem<TResult, TItem>(this TResult result, Status status)
//            where TResult : Result<TItem>
//        {
//            result.Items = result.Items ?? new List<ResultItem<TItem>>();
//            result.Items.Add(new ResultItem<TItem>
//            {
//                Status = status
//            });
//            return result;
//        }

//        public static void AddStatusItem2<T>(this Result<T> result, int code, string message)
//        {
//            AddStatusItem2(result, new Status(code, message));
//        }

//        public static void AddStatusItem2<T>(this Result<T> result, Status status)
//        {
//            result.Items = result.Items ?? new List<ResultItem<T>>();
//            result.Items.Add(new ResultItem<T>
//            {
//                Status = status
//            });
//        }
//    }

//    public class ApiAuthorizeService : IApiAuthorizeService
//    {
//        private readonly INonceService _nonceService;
//        private readonly ICustomerApiKeyService _customerApiKeyService;

//        public ApiAuthorizeService(INonceService nonceService, ICustomerApiKeyService customerApiKeyService)
//        {
//            _nonceService = nonceService ?? throw new ArgumentNullException(nameof(nonceService));
//            _customerApiKeyService = customerApiKeyService ?? throw new ArgumentNullException(nameof(customerApiKeyService));
//        }

//        async Task<Result<AuthApiAccessResponseItem>> IApiAuthorizeService.AuthApiAccessAsync(IRequest<AuthApiAccessRequestItem> request)
//        {
//            var requestItem = request.Payload;

//            string payload = await ReadPayloadAsync(requestItem, request.Properties);

//            var nonceResult = await _nonceService.IsValidNonceAsync(payload, requestItem.ApiNonce);
//            var nonceItem = nonceResult.Items.First();
//            if (nonceItem.Status.Code != 0)
//            {
//                var t = new AuthApiAccessResponse();
//                t.AddStatusItem2(nonceItem.Status);
//                return t;
//            }

//            var accessResult = await _customerApiKeyService.GetByApiKeyAsync(requestItem.ApiKey, requestItem.ApiPassphrase);
//            var accessItem = accessResult.Items.First();
//            if (accessItem.Status.Code != 0)
//            {
//                var t = new AuthApiAccessResponse();
//                t.AddStatusItem2(accessItem.Status);
//                return t;
//            }

//            BTCS.Portfolio.Domain.CustomerApiKey accessData = accessItem.Data;
//            if (!AuthUtil.IsValidHMACSHA256HexSignature(payload, accessData.PrivateApiKey, requestItem.ApiSignature))
//            {
//                var t = new AuthApiAccessResponse();
//                t.AddStatusItem2(1, "Invalid Signature");
//                return t;
//            }

//            return new AuthApiAccessResponse
//            {
//                Items = new ResultItem<AuthApiAccessResponseItem>[]
//                {
//                    new ResultItem<AuthApiAccessResponseItem>
//                    {
//                        Status = new Status(),
//                        Data = new AuthApiAccessResponseItem
//                        {
//                            CustomerId = accessData.CustomerId
//                        }
//                    }
//                }
//            };
//        }

//        public Task<string> ReadPayloadAsync(AuthApiAccessRequestItem requestItem, IDictionary<string, object> properties)
//        {
//            var values = properties.Values;
//            var content = values
//                .Where(v => v != null)
//                .Select(v => JsonConvert.SerializeObject(v))
//                .ToList();

//            return Task.FromResult(GetPayload(requestItem.ApiMethod, requestItem.ApiPath, requestItem.ApiKey, requestItem.ApiPassphrase, requestItem.ApiNonce, content));
//        }

//        public string GetPayload(params object[] objects)
//        {
//            return GetPayload((IEnumerable<object>)objects);
//        }

//        private string GetPayload(IEnumerable<object> objects)
//        {
//            var sb = new StringBuilder();
//            AppendValues(objects, sb);
//            return sb.ToString();
//        }

//        // TODO: Perhaps make separate nuget util class helper method
//        private void AppendValues(object obj, StringBuilder sb)
//        {
//            Type type = obj.GetType();
//            if (type != typeof(object))
//            {
//                if (type.IsValueType || obj is string)
//                {
//                    sb.Append(obj);
//                }
//                else if (type.IsArray || typeof(ICollection).IsAssignableFrom(type))
//                {
//                    IEnumerable<object> list = (IEnumerable<object>)obj;
//                    foreach (object innerObj in list)
//                    {
//                        AppendValues(innerObj, sb);
//                    }
//                }

//                else
//                {
//                    FieldInfo[] fields = obj.GetType().GetFields();
//                    foreach (FieldInfo prop in fields)
//                    {
//                        AppendValues(prop.GetValue(obj), sb);
//                    }
//                    PropertyInfo[] props = obj.GetType().GetProperties();
//                    foreach (PropertyInfo prop in props)
//                    {
//                        AppendValues(prop.GetValue(obj), sb);
//                    }
//                }
//            }
//        }
//    }
//}
