using Microsoft.EntityFrameworkCore;
namespace CarInventory
{
	public class CarDb : DbContext // Db needs to inherit from DbContext
	{
		public CarDb(DbContextOptions<CarDb> options) : base(options) { } // Takes in options to pass to DbContext
		public DbSet<Car> Cars { get; set; } // Creates the Cars table in the Db
	}
}
