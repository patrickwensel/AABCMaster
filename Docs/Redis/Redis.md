# Redis Cache for AABC.Web

Use `MSOpenTech` Redis on Windows, v3.0+

* [Redis-Windows github](https://github.com/ServiceStack/redis-windows)
* [Redis-Windows docs](https://github.com/ServiceStack/redis-windows/blob/master/docs/msopentech-redis-on-windows.md)

There's three options to run on Windows:

1. Redis on Ubuntu on Windows
2. Redis on Windows via Vagrant
3. Redis on Winows via native port


We use Option 3

Use the files found in our repo's `/Assets` directory.  To update, refer to the latest from the github repo.

## Port and Config

Redis runs on port 6379 by default.  Use the following in Web.Config:

    <configuration>
	    <redisCacheClient allowAdmin="true" ssl="false" connectTimeout="3000" database="0">
		    <serverEnumerationStrategy mode="Single" targetRole="PreferSlave" unreachableServerAction="IgnoreIfOtherAvailable" />
		    <hosts>
			    <add host="127.0.0.1" cachePort="6379" />
		    </hosts>
	    </redisCacheClient>
	</configuration> 

And:

    <configuration>
        <configSections>
            <section name="redisCacheClient" type="StackExchange.Redis.Extensions.LegacyConfiguration.RedisCachingSectionHandler, StackExchange.Redis.Extensions.LegacyConfiguration" />
        </configSections
    </configuration>

## Running as a Service

    redis-server --service-install redis.windows.conf
    redis-server --service-uninstall
    redis-server --service-start
    redis-server --service-stop

    redis-server --service-install –service-name redisService1 –port 10001
    redis-server --service-start –service-name redisService1
    redis-server --service-install –service-name redisService2 –port 10002
    redis-server --service-start –service-name redisService2
    redis-server --service-install –service-name redisService3 –port 10003
    redis-server --service-start –service-name redisService3

## Monitoring

To view all redis operations in realtime, use:

    redis-cli monitor

(use CTRL+C to exit the monitor)

