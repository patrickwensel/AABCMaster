using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;

namespace AABC.Cache.Tests
{
    [TestClass]
    public class RedisTests
    {
        [TestMethod]
        public void redis_is_reachable()
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect("localhost");
                var result = redis.GetDatabase().Ping();
                Assert.IsTrue(result.Milliseconds > 0);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void add_element_to_hash()
        {
            var client = GetClient();
            var cacheManager = new CacheRepository<TestCustomer, int>(client, m => m.Id);
            client.FlushDb();
            var customer = new TestCustomer { Id = 1, Name = "John Smith" };
            cacheManager.Set(customer);
        }

        private static ICacheClient GetClient()
        {
            var redisConfiguration = new RedisConfiguration()
            {
                AbortOnConnectFail = true,
                //KeyPrefix = "_my_key_prefix_",
                Hosts = new RedisHost[]
                {
                    new RedisHost(){Host = "localhost"},
                },
                AllowAdmin = true,
                ConnectTimeout = 3000,
                Database = 0,
                //Ssl = true,
                //Password = "my_super_secret_password",
                ServerEnumerationStrategy = new ServerEnumerationStrategy()
                {
                    Mode = ServerEnumerationStrategy.ModeOptions.All,
                    TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                    UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
                }
            };
            var serializer = new NewtonsoftSerializer();
            var client = new StackExchangeRedisCacheClient(serializer, redisConfiguration);
            return client;
        }

        public class TestCustomer
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
