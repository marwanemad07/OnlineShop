var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Extensions.AddDbContext(builder.Services, builder.Configuration);

Extensions.AddIdentity(builder.Services);

Extensions.ConfigureIdentityOptions(builder.Services);

Extensions.InjectServices(builder.Services);
Extensions.InjectReposotries(builder.Services);

Extensions.AddCorsPolicy(builder.Services);

Extensions.AddAuthentication(builder.Services, builder.Configuration);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
Extensions.AddSwaggerGen(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("ApiPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
