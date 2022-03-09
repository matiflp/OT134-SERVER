using Microsoft.EntityFrameworkCore;
using OngProject.Entities;

namespace OngProject.DataAccess
{
    public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Activities> Activities { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Contacts> Contacts { get; set; }
		public DbSet<Member> Members { get; set; }
		public DbSet<New> News { get; set; }
		public DbSet<Organization> Organizations { get; set; }
		public DbSet<Rol> Roles { get; set; }
		public DbSet<Slides> Slides { get; set; }
		public DbSet<Testimonials> Testimonials { get; set; }
		public DbSet<User> Users { get; set; }

		private readonly SeedDataUser dataUser = new();
		private readonly SeedUserRol rolUser = new();
        private readonly SeedTestimonial seedTestimonial = new();
		private readonly SeedNew seedNew = new();
		private readonly SeedCategory seedCategory = new();
		private readonly SeedActivity seedActivity = new();
		private readonly SeedMember seedMember = new();
		private readonly SeedContact seedContact = new();
		private readonly SeedComment seedComment = new();
		private readonly SeedOrganization seedOrganization = new();
		private readonly SeedSlide seedSlide = new();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			seedCategory.SeedCategories(modelBuilder);
			seedActivity.SeedActivities(modelBuilder);
			rolUser.SeedRoles(modelBuilder);
			dataUser.SeedRegularUsers(modelBuilder);
			dataUser.SeedAdministratorUsers(modelBuilder);
			seedTestimonial.SeedTestimonials(modelBuilder);
			seedNew.SeedNews(modelBuilder);
			seedMember.SeedMembers(modelBuilder);
			seedComment.SeedComments(modelBuilder);
			seedContact.SeedContacts(modelBuilder);
			seedOrganization.SeedOrganizations(modelBuilder);
			seedSlide.SeedSlides(modelBuilder);
		}
	}	
}