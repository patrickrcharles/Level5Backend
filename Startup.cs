//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Microsoft.Extensions.Options;
//using Microsoft.AspNetCore.Mvc.ApiExplorer;
//using Microsoft.OpenApi.Any;
//using Level5Backend.Models;

//namespace level5Server.Models.level5
//{
//    public class Startup
//    {
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public IConfiguration Configuration { get; }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public void ConfigureServices(IServiceCollection services)
//        {

//            services.AddControllers();

//            //services.AddApiVersioning(options =>
//            //{
//            //    options.DefaultApiVersion = new ApiVersion(1, 0);
//            //    options.AssumeDefaultVersionWhenUnspecified = true;
//            //    options.ReportApiVersions = true;
//            //    options.ApiVersionReader = ApiVersionReader.Combine(
//            //        new QueryStringApiVersionReader("api-version"),
//            //        new HeaderApiVersionReader("api-version"));
//            //});

//            //services.AddVersionedApiExplorer(options =>
//            //{
//            //    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
//            //    // note: the specified format code will format the version as "'v'major[.minor][-status]"
//            //    options.GroupNameFormat = "'v'VVV";

//            //    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
//            //    // can also be used to control the format of the API version in route templates
//            //    options.SubstituteApiVersionInUrl = true;
//            //});
//            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
//            //services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

//            //// Register the Swagger generator, defining 1 or more Swagger documents
//            //services.AddSwaggerGen(c =>
//            //{
//            //    c.SwaggerDoc("v1", new OpenApiInfo
//            //    {
//            //        Version = "v1",
//            //        Title = "Level 5 API",
//            //        Description = "Level 5 web api",
//            //    });
//            //    // Set the comments path for the Swagger JSON and UI.
//            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//            //    c.IncludeXmlComments(xmlPath);
//            //});

//            services.AddRazorPages();
//            // level5 dbcontext
//            services.AddDbContext<Level5Context>(options =>
//            options.UseMySql("DefaultConnection", ServerVersion.AutoDetect("DefaultConnection")));


//            // jwt token config
//            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
//            {
//                options.RequireHttpsMetadata = false;
//                options.SaveToken = true;
//                options.TokenValidationParameters = new TokenValidationParameters()
//                {
//                    ValidateIssuer = true,
//                    ValidateAudience = true,
//                    ValidAudience = Configuration["Jwt:Audience"],
//                    ValidIssuer = Configuration["Jwt:Issuer"],
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
//                };
//            });

//            //services.AddControllersWithViews();

//            // add custom locations for views
//            // {1} = folder/controller name 
//            // {0} = file returned, ex. index.cshtml
//            //services.Configure<RazorViewEngineOptions>(o =>
//            //{
//            //    o.ViewLocationFormats.Clear();
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/level5/{1}/{0}" + RazorViewEngine.ViewExtension);
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/level5/Shared/{0}" + RazorViewEngine.ViewExtension);
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/serialkiller/{1}/{0}" + RazorViewEngine.ViewExtension);
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/serialkiller/Shared/{0}" + RazorViewEngine.ViewExtension);
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/nftairdrop/{1}/{0}" + RazorViewEngine.ViewExtension);
//            //    o.ViewLocationFormats.Add
//            //    ("/Views/nftairdrop/Shared/{0}" + RazorViewEngine.ViewExtension);
//            //});
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            app.UseStaticFiles();

//            app.UseHttpsRedirection();
//            app.UseRouting();

//            app.UseSwagger();
//            app.UseSwaggerUI(
//                options =>
//                {
//                // build a swagger endpoint for each discovered API version
//                foreach (var description in provider.ApiVersionDescriptions)
//                    {
//                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
//                    }
//                });

//            //// Enable middleware to serve generated Swagger as a JSON endpoint.
//            //app.UseSwagger();
//            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
//            //// specifying the Swagger JSON endpoint.
//            //app.UseSwaggerUI(c =>
//            //{
//            //    c.SwaggerEndpoint("v1/swagger.json", "Level 5 v1");
//            //    //c.RoutePrefix = "swagger/ui";
//            //});

//            app.UseAuthentication();
//            app.UseAuthorization();
//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapControllers();
//            });
//        }

//        public class SwaggerDefaultValues : IOperationFilter
//        {
//            public void Apply(OpenApiOperation operation, OperationFilterContext context)
//            {
//                var apiDescription = context.ApiDescription;
//                operation.Deprecated |= apiDescription.IsDeprecated();

//                if (operation.Parameters == null)
//                    return;

//                // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
//                // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
//                foreach (var parameter in operation.Parameters)
//                {
//                    var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
//                    if (parameter.Description == null)
//                    {
//                        parameter.Description = description.ModelMetadata?.Description;
//                    }

//                    if (parameter.Schema.Default == null && description.DefaultValue != null)
//                    {
//                        parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
//                    }

//                    parameter.Required |= description.IsRequired;
//                }
//            }
//        }

//        public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
//        {
//            private readonly IApiVersionDescriptionProvider _provider;

//            public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

//            public void Configure(SwaggerGenOptions options)
//            {
//                // add a swagger document for each discovered API version
//                // note: you might choose to skip or document deprecated API versions differently
//                foreach (var description in _provider.ApiVersionDescriptions)
//                {
//                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
//                }
//            }

//            private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
//            {
//                var info = new OpenApiInfo()
//                {
//                    Title = "Skeleton District API",
//                    Version = description.ApiVersion.ToString(),
//                };

//                if (description.IsDeprecated)
//                {
//                    info.Description += " This API version has been deprecated.";
//                }

//                return info;
//            }
//        }
//    }
//}
