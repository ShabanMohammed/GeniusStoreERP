using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // السعر
    public decimal Price { get; set; }

    // رصيد المخزن الحالي (تم تعديله لعشري كما اتفقنا)
    public decimal StockQuantity { get; set; }

    // حد الطلب: الكمية التي إذا وصل إليها المخزن يجب إصدار أمر شراء جديد
    // نوعه decimal ليتوافق مع وحدة قياس الصنف (مثلاً تنبيه عند بقاء 0.5 طن)
    public decimal ReorderLevel { get; set; }

    public string? SKU { get; set; }
    public string? Barcode { get; set; }

    // Foreign Key
    public int CategoryId { get; set; }

    // Navigation Property
    public virtual Category Category { get; set; } = null!;
}