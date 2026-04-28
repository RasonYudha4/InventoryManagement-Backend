using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class CategorySeeder
{
    public static async Task SeedAsync(IApplicationDbContext context, CancellationToken cancellationToken)
    {
        if (await context.Categories.AnyAsync(cancellationToken))
            return;

        var electronics = new Category { Name = "Electronics",    Description = "Electronic components and devices" };
        var rawMaterials   = new Category { Name = "Raw Materials",  Description = "Unprocessed production inputs" };
        var packaging      = new Category { Name = "Packaging",      Description = "Boxes, wraps, and containers" };
        var officeSupplies = new Category { Name = "Office Supplies", Description = "Stationery and office consumables" };
        var safety         = new Category { Name = "Safety",         Description = "PPE and safety equipment" };

        var subCategories = new List<Category>
        {
            // Electronics
            new() { Name = "Cables & Connectors",  Description = "Power and data cables",       ParentCategory = electronics },
            new() { Name = "Sensors",              Description = "Industrial sensors",           ParentCategory = electronics },
            new() { Name = "Circuit Boards",       Description = "PCBs and control boards",      ParentCategory = electronics },

            // Raw Materials
            new() { Name = "Metals",               Description = "Steel, aluminium, copper",     ParentCategory = rawMaterials },
            new() { Name = "Plastics",             Description = "Pellets, sheets, and rods",    ParentCategory = rawMaterials },
            new() { Name = "Textiles",             Description = "Fabrics and fibres",           ParentCategory = rawMaterials },

            // Packaging
            new() { Name = "Corrugated Boxes",     Description = "Single and double wall boxes", ParentCategory = packaging },
            new() { Name = "Bubble Wrap",          Description = "Protective air cushioning",    ParentCategory = packaging },
            new() { Name = "Pallets",              Description = "Wooden and plastic pallets",   ParentCategory = packaging },

            // Office Supplies
            new() { Name = "Stationery",           Description = "Pens, paper, and folders",     ParentCategory = officeSupplies },
            new() { Name = "Printer Consumables",  Description = "Ink, toner, and paper rolls",  ParentCategory = officeSupplies },

            // Safety
            new() { Name = "Protective Clothing",  Description = "Gloves, goggles, and vests",   ParentCategory = safety },
            new() { Name = "First Aid",            Description = "Kits and medical supplies",    ParentCategory = safety },
        };

        var rootCategories = new List<Category>
        {
            electronics, rawMaterials, packaging, officeSupplies, safety
        };

        await context.Categories.AddRangeAsync(rootCategories, cancellationToken);
        await context.Categories.AddRangeAsync(subCategories, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}