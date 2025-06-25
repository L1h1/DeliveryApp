using Serilog;

namespace OrderService.API
{
    public static class DependencyInjection
    {
        public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, config) =>
            {
                config.WriteTo.Http(
                    requestUri: "http://logstash:5644",
                    queueLimitBytes: 100 * 1024 * 1024);
            });

            return builder;
        }
    }
}
