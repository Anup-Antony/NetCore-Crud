using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCrud.Api.Repository;
using NetCoreCrud.Api.Services;
using NetCoreCrud.Api.Swagger;
using Swashbuckle.AspNetCore.Swagger;

namespace NetCoreCrud.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = Logging.GetLogger();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton(logger);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<CustomerDbContext>(options => options.UseInMemoryDatabase("Customer"));

            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Title = "Customer API",
                            Version = "v1",
                            Description = "This API performs CRUD operations using EF core."
                        });


                    options.OperationFilter<SwaggerHeaderFilter>();

                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "NetCoreCrud.Api.xml");
                    options.IncludeXmlComments(filePath);
                }

            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API");
            });

            app.UseMvc();
        }
    }
}
