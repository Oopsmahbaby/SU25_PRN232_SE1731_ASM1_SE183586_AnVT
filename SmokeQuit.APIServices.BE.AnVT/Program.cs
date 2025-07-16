using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using SmokeQuit.Repositories.AnVT.DBContext;
using SmokeQuit.Repositories.AnVT.Models;
using SmokeQuit.Services.AnVT;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SU25_PRN232_SE1731_G6_SmokeQuitContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
	});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
			// ClockSkew = TimeSpan.Zero
		};
	});
builder.Services.AddSwaggerGen(option =>
{
	////JWT Config
	option.DescribeAllParametersInCamelCase();
	option.ResolveConflictingActions(conf => conf.First());     // duplicate API name if any, ex: Get() & Get(string id)
	option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter a valid token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			new string[]{}
		}
	});
});

static IEdmModel GetEdmModel()
{
	var odataBuilder = new ODataConventionModelBuilder();
	odataBuilder.EntitySet<BlogPostsAnVt>("BlogPostsAnVt"); // EDM - ENTITY DATA MODEL
	odataBuilder.EntitySet<QuitPlansAnhDtn>("QuitPlansAnhDtn");
	return odataBuilder.GetEdmModel();
}
builder.Services.AddControllers().AddOData(options =>
{
	options.Select().Filter().OrderBy().Expand().SetMaxTop(null).Count();
	options.AddRouteComponents("odata", GetEdmModel());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SystemUserAccountService>();
builder.Services.AddScoped<IQuitPlansAnhDtn_AnVTService, QuitPlansAnhDtn_AnVTService>();
builder.Services.AddScoped<IBlogPostsAnVTService, BlogPostsAnVTService>();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
