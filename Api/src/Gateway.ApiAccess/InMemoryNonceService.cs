//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Gateway.ApiAccess
//{
//    public class InMemoryNonceService : INonceService
//    {
//        private readonly MemoryCache _cache;
//        // Must be long enough so that it does not expire in between a successful
//        // pass of nonce check and subsequent read of the cache.
//        private readonly TimeSpan _cacheExpiryTimespan = new TimeSpan(0, 1, 0);
//        private readonly int _nonceBoundsSeconds = 10;

//        public InMemoryNonceService()
//        {
//            _cache = new MemoryCache(new MemoryCacheOptions());
//        }

//        public Task<Result> IsValidNonceAsync(string signature, long nonce)
//        {
//            // Do nonce check first as it is much faster than looking in cache
//            DateTime nonceTime = TimeUtil.GetDateTimeFromUnixEpochTime(nonce);
//            TimeSpan diff = DateTime.UtcNow - nonceTime;
//            if (Math.Abs(diff.TotalSeconds) > _nonceBoundsSeconds)
//            {
//                return Task.FromResult(new Result
//                {
//                    Items = new List<ResultItem>
//                    {
//                        new ResultItem
//                        {
//                            Status = new Status(1, "Nonce out of bounds")
//                        }
//                    }
//                });
//            }

//            if (_cache.TryGetValue(signature, out _))
//            {
//                return Task.FromResult(new Result
//                {
//                    Items = new List<ResultItem>
//                    {
//                        new ResultItem
//                        {
//                            Status = new Status(1, "Duplicate signature")
//                        }
//                    }
//                });
//            }
            
//            // We do not care about the value. We just want to store signature until nonce is out of bounds.
//            _cache.Set(signature, "", new DateTimeOffset(DateTime.UtcNow.Add(_cacheExpiryTimespan)));
//            return Task.FromResult(new Result
//            {
//                Items = new List<ResultItem>
//                {
//                    new ResultItem
//                    {
//                        Status = new Status()
//                    }
//                }
//            });
//        }
//    }
//}
