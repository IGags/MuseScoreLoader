using Logic;
using Microsoft.AspNetCore.Builder;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogic();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
        {
            builder.UseRouting();
            
            //if (env.IsDevelopment())
            {
                builder.UseSwagger();
                builder.UseSwaggerUI();
            }

            builder.UseEndpoints(x => x.MapControllers());
        }
    }
}
