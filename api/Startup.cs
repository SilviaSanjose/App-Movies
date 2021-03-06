using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Context;
using api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace api
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)  //constructor para añadir la configuración de la conexion con la bbdd
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers();  //General para usar controladores.
            services.AddControllers(action => {   //forma para indicar formatos aceptados o no en la petición api
                action.ReturnHttpNotAcceptable = true;  //no acepta Http
                
            })
            .AddXmlDataContractSerializerFormatters() //indicar que acepta formato XML
            .AddNewtonsoftJson();


            //Inyección de dependencias: Con sólo cambiar un parámetro no hace falta tocar más código en el contructor de CastController ni en clases
            services.AddTransient<IMailService, LocalMailService>();  //comentar para usar CloudMail
            //services.AddTransient<IMailService, CloudMailService>();   //comentar para usar LocalMail

            //Entity Framework
            string connectionString = _configuration["connectionStrings:movieInfoDbConnectionString"];
            services.AddDbContext<MovieInfoContext>(o => {
                o.UseSqlite(connectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
