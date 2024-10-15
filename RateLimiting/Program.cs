using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


#region limitByPolicy

    #region ---------------------------- ** NormalLimiter ** ---------------------------------------
        
        #region GetConcurrencyLimiter
            builder.Services.AddRateLimiter(options =>
            {
                options.AddConcurrencyLimiter("Concurency", opt =>
                {
                    opt.PermitLimit = 1; // تعداد درخواست 
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // ترتیب پردازش صف
                    opt.QueueLimit = 1; // محدودیت صف
                });
            });
        #endregion
        
        
        
        #region FixedWindowLimiter
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("FixedWindowLimiter", opt=>
                {
                    opt.AutoReplenishment = true; // تکمیل خودکار
                    opt.Window = TimeSpan.FromSeconds(10); // بازه ویندوز برای رفرش کردن ظرفیت درخواستها
                    opt.PermitLimit = 3;
                    opt.QueueLimit = 2;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
        #endregion
        
        
        
        #region SlidingWindowLimiter 
            builder.Services.AddRateLimiter(options =>
            {
                options.AddSlidingWindowLimiter("SlidingWindow", opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(30); // بازه ویندوز
                    opt.SegmentsPerWindow = 3; // تعداد تکه در هر ویندوز
                    opt.PermitLimit = 100;
                    opt.QueueLimit = 2;
                    opt.AutoReplenishment = true;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
        #endregion
        
        
        
        #region TokenBucketLimiter
        builder.Services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter("TokenBucket", opt =>
            {
                opt.TokenLimit = 6; // ظرفیت سطل
                opt.TokensPerPeriod = 4; // میزان توکن اضافه شونده در بازه مشخص شده
                opt.ReplenishmentPeriod = TimeSpan.FromSeconds(20); // بازه مشخص شده برا اضافه کردن توکن های جدید
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 2;// محدودیت صف
            });
        });
        #endregion

    #endregion

    #region ----------------------------- ** LimitByKey(IpAddress) ** ----------------------------
    
        #region FixedWindowLimiter
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("FixPerIpPolicy", context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                          context.Connection.RemoteIpAddress?.ToString(), ip =>
                          new FixedWindowRateLimiterOptions
                          {
                              PermitLimit = 3,
                              AutoReplenishment = true,
                              Window = TimeSpan.FromSeconds(10),
                              QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                              QueueLimit = 2 // Allow 2 requests to queue if tokens are exhausted
                          });
                });
            });
        #endregion
    
    
        #region SlidingWindowLimiter 
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("SlidingPerIpPolicy", context =>
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(
                          context.Connection.RemoteIpAddress?.ToString(), ip =>
                          new SlidingWindowRateLimiterOptions
                          {
                            PermitLimit = 3,
                            SegmentsPerWindow = 4,
                            AutoReplenishment =true, 
                            Window = TimeSpan.FromMinutes(2),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 2 // Allow 2 requests to queue if tokens are exhausted
                          });
                });
            });
        #endregion
    
    
        #region GetConcurrencyLimiter
        builder.Services.AddRateLimiter(options =>
        {
            // پالیسی زیر باید در کنترلر اضافه شود PerIpPolicy
            options.AddPolicy("PerIpPolicy", context =>
            {
                return RateLimitPartition.GetConcurrencyLimiter(
                      context.Connection.RemoteIpAddress?.ToString(), ip =>
                      new ConcurrencyLimiterOptions
                      {
                          PermitLimit = 3,
                          QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                          QueueLimit = 2 // Allow 2 requests to queue if tokens are exhausted
                      });
            });
        });
        #endregion
    
    
        #region TokenBucketLimiter
        builder.Services.AddRateLimiter(options =>
        {
            // پالیسی زیر باید در کنترلر اضافه شود PerIpPolicy
            options.AddPolicy("PerIpPolicy", context =>
            {
                return RateLimitPartition.GetTokenBucketLimiter(
                      context.Connection.RemoteIpAddress?.ToString(), ip =>
                      new TokenBucketRateLimiterOptions
                      {
                          TokenLimit = 6, // Maximum number of tokens (i.e., max 10 requests at once)
                          TokensPerPeriod = 3, // Number of tokens replenished per period
                          ReplenishmentPeriod = TimeSpan.FromSeconds(10), // Replenish tokens every minute
                          QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                          QueueLimit = 2 // Allow 2 requests to queue if tokens are exhausted
                      });
            });
        });
        #endregion

    #endregion

#endregion


#region GlobalLimiter
    builder.Services.AddRateLimiter(options =>
    {
        //options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
        //            RateLimitPartition.GetFixedWindowLimiter(
        //                partitionKey: httpcontext.Connection.RemoteIpAddress?.ToString()!,
        //                factory: partition => new FixedWindowRateLimiterOptions
        //                {
        //                    AutoReplenishment = true,
        //                    PermitLimit = 3,
        //                    Window = TimeSpan.FromMinutes(5)
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
    });
#endregion



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// this is Mandatory
app.UseRateLimiter();

app.UseAuthorization();

// this is optional
app.MapDefaultControllerRoute().RequireRateLimiting("FixedWindowLimiter");

app.Run();
