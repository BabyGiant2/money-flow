using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=moneyflow.db"));

var app = builder.Build();
app.UseCors();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ==========================================
// 🚀 API ENDPOINTS
// ==========================================

// [GET] ดึงข้อมูลทั้งหมด
app.MapGet("/api/transactions", async (AppDbContext db) =>
    await db.Transactions.ToListAsync());

// [POST] เพิ่มข้อมูลใหม่
app.MapPost("/api/transactions", async (AppDbContext db, Transaction t) => {
    db.Transactions.Add(t);
    await db.SaveChangesAsync();
    return Results.Created($"/api/transactions/{t.Id}", t);
});

// [PUT] แก้ไขข้อมูล (อัปเดตฟีเจอร์ใหม่ ✨)
app.MapPut("/api/transactions/{id}", async (int id, AppDbContext db, Transaction updatedTx) => {
    var tx = await db.Transactions.FindAsync(id);
    if (tx is null) return Results.NotFound();
    
    // อัปเดตค่าใหม่
    tx.Date = updatedTx.Date;
    tx.Type = updatedTx.Type;
    tx.Category = updatedTx.Category;
    tx.Note = updatedTx.Note;
    tx.Amount = updatedTx.Amount;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// [DELETE] ลบข้อมูล
app.MapDelete("/api/transactions/{id}", async (int id, AppDbContext db) => {
    if (await db.Transactions.FindAsync(id) is Transaction t) {
        db.Transactions.Remove(t);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    return Results.NotFound();
});

app.Run();

// ==========================================
// 📦 DATA MODELS
// ==========================================
class Transaction {
    public int Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // ฟิลด์หมวดหมู่ที่เพิ่มเข้ามา
    public string Note { get; set; } = string.Empty;
    public double Amount { get; set; }
}

class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Transaction> Transactions => Set<Transaction>();
}