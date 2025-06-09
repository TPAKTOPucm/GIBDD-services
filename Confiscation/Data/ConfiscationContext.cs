using ConfiscationService.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfiscationService.Data;

public class ConfiscationContext : DbContext
{
    public ConfiscationContext(DbContextOptions options) : base(options) {}
    public DbSet<Vehicle> Vehicles { get; set; }
	public DbSet<ConfiscationOrder> ConfiscationOrders { get; set; }
}
