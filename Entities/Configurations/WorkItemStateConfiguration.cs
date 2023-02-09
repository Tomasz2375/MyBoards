using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBoards.Entities.Configurations
{
    public class WorkItemStateConfiguration : IEntityTypeConfiguration<WorkItemState>
    {
        public void Configure(EntityTypeBuilder<WorkItemState> builder)
        {
            builder.Property(wis => wis.State)
                .IsRequired()
                .HasMaxLength(60);

            builder.HasData(
                new WorkItemState() { Id = 1, State = "To Do" },
                new WorkItemState() { Id = 2, State = "Doing" },
                new WorkItemState() { Id = 3, State = "Done" });
        }
    }
}
