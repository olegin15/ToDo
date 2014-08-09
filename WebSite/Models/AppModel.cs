using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;

namespace ToDoService.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public bool IsDone { get; set; }
        public DateTime? EndDate { get; set; }
    }
    
    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<ToDo> ToDoes { get; set; }
    }
}