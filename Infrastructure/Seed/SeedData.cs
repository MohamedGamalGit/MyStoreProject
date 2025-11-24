using Infrastructure.Data;
using Models.Models;
using Models.Models.Permission;

namespace Models.Seed
{
    public static class SeedData
    {
        public static void Seed(StoreDbContext ctx)
        {
            ctx.Database.EnsureCreated();

            //-------------------------------------------------------
            // 1️⃣ Seed default Actions (only if missing)
            //-------------------------------------------------------
            var defaultActions = new[] { "View", "Create", "Edit", "Delete" };

            foreach (var actionKey in defaultActions)
            {
                if (!ctx.ActionEntity.Any(a => a.Key == actionKey))
                {
                    ctx.ActionEntity.Add(new ActionEntity
                    {
                        Key = actionKey,
                        DisplayName = actionKey
                    });
                }
            }

            ctx.SaveChanges();

            //-------------------------------------------------------
            // 2️⃣ Pages are dynamic → we do NOT seed any pages
            //    Only attach actions to existing pages
            //-------------------------------------------------------
            var actions = ctx.ActionEntity.ToList();
            var pages = ctx.Page.ToList();

            //-------------------------------------------------------
            // 3️⃣ Auto-create PageAction (Page + Action pairs)
            //-------------------------------------------------------
            foreach (var page in pages)
            {
                foreach (var action in actions)
                {
                    bool exists = ctx.PageAction.Any(pa =>
                        pa.PageId == page.Id &&
                        pa.ActionEntityId == action.Id
                    );

                    if (!exists)
                    {
                        ctx.PageAction.Add(new PageAction
                        {
                            PageId = page.Id,
                            ActionEntityId = action.Id
                        });
                    }
                }
            }

            ctx.SaveChanges();

            //-------------------------------------------------------
            // 4️⃣ Roles are dynamic → do NOT assign permissions
            //    Admin panel will assign them manually
            //-------------------------------------------------------
            var roles = ctx.Roles.ToList();

            foreach (var role in roles)
            {
                // we intentionally leave roles without permissions
                // user will assign from Angular UI
            }

            ctx.SaveChanges();
        }
    }
}
