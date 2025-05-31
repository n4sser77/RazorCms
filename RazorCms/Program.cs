using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using RazorCms.Data;
using RazorCms.DTOs;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login"; // Redirect to login page
    });

builder.Services.AddAuthorization();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IVisitorTrackingService, VisitorTrackingService>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/api/pages/{id}", async (ApplicationDbContext db, int id) =>
{
    var page = db.Pages.FindAsync(id);
    if (page == null) return Results.NotFound("Page not found");
    var blocks = new List<Block>();
    try
    {
        blocks = JsonSerializer.Deserialize<List<Block>>(page.Result.Content);
    }
    catch (Exception e)
    {
        blocks = new List<Block>();
    }

    var pageDto = new RazorCms.DTOs.PageDto()
    {
        Id = page.Result.Id,
        Title = page.Result.Title,
        IsHidden = page.Result.IsHidden,
        OrderIndex = page.Result.OrderIndex,
        Blocks = blocks,
        UserId = page.Result.UserId
    };

    return Results.Ok(pageDto);
});

app.MapPost("/api/pages/save/", async (RazorCms.DTOs.PageDto pageDto, ApplicationDbContext db) =>
{
    if (pageDto == null)
        return Results.BadRequest("Page cannot be null");

    if (string.IsNullOrEmpty(pageDto.Title))
        return Results.BadRequest("Page title cannot be empty");



    if (pageDto.Blocks.Count < 1)
        return Results.BadRequest("Page content cannot be empty");

    var page = new RazorCms.Models.Page()
    {
        Title = pageDto.Title,
        IsHidden = pageDto.IsHidden,
        OrderIndex = pageDto.OrderIndex,
        Content = System.Text.Json.JsonSerializer.Serialize(pageDto.Blocks),
        UserId = pageDto.UserId

    };

    db.Add(page);
    await db.SaveChangesAsync();
    return Results.Created($"/api/pages/{page.Id}", pageDto);
});

app.MapPut("/api/pages/save/", async (RazorCms.DTOs.BatchUpdateDto batchUpdateDto, ApplicationDbContext db) =>
{

    var page = await db.Pages.FindAsync(batchUpdateDto.PageId);
    if (page == null)
    {
        return Results.NotFound("Page not found");
    }
    List<Block> blocks;
    try
    {
        blocks = JsonSerializer.Deserialize<List<Block>>(page.Content);

    }
    catch (Exception e)
    {
        blocks = new List<Block>();
    }

    foreach (var addedBlocks in batchUpdateDto.AddedBlocks)
    {
        blocks.Add(addedBlocks);
    }


    foreach (var editedBlock in batchUpdateDto.EditedBlocks)
    {
        var blockToUpdate = blocks.FirstOrDefault(b => b.Id == editedBlock.Id);
        if (blockToUpdate != null)
        {
            blockToUpdate.Text = editedBlock.Text;
            blockToUpdate.Type = editedBlock.Type;
            blockToUpdate.Url = editedBlock.Url;
            blockToUpdate.Order = editedBlock.Order;

        }

    }
    foreach (var deletedBlockId in batchUpdateDto.DeletedBlockIds)
    {
        var blockToDelete = blocks.FirstOrDefault(b => b.Id == deletedBlockId);
        if (blockToDelete != null)
        {
            blocks.Remove(blockToDelete);
        }
    }

    var jsonContent = JsonSerializer.Serialize<List<Block>>(blocks);
    page.Content = jsonContent;
    await db.SaveChangesAsync();
    return Results.Ok("Page updated successfully");
});

app.Run();
