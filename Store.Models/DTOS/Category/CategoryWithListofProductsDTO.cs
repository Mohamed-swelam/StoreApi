﻿namespace Store.Models.DTOS.Category
{
    public class CategoryWithListofProductsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> productNames { get; set; } = new();
    }
}
