using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for managing Product entities.
/// This class inherits from the generic Repository class and adds product-specific functionality.
/// </summary>
public class ProductRepository : Repository<Product, Guid>, IProductRepository 
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) : base(context)
    {
    }
    

    // You can add product-specific methods here if needed
}