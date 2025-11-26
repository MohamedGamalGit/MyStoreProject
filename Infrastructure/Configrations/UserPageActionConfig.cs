using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;
using Models.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configrations
{
    public class UserPageActionConfig : IEntityTypeConfiguration<UserPageAction>
    {
        public void Configure(EntityTypeBuilder<UserPageAction> builder)
        {
            // Primary Key مركب
            builder.HasKey(upa => new { upa.UserId, upa.PageActionId });

            // العلاقة مع User
            builder.HasOne(upa => upa.User)
                   .WithMany(u => u.UserPageActions)
                   .HasForeignKey(upa => upa.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // العلاقة مع PageAction
            builder.HasOne(upa => upa.PageAction)
                   .WithMany()
                   .HasForeignKey(upa => upa.PageActionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
