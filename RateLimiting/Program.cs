using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


#region ByPolicy

#region NormalLimiter
#region GetConcurrencyLimiter
//builder.Services.AddRateLimiter(options =>
//{
//    // پالیسی زیر باید در کنترلر اضافه شود Concurency
//    options.AddConcurrencyLimiter("Concurency", options =>
//    {
//        options.PermitLimit = 1;
//        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        options.QueueLimit = 1;
//    });
//});
#endregion



#region FixedWindowLimiter
//builder.Services.AddRateLimiter(options =>
//{
//    options.AddFixedWindowLimiter("FixedWindowLimiter", options =>
//    {
//        options.AutoReplenishment = true;
//        options.PermitLimit = 3;
//        options.QueueLimit = 2;
//        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        options.Window = TimeSpan.FromSeconds(10);

//    });
//});
#endregion



#region SlidingWindowLimiter 
تا اینجا درست شد
//builder.Services.AddRateLimiter(options =>
//{
//    options.AddPolicy("PerIpPolicy", context =>
//    {
//        return RateLimitPartition.GetSlidingWindowLimiter(
//              context.Connection.RemoteIpAddress?.ToString(), ip =>
//              new TokenBucketRateLimiterOptions
//              {
//                  TokenLimit = 6, // Maximum number of tokens (i.e., max 10 requests at once)
//                  TokensPerPeriod = 3, // Number of tokens replenished per period
//                  ReplenishmentPeriod = TimeSpan.FromSeconds(10), // Replenish tokens every minute
//                  QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                  QueueLimit = 2 // Allow 2 requests to queue if tokens are exhausted
//              });
//    });
//});
#endregion



#region TokenBucketLimiter
//builder.Services.AddRateLimiter(options =>
//{
//    options.AddPolicy("PerIpPolicy", context =>
//    {
//        return RateLimitPartition.GetTokenBucketLimiter(
//              context.Connection.RemoteIpAddress?.ToString(), ip =>
//              new TokenBucketRateLimiterOptions
//              {
//                  TokenLimit = 6, // Maximum number of tokens (i.e., max 10 requests at once)
//                  TokensPerPeriod = 3, // Number of tokens replenished per period
//                  ReplenishmentPeriod = TimeSpan.FromSeconds(10), // Replenish tokens every minute
//                  QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                  QueueLimit = 2 // Allow 2 requests to queue if tokens are exhausted
//              });
//    });
//});
#endregion
#endregion



#endregion

#region GlobalLimiter
//builder.Services.AddRateLimiter(options =>
//{
//options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
//            RateLimitPartition.GetFixedWindowLimiter(
//                partitionKey: httpcontext.Request.Headers.Host.ToString(),
//                factory: partition => new FixedWindowRateLimiterOptions
//                {
//                    AutoReplenishment = true,
//                    PermitLimit = 3,
//                    Window = TimeSpan.FromMinutes(1)
//                }
//            ));

//options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
//    {

//        return RateLimitPartition.GetTokenBucketLimiter
//        (context.Connection.RemoteIpAddress!, _ =>
//            new TokenBucketRateLimiterOptions
//            {
//                TokenLimit = 5,
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                QueueLimit = 2,
//                ReplenishmentPeriod = TimeSpan.FromSeconds(10),
//                TokensPerPeriod = 2,
//                AutoReplenishment = true
//            });
//    });
//});
#endregion



// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
