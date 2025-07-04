﻿using ProductService.Domain.Entities;

namespace ProductService.Application.DTOs.Request
{
    public sealed record ProductDetailsRequestDTO
    {
        required public Guid ProductId { get; init; }
        public string? Description { get; init; }
        public NutritionInfo? Nutrition { get; init; }
        public List<string>? Composition { get; init; }
    }
}
