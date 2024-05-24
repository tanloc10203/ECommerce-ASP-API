using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Models.Domain;
using ECommerce.Services.AuthService;
using ECommerce.Services.CategoryService;
using ECommerce.Services.FileService;
using ECommerce.Services.OrderService;
using ECommerce.Services.ProductService;
using ECommerce.Services.TokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Config để map dữ liệu từ file config
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppSettings"));

// Lấy mã bí mật để sử dụng jwt
var secretKey = builder.Configuration["JWT:Secret"] ?? "my-secretKey-default";
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerceCSharp API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[]{}
        }
    });
});

// Add Cors
builder.Services.AddCors((p) => p.AddPolicy("ECommerceCSharpApiCors", build =>
{
    // public accept all host connect to server api
    build.WithOrigins("*");
    build.WithOrigins(builder.Configuration["AppSettings:FeEndPoint"] ?? "http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

// For Identity Dành cho phân quyền
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<BaseDbContext>()
    .AddDefaultTokenProviders();

// Kết nối database SqlServer lấy url kết nối thông qua file appsettings.json
builder.Services.AddDbContext<BaseDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Responsitory
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Cấu hình mật khẩu người dùng
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
});

// Thêm xác thực đăng nhập bằng jwt
builder.Services.AddAuthentication((option) =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(otp =>
{
    otp.SaveToken = true;
    otp.RequireHttpsMetadata = false;
    otp.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        ClockSkew = TimeSpan.Zero
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Public static url
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Resources"
});

app.UseCors("ECommerceCSharpApiCors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
