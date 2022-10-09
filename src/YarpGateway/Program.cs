
using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;
using Serilog;
using Shared.Hosting;
using YarpGateway.Extensions;

var assemblyName = "OrderService";
SerilogConfigurationHelper.Configure(assemblyName);

try
{
    Log.Information($"Starting {assemblyName}.");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host
        .UseSerilog()
        .AddYarpJson();// ���Yarp�������ļ�


    // ���Yarp�������ReverseProxy
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    //// Add services to the container.

    builder.Services.AddControllers();
    //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //builder.Services.AddEndpointsApiExplorer();
    //builder.Services.AddSwaggerGen();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Gateway",
            Version = "v1"
        });
        options.DocInclusionPredicate((docName, description) => true);
        options.CustomSchemaIds(type => type.FullName);
    });


    var app = builder.Build();

    // ����ڲ������Swagger�յ�
    app.UseSwaggerUIWithYarp();

    //�������ص�ַ���Զ���ת�� /swagger ����ҳ
    //app.UseRewriter(new RewriteOptions()
    //    // Regex for "", "/" and "" (whitespace)
    //    .AddRedirect("^(|\\|\\s+)$", "/swagger"));

    //builder.Services.AddCors(options =>
    //{
    //    options.AddDefaultPolicy(c =>
    //    {
    //        c
    //        //.WithOrigins(
    //        //    builder.Configuration["App:CorsOrigins"]
    //        //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
    //        //        .ToArray()
    //        //)
    //        //.SetIsOriginAllowedToAllowWildcardSubdomains()
    //        .AllowAnyOrigin()
    //        .AllowAnyHeader()
    //        .AllowAnyMethod();
    //        //.AllowCredentials();
    //    });
    //});
    //// Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}

    app.UseHttpsRedirection();

    app.MapControllers();

    //app.UseCors();
    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        // ���Yarp�ն�Endpoints
        endpoints.MapReverseProxy();
    });



    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"{assemblyName} terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}
